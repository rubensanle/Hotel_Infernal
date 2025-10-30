using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 5f;
    public float sprintSpeed = 8f;
    public float gravity = -20f;
    public float jump = 3.2f;
    public float doubleJump = 0.6f;
    public float doubleTap = 0.3f;

    public float dashSpeed = 14f;
    public float dashDuration = 0.4f;

    private Vector3 velocity;
    private bool enSuelo;
    private int jumpCont = 0;
    private float lastJump = 0f;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private Vector3 dashDirection;
    private KeyCode dashKey;

    private Vector3 moveInput;
    private Dictionary<KeyCode, float> lastKeyPress = new Dictionary<KeyCode, float>();

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        enSuelo = controller.isGrounded;
        if (enSuelo && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpCont = 0;
        }

        // Detectar doble pulsación para dash con WASD
        DetectDash(KeyCode.W, transform.forward);
        DetectDash(KeyCode.S, -transform.forward);
        DetectDash(KeyCode.A, -transform.right);
        DetectDash(KeyCode.D, transform.right);

        // Movimiento horizontal con WASD
        float x = 0f;
        float z = 0f;
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;

        moveInput = (transform.right * x + transform.forward * z).normalized;

        // Movimiento
        if (isDashing)
        {
            if (!Input.GetKey(dashKey))
            {
                isDashing = false;
            }
            else
            {
                controller.Move(dashDirection * dashSpeed * Time.deltaTime);
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0f)
                {
                    isDashing = false;
                }
            }
        }
        else
        {
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;
            controller.Move(moveInput * currentSpeed * Time.deltaTime);
        }

        // Salto y doble salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float timeSinceLastJump = Time.time - lastJump;
            lastJump = Time.time;

            if (jumpCont < 2)
            {
                if (jumpCont == 1 && timeSinceLastJump > doubleTap)
                    return;

                float jumpForce = Mathf.Sqrt(jump * -2f * gravity);
                if (jumpCont == 1)
                    jumpForce *= doubleJump;

                velocity.y = jumpForce;
                jumpCont++;
            }
        }

        // Gravedad
        velocity.y += gravity * Time.deltaTime;
        if (velocity.y < -50f)
            velocity.y = -50f;

        controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    void DetectDash(KeyCode key, Vector3 direction)
    {
        if (Input.GetKeyDown(key))
        {
            if (lastKeyPress.ContainsKey(key) && Time.time - lastKeyPress[key] < doubleTap)
            {
                StartDash(direction, key);
            }
            lastKeyPress[key] = Time.time;
        }
    }

    void StartDash(Vector3 direction, KeyCode key)
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashDirection = direction;
        dashKey = key;
    }
}
