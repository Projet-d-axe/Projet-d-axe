using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(Animator))]
public class PlayerController2D : MonoBehaviour
{
    // MOVEMENT
    [Header("MOVEMENT")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 20f;
    [Range(0f, 1f)] [SerializeField] private float airControl = 0.5f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // JUMP
    [Header("JUMP")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private int maxJumps = 1;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float fallMultiplier = 2f;
    [SerializeField] private float lowJumpMultiplier = 1.5f;

    // DASH
    [Header("DASH")]
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private ParticleSystem dashParticles;

    // SHOOT
    [Header("SHOOT")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float recoilForce = 2f;
    [SerializeField] private AudioClip shootSound;

    // COMPONENTS
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;
    private bool isFacingRight = true;

    // STATE
    private bool isGrounded;
    private bool isDashing;
    private int jumpCount;
    private float lastGroundedTime;
    private float lastJumpPressTime;
    private float lastDashTime;
    private float nextFireTime;
    private float originalGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        // Input and Timers
        HandleJumpInput();
        HandleDashInput();
        HandleShooting();
        UpdateTimers();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        // Physics
        GroundCheck();
        if (!isDashing) HandleMovement();
        HandleGravity();
    }

    #region MOVEMENT
    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        float targetSpeed = moveInput * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        
        // Air control modifier
        if (!isGrounded) accelRate *= airControl;

        rb.AddForce(Vector2.right * speedDiff * accelRate);

        // Flip handling
        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    #endregion

    #region JUMP
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

    private void HandleGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = originalGravity * fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.gravityScale = originalGravity * lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = originalGravity;
        }
    }
    #endregion

    #region DASH
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
        rb.gravityScale = 0;
        
        if (dashParticles) dashParticles.Play();
        StartCoroutine(StopDashAfterTime(dashDuration));
    }

    private IEnumerator StopDashAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        StopDash();
    }

    private void StopDash()
    {
        isDashing = false;
        rb.gravityScale = originalGravity;
    }
    #endregion

    #region SHOOT
    private void HandleShooting()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && !isDashing)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
            
            // Recoil effect
            if (isGrounded) rb.AddForce(new Vector2(isFacingRight ? -recoilForce : recoilForce, 0), ForceMode2D.Impulse);
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(direction, bulletSpeed, gameObject.tag);
        }
        else
        {
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.linearVelocity = direction * bulletSpeed;
        }

        anim.SetTrigger("Shoot");
        if (shootSound) audioSource.PlayOneShot(shootSound);
    }
    #endregion

    #region UTILITIES
    private void GroundCheck()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);

        if (isGrounded && !wasGrounded)
        {
            lastGroundedTime = Time.time;
            jumpCount = 0;
        }
    }

    private void UpdateTimers()
    {
        // Automatic jump buffer expiration
        if (Time.time - lastJumpPressTime > jumpBufferTime + 0.1f)
        {
            lastJumpPressTime = float.MinValue;
        }
    }

    private void UpdateAnimations()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        anim.SetBool("IsDashing", isDashing);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
    #endregion
}