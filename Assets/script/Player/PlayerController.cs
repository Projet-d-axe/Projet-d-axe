using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour, iDamageable
{
    // === VARIABLES ===
    [Header("Player Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    // === LAYERS ===
    public LayerMask groundLayerMask;
    public LayerMask enemyLayerMask;

    // === COMPONENTS ===
    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private SpriteRenderer sr;
    private Animator anim;
    public WeaponUI weaponUI;

    // === MOVEMENT ===
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 15f;
    public float deceleration = 20f;
    [Range(0f, 1f)] public float airControl = 0.6f;
    private float moveInput;
    private bool isFacingRight = true;

    // === JUMP ===
    [Header("Jump")]
    public float jumpForce = 16f;
    public float fallMultiplier = 2.5f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.1f;
    public int maxJumpCount = 1;
    private bool isGrounded;
    private float lastGroundedTime;
    private float jumpBufferCounter;
    private int jumpCount;

    // === ROLL ===
    [Header("Roll")]
    public float rollSpeed = 12f;
    public float rollDuration = 0.2f;
    public float rollCooldown = 0.5f;
    public float LongJumpSpeed;
    private bool isRolling = false;
    private float nextRollTime;

    // === CROUCH ===
    [Header("Crouch")]
    public float crouchSpeedMultiplier = 0.5f;
    public float crouchHeightMultiplier = 0.5f;
    private bool isCrouching;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    // === FAST FALL ===
    [Header("Fast Fall")]
    public float fastFallSpeed = 30f;
    private bool isFastFalling;

    // === GROUND CHECK ===
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // === WEAPON ===
    [Header("Weapons")]
    public List<WeaponSystem> weapons;
    private WeaponSystem currentWeapon;
    private int currentWeaponIndex = 0;
    private bool isAiming;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalColliderSize = col.size;
        originalColliderOffset = col.offset;

        currentHealth = maxHealth;

        if (weapons.Count > 0)
            EquipWeapon(0);
    }

    void Update()
    {
        GetInputs();

        if (!isRolling)
        {
            CheckGrounded();
            HandleMovement();
            HandleCrouch();
            HandleFastFall();
            HandleAiming();
            HandleJump();
        }

        HandleRoll();
        HandleWeaponInput();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        ApplyGravityModifier();
    }

    private void GetInputs()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    private void CheckGrounded()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            jumpCount = 0;
            isFastFalling = false;
            if (!wasGrounded) isRolling = false;
        }
        else if (wasGrounded)
        {
            lastGroundedTime = Time.time;
        }
    }

    private void HandleMovement()
    {
        if (isAiming && currentWeapon != null && currentWeapon.lockMovementWhenAiming)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float targetSpeed = moveInput * moveSpeed;
        float accel = Mathf.Abs(moveInput) > 0.1f ? acceleration : deceleration;
        float control = isGrounded ? 1f : airControl;

        if (isCrouching) targetSpeed *= crouchSpeedMultiplier;
        if (isAiming && currentWeapon != null) targetSpeed *= currentWeapon.aimingMoveSpeedMultiplier;

        float speedDifference = targetSpeed - rb.linearVelocity.x;
        float movement = speedDifference * accel * control;

        rb.AddForce(new Vector2(movement, 0));

        if (moveInput != 0 && !isRolling)
            FlipCharacter();
    }

    private void FlipCharacter()
    {
        if ((moveInput > 0 && !isFacingRight) || (moveInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void HandleJump()
    {
        if (jumpBufferCounter > 0 && (isGrounded || Time.time < lastGroundedTime + coyoteTime || jumpCount < maxJumpCount))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0;
            jumpCount++;
        }
    }

    private void ApplyGravityModifier()
    {
        if (!isFastFalling && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void HandleCrouch()
    {
        bool inputDown = Input.GetAxisRaw("Vertical") < 0;

        if (inputDown && isGrounded && !isCrouching)
        {
            isCrouching = true;
            col.size = new Vector2(originalColliderSize.x, originalColliderSize.y * crouchHeightMultiplier);
            col.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y * crouchHeightMultiplier);
        }
        else if (!inputDown && isCrouching && !CheckCeiling())
        {
            isCrouching = false;
            col.size = originalColliderSize;
            col.offset = originalColliderOffset;
        }
    }

    private bool CheckCeiling()
    {
        float checkHeight = originalColliderSize.y - col.size.y;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, checkHeight, groundLayer);
        return hit.collider != null;
    }

    private void HandleFastFall()
    {
        if (!isGrounded && Input.GetAxisRaw("Vertical") < 0 && rb.linearVelocity.y < 0)
        {
            isFastFalling = true;
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fastFallSpeed - 1) * Time.deltaTime;
        }
        else
        {
            isFastFalling = false;
        }
    }

    private void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextRollTime)
        {
            float dir = Mathf.Sign(moveInput != 0 ? moveInput : transform.localScale.x);
            StartCoroutine(PerformRoll(dir));
        }
    }

    private IEnumerator PerformRoll(float direction)
    {
        isRolling = true;
        nextRollTime = Time.time + rollCooldown;

        float startTime = Time.time;
        float rollTime = isGrounded ? rollDuration : rollDuration * 1.5f;
        float speed = isGrounded ? rollSpeed : rollSpeed * 0.8f;

        while (Time.time < startTime + rollTime)
        {
            rb.linearVelocity = new Vector2(speed * direction, rb.linearVelocity.y);
            yield return null;
        }

        isRolling = false;
    }

    private void HandleAiming()
    {
        isAiming = Input.GetMouseButton(1) || Input.GetAxis("Aim") > 0.1f;

        if (isAiming && Camera.main)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localScale = new Vector3(
                (mousePos.x < transform.position.x) ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    private void HandleWeaponInput()
    {
        if (currentWeapon == null) return;

        if (Input.GetButton("Fire1"))
            currentWeapon.TryShoot();

        if (Input.GetKeyDown(KeyCode.R))
            currentWeapon.Reload();

        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Count > 1) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Count > 2) EquipWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4) && weapons.Count > 3) EquipWeapon(3);
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count) return;

        foreach (var weapon in weapons)
            weapon.gameObject.SetActive(false);

        currentWeaponIndex = index;
        currentWeapon = weapons[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);

        if (weaponUI)
            weaponUI.SetCurrentWeapon(currentWeapon, currentWeaponIndex);
    }

    private void UpdateAnimations()
    {
        anim.SetFloat("VelocityX", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("VelocityY", rb.linearVelocity.y);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsCrouching", isCrouching);
        anim.SetBool("IsRolling", isRolling);
        anim.SetBool("IsAiming", isAiming);
        anim.SetBool("IsFastFalling", isFastFalling);

        if (currentWeapon != null)
            anim.SetInteger("CurrentAmmo", currentWeapon.GetCurrentAmmo());
    }

    public void Damage(int damageAmount)
    {
        if (isInvincible) return;

        currentHealth -= damageAmount;
        StartCoroutine(InvincibilityFlash());

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator InvincibilityFlash()
    {
        isInvincible = true;
        float flashDuration = invincibilityDuration / 10;
        
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
        }

        isInvincible = false;
    }

    private void Die()
    {
        anim.SetTrigger("Die");
        enabled = false;
        // Ajouter ici d'autres effets de mort si nécessaire
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}