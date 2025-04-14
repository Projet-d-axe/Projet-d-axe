using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour 
{
    private PlayerHealth health;
    #region Movement Settings
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 20f;
    [Range(0,1)] [SerializeField] private float airControl = 0.6f;
    #endregion

    #region Jump Settings
    [Header("Jump")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private int maxAirJumps = 1;
    #endregion

    #region Crouch Settings
    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float crouchSpeed = 10f;
    [SerializeField] private float crouchSpeedMultiplier = 0.4f;
    #endregion

    #region Roll Settings
    [Header("Roll")]
    [SerializeField] private float rollDistance = 4f;
    [SerializeField] private float rollDuration = 0.2f;
    [SerializeField] private float rollCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    #endregion

    #region Advanced Movement
    [Header("Advanced")]
    [SerializeField] private float fastFallSpeed = 30f;
    [SerializeField] private float longJumpMultiplier = 1.8f;
    [SerializeField] private float airRollBoost = 1.3f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    #endregion

    #region Aim Settings
    [Header("Aiming")]
    [SerializeField] private bool aimWithMouse = true;
    [SerializeField] private float aimLockMovementThreshold = 0.7f;
    [SerializeField] private float aimMovementReduction = 0.5f;
    private bool isAiming = false;
    #endregion

    #region Components
    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    [SerializeField] private Transform graphics;
    #endregion

    #region State
    private bool isGrounded;
    private bool isCrouching;
    private bool isRolling;
    private bool isFacingRight = true;
    private bool isJumpCut;
    private int remainingJumps;
    private float lastGroundedTime;
    private float nextRollTime;
    private float originalColHeight;
    private float moveInput;
    private float jumpBufferCounter;
    #endregion

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        health.OnDeath.AddListener(HandleDeath);
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        originalColHeight = col.size.y;
        ResetJumps();
        
        if (graphics == null) graphics = transform.Find("Graphics") ?? transform;
    }

    private void Update()
    {
        GetInputs();
        
        if (!isAiming)
        {
            HandleFlip();
        }
        else
        {
            HandleAimDirection();
        }
        
        HandleFastFall();
    }

    private void HandleFlip()
    {
        throw new NotImplementedException();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        if (!isRolling) HandleMovement();
        ApplyJumpGravity();
        HandleCrouch();
    }

    private void HandleDeath()
    {
        enabled = false;
    }

    #region Inputs
    private void GetInputs()
    {
        // Vise avec clique droit
        isAiming = Input.GetMouseButton(1);
        
        // Gestion du mouvement différente selon si on vise ou non
        if (!isAiming)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            float rawInput = Input.GetAxisRaw("Horizontal");
            moveInput = Mathf.Abs(rawInput) > aimLockMovementThreshold ? 0 : rawInput * aimMovementReduction;
        }

        // Jump Buffer
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
            isJumpCut = true;

        isCrouching = Input.GetAxisRaw("Vertical") < 0 && isGrounded;

        if (Input.GetButtonDown("Fire3") && CanRoll())
            StartCoroutine(Roll());
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        float currentMaxSpeed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        float targetSpeed = moveInput * currentMaxSpeed;
        float accel = (Mathf.Abs(moveInput) > 0.1f) ? acceleration : deceleration;
        float control = isGrounded ? 1f : airControl;

        rb.linearVelocity = new Vector2(
            Mathf.Lerp(rb.linearVelocity.x, targetSpeed, accel * control * Time.fixedDeltaTime),
            rb.linearVelocity.y
        );
    }
    #endregion

    #region Aiming
    private void HandleAimDirection()
    {
        if (aimWithMouse)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - transform.position).normalized;
            
            bool shouldFaceRight = direction.x > 0;
            if (shouldFaceRight != isFacingRight)
            {
                Flip();
            }
        }
    }
    #endregion

    #region Flip
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * (isFacingRight ? 1 : -1);
        transform.localScale = newScale;
    }
    #endregion

    #region Jump
    private void TryJump()
    {
        if (CanJump())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            remainingJumps--;
            isJumpCut = false;
        }
    }

    private bool CanJump()
    {
        return (isGrounded || Time.time < lastGroundedTime + coyoteTime || remainingJumps > 0) && !isRolling;
    }

    private void ApplyJumpGravity()
    {
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        else if (rb.linearVelocity.y > 0 && isJumpCut)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
    }

    private void ResetJumps() => remainingJumps = maxAirJumps;
    #endregion

    #region Fast Fall 
    private void HandleFastFall()
    {
        bool isPressingDown = Input.GetAxisRaw("Vertical") < -0.5f;
        if (isPressingDown && !isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fastFallSpeed);
        }
    }
    #endregion

    #region Crouch
    private void HandleCrouch()
    {
        if (isCrouching)
        {
            col.size = new Vector2(
                col.size.x,
                Mathf.MoveTowards(col.size.y, crouchHeight, crouchSpeed * Time.fixedDeltaTime)
            );
        }
        else
        {
            col.size = new Vector2(
                col.size.x,
                Mathf.MoveTowards(col.size.y, originalColHeight, crouchSpeed * Time.fixedDeltaTime)
            );
        }
    }
    #endregion

    #region Roll
    private bool CanRoll() => Time.time >= nextRollTime && !isRolling;

    private IEnumerator Roll()
    {
        isRolling = true;
        float direction = isFacingRight ? 1 : -1;
        float startTime = Time.time;

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        while (Time.time < startTime + rollDuration)
        {
            float progress = (Time.time - startTime) / rollDuration;
            float currentSpeed = Mathf.Lerp(rollDistance / rollDuration, 0, progress);
            rb.linearVelocity = new Vector2(direction * currentSpeed, rb.linearVelocity.y);
            yield return null;
        }

        EndRoll();
    }

    private void EndRoll()
    {
        isRolling = false;
        nextRollTime = Time.time + rollCooldown;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
    }
    #endregion

    #region Ground Check
    private void CheckGrounded()
    {
        bool wasGrounded = isGrounded;
        RaycastHit2D hit = Physics2D.BoxCast(
            col.bounds.center,
            new Vector2(col.bounds.size.x * 0.9f, 0.1f),
            0f,
            Vector2.down,
            col.bounds.extents.y + 0.05f,
            LayerMask.GetMask("Ground")
        );
        
        isGrounded = hit.collider != null;

        if (isGrounded && !wasGrounded) 
        {
            ResetJumps();
            isJumpCut = false;
        }
        else if (!isGrounded && wasGrounded) 
        {
            lastGroundedTime = Time.time;
        }

        if (jumpBufferCounter > 0 && CanJump())
        {
            TryJump();
            jumpBufferCounter = 0;
        }
    }
    #endregion

    #region Debug GUI
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 14;
        style.normal.textColor = Color.white;

        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        GUILayout.BeginVertical("box", style);
        
        GUILayout.Label($"Position: {transform.position}");
        GUILayout.Label($"Velocity: X:{rb.linearVelocity.x:F1} Y:{rb.linearVelocity.y:F1}");
        GUILayout.Label($"Grounded: {isGrounded}", GetStyle(isGrounded));
        GUILayout.Label($"Facing: {(isFacingRight ? "Right" : "Left")}");
        GUILayout.Label($"Aiming: {isAiming}", GetStyle(isAiming));
        GUILayout.Label($"Move Input: {moveInput:F2}");

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private GUIStyle GetStyle(bool condition)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = condition ? Color.green : Color.red;
        return style;
    }
    #endregion
}