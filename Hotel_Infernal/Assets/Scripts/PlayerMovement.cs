using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 5f;
    public float gravity = -20f;
    public float jump = 3.2f;
    public float doubleJump = 0.6f;
    public float doubleTap = 0.4f;

    private Vector3 velocity;
    private bool enSuelo;
    private int jumpCont = 0;
    private float lastJump = 0f;

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

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move.normalized * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            float timeSinceLastJump = Time.time - lastJump;
            lastJump = Time.time;

            if (jumpCont < 2)
            {
                if (jumpCont == 1 && timeSinceLastJump > doubleTap)
                {
                    return;
                }

                float jumpForce = Mathf.Sqrt(jump * -2f * gravity);
                if (jumpCont == 1) 
                    jumpForce *= doubleJump;

                velocity.y = jumpForce;
                jumpCont++;
            }
        }

        velocity.y += gravity * Time.deltaTime;
        if (velocity.y < -50f)
            velocity.y = -50f;

        controller.Move(velocity * Time.deltaTime);
    }
}