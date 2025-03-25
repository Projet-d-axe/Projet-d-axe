using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;
    public float acceleration = 15f;
    public float deceleration = 20f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public int maxJumps = 1;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;

    [Header("Dash")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    // Components
    private Rigidbody2D rb;
    private Animator anim;
    private bool isFacingRight = true;

    // State
    private bool isGrounded;
    private bool isDashing;
    private int jumpCount;
    private float lastGroundedTime;
    private float lastJumpPressTime;
    private float lastDashTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleJumpInput();
        HandleDashInput();
        UpdateTimers();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        GroundCheck();
        if (!isDashing) HandleMovement();
    }

    private void HandleMovement()
    {
        float targetSpeed = Input.GetAxisRaw("Horizontal") * speed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        rb.AddForce(Vector2.right * speedDiff * accelRate);

        // Flip sprite
        if (targetSpeed > 0 && !isFacingRight) Flip();
        else if (targetSpeed < 0 && isFacingRight) Flip();
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            lastJumpPressTime = Time.time;
            TryJump();
        }
    }

    private void TryJump()
    {
        bool canCoyoteJump = Time.time - lastGroundedTime <= coyoteTime;
        bool hasJumpBuffer = Time.time - lastJumpPressTime <= jumpBufferTime;

        if ((isGrounded || canCoyoteJump || jumpCount < maxJumps) && hasJumpBuffer)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
            lastJumpPressTime = float.MinValue;
        }
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        lastDashTime = Time.time;
        rb.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * dashSpeed, 0);
        Invoke(nameof(StopDash), dashDuration);
    }

    private void StopDash() => isDashing = false;

    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        isGrounded = hit.collider != null;

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            jumpCount = 0;
        }
    }

    private void UpdateTimers()
    {
        // Reset dash when grounded
        if (isGrounded && Time.time >= lastDashTime + dashCooldown)
        {
            isDashing = false;
        }
    }

    private void UpdateAnimations()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsDashing", isDashing);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // Public method for weapon knockback
    public void ApplyKnockback(Vector2 force) => rb.AddForce(force, ForceMode2D.Impulse);
}