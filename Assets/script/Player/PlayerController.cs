using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    // === VARIABLES ===
    [Header("Player Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    // === LAYERS ===
    public LayerMask groundLayerMask;
    public LayerMask enemyLayerMask;

    // === TAGS ===
    public string playerTag = "Player";
    public string enemyTag = "Enemy";


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
    private float xSpeed;

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
    private float ySpeed;

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
    public TokenSystem tokenSystem;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        originalColliderSize = col.size;
        originalColliderOffset = col.offset;

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
        if (!isFastFalling)
            ApplyGravityModifier();

        ySpeed = rb.linearVelocityY;
        xSpeed = Mathf.Abs(rb.linearVelocityX);

        
        anim.SetFloat("ySpeed", ySpeed);
        anim.SetFloat("xSpeed", Mathf.Abs(moveInput));
       

        
    }

    private void GetInputs()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
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

        rb.linearVelocity = new Vector2(
            Mathf.Lerp(rb.linearVelocity.x, targetSpeed, accel * control * Time.deltaTime),
            rb.linearVelocity.y
        );

        if (moveInput != 0 && !isRolling)
            FlipCharacter();
    }

    private void FlipCharacter()
    {
        if ((moveInput > 0 && !isFacingRight) || (moveInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(
                -transform.localScale.x,
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    private void HandleJump()
    {
        if (jumpBufferCounter > 0 && (isGrounded || Time.time < lastGroundedTime + coyoteTime || jumpCount < maxJumpCount))
        {
            anim.SetTrigger("jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0;
            jumpCount++;
        }
    }

    private void ApplyGravityModifier()
    {
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, checkHeight, ~LayerMask.GetMask("Player"));
        return hit.collider != null;
    }

    private void HandleFastFall()
    {
        if (!isGrounded && Input.GetAxisRaw("Vertical") < 0 && rb.linearVelocity.y < 0)
        {
            isFastFalling = true;
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fastFallSpeed - 1f) * Time.deltaTime;
        }
        else
        {
            isFastFalling = false;
        }
    }

    private void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextRollTime && isGrounded)
        {
            float dir = Mathf.Sign(moveInput != 0 ? moveInput : transform.localScale.x);
            anim.SetTrigger("roll");
            StartCoroutine(PerformRoll(dir));
        }
    }

    private IEnumerator PerformRoll(float direction)
    {
        bool rollJump = false;
        isRolling = true;
        Debug.Log("Start Roll");
        nextRollTime = Time.time + rollCooldown;

        float startTime = Time.time;
        float rollTime = isGrounded ? rollDuration : rollDuration * 1.5f;
        float speed = isGrounded ? rollSpeed : rollSpeed * 0.8f;

        while (Time.time < startTime + rollTime)
        {
            if (!rollJump)
            {
                rb.linearVelocity = new Vector2(speed * direction, rb.linearVelocity.y);
                yield return null;
            }
            

            if (Input.GetButtonDown("Jump") || rollJump)
            {
                rb.linearVelocity = new Vector2(LongJumpSpeed * direction, jumpForce);
                rollJump = true;
                Debug.Log("Long Jump !");
            }
            yield return null;
        }

        isRolling = false;
        Debug.Log("Stop Roll");
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

        // Changement d'arme avec vérification
        if (Input.GetKeyDown(KeyCode.Alpha1)) TrySwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TrySwitchWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TrySwitchWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TrySwitchWeapon(3);
    }

    private void TrySwitchWeapon(int index)
    {
        if (index < weapons.Count && tokenSystem.IsWeaponUnlocked(index))
        {
            EquipWeapon(index);
        }
        else
        {
            tokenSystem.PlayLockedWeaponFeedback();
        }
    }
    
    private void EquipWeapon(int index)
    {
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

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void ForceStun()
    {

    }
}
