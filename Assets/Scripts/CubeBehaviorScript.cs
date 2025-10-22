using UnityEngine;

public class CubeBehaviorScript : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isJumping = false;
    private float jumpHeight = 1f;
    private float jumpDuration = 0.5f;
    private float jumpTimer = 0f;
    private float angryJumpInterval = 1f;
    private float angryTimer = 0f;

    void Start()
    {
        originalPosition = transform.position;
    }

    public void StartJumping()
    {
        isJumping = true;
    }

    public void StopJumping()
    {
        isJumping = false;
        transform.position = originalPosition;
    }

    void Update()
    {
        if (isJumping)
        {
            angryTimer += Time.deltaTime;
            if (angryTimer >= angryJumpInterval)
            {
                PerformJump();
                angryTimer = 0f;
            }
        }

        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
            float progress = 1 - (jumpTimer / jumpDuration);
            float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            transform.position = originalPosition + new Vector3(0, height, 0);
            if (jumpTimer <= 0)
            {
                transform.position = originalPosition;
            }
        }
    }

    private void PerformJump()
    {
        jumpTimer = jumpDuration;
    }
}