using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour 
{
    //=== Références ===//
    [Header("Références")]
    [SerializeField] private Transform _graphics;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private TextMeshProUGUI _bulletTypeText;
    [SerializeField] private Transform _aimPivot;
    [SerializeField] private LayerMask _enemyLayer;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private Weapon _weapon;
    private PlayerHealth _health;

    //=== Paramètres de Mouvement ===//
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _acceleration = 15f;
    [SerializeField] private float _deceleration = 20f;
    [Range(0,1)] [SerializeField] private float _airControl = 0.6f;

    //=== Saut ===//
    [Header("Jump")]
    [SerializeField] private float _jumpForce = 16f;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _lowJumpMultiplier = 2f;
    [SerializeField] private float _coyoteTime = 0.15f;
    [SerializeField] private int _maxAirJumps = 1;

    //=== Crouch ===//
    [Header("Crouch")]
    [SerializeField] private float _crouchHeight = 0.5f;
    [SerializeField] private float _crouchSpeed = 10f;
    [SerializeField] private float _crouchSpeedMultiplier = 0.4f;

    //=== Roulade ===//
    [Header("Roll")]
    [SerializeField] private float _rollDistance = 4f;
    [SerializeField] private float _rollDuration = 0.2f;
    [SerializeField] private float _rollCooldown = 0.5f;

    //=== Mouvement Avancé ===//
    [Header("Advanced")]
    [SerializeField] private float _fastFallSpeed = 30f;
    [SerializeField] private float _jumpBufferTime = 0.1f;

    //=== Visée ===//
    [Header("Aiming")]
    [SerializeField] private bool _aimWithMouse = true;
    [SerializeField] private float _aimLockMovementThreshold = 0.7f;
    [SerializeField] private float _aimMovementReduction = 0.5f;
    [SerializeField] private float _aimRotationOffset = 90f;

    //=== États ===//
    private bool _isGrounded;
    private bool _isCrouching;
    private bool _isRolling;
    private bool _isFacingRight = true;
    private bool _isJumpCut;
    private bool _isAiming;
    private int _remainingJumps;
    private float _lastGroundedTime;
    private float _nextRollTime;
    private float _originalColHeight;
    private float _moveInput;
    private float _jumpBufferCounter;
    private Vector2 _aimDirection;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();
        _health = GetComponent<PlayerHealth>();
        _weapon = GetComponentInChildren<Weapon>();
        _originalColHeight = _col.size.y;

        if (_graphics == null) _graphics = transform;
        if (_health != null) _health.OnDeath.AddListener(HandleDeath);

        ResetJumps();
    }

    void Update()
    {
        GetInputs();
        HandleAiming();
        UpdateUI();
    }

    void FixedUpdate()
    {
        CheckGrounded();
        if (!_isRolling) HandleMovement();
        ApplyJumpGravity();
        HandleCrouch();
    }

    #region Inputs
    private void GetInputs()
    {
        // Vise avec clic droit
        _isAiming = Input.GetMouseButton(1);

        // Mouvement
        if (!_isAiming)
        {
            _moveInput = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            float rawInput = Input.GetAxisRaw("Horizontal");
            _moveInput = Mathf.Abs(rawInput) > _aimLockMovementThreshold ? 0 : rawInput * _aimMovementReduction;
        }

        // Tir avec clic gauche
        if (Input.GetMouseButton(0) && _weapon != null)
        {
            _weapon.AttemptShoot(_aimDirection);
        }

        // Changement de type de balle
        if (Input.GetKeyDown(KeyCode.Alpha1)) _weapon?.SetBulletType(Weapon.BulletType.Normal);
        if (Input.GetKeyDown(KeyCode.Alpha2)) _weapon?.SetBulletType(Weapon.BulletType.Platform);
        if (Input.GetKeyDown(KeyCode.Alpha3)) _weapon?.SetBulletType(Weapon.BulletType.Piercing);

        // Saut
        if (_jumpBufferCounter > 0 && CanJump())
        {
            TryJump();
            _jumpBufferCounter = 0;
        }
        if (Input.GetButtonDown("Jump"))
            _jumpBufferCounter = _jumpBufferTime;
        else
            _jumpBufferCounter -= Time.deltaTime;

        if (Input.GetButtonUp("Jump") && _rb.linearVelocity.y > 0)
            _isJumpCut = true;

        // Crouch
        _isCrouching = Input.GetAxisRaw("Vertical") < 0 && _isGrounded;

        // Roulade
        if (Input.GetButtonDown("Fire3") && CanRoll())
            StartCoroutine(Roll());
    }
    #endregion

    #region Mouvement
    private void HandleMovement()
    {
        float currentMaxSpeed = _isCrouching ? _moveSpeed * _crouchSpeedMultiplier : _moveSpeed;
        float targetSpeed = _moveInput * currentMaxSpeed;
        float accelRate = (Mathf.Abs(_moveInput) > 0.1f) ? _acceleration : _deceleration;
        float control = _isGrounded ? 1f : _airControl;

        _rb.linearVelocity = new Vector2(
            Mathf.Lerp(_rb.linearVelocity.x, targetSpeed, accelRate * control * Time.fixedDeltaTime),
            _rb.linearVelocity.y
        );

        if (!_isAiming && _moveInput != 0)
            CheckFlip();
    }

    private void CheckFlip()
    {
        bool shouldFlip = (_moveInput > 0 && !_isFacingRight) || (_moveInput < 0 && _isFacingRight);
        if (shouldFlip) Flip();
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    #endregion

    #region Visée
    private void HandleAiming()
    {
        if (_weapon == null || _aimPivot == null) return;

        // Calcul de la direction de visée
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _aimDirection = (mousePos - (Vector2)_aimPivot.position).normalized;

        // Rotation de l'arme
        float angle = Mathf.Atan2(_aimDirection.y, _aimDirection.x) * Mathf.Rad2Deg;
        _weapon.transform.rotation = Quaternion.Euler(0, 0, angle + _aimRotationOffset);

        // Flip du personnage en visée
        if (_isAiming)
        {
            bool shouldFaceRight = _aimDirection.x > 0;
            if (shouldFaceRight != _isFacingRight) Flip();
        }
    }
    #endregion

    #region Saut
    private bool CanJump() => (_isGrounded || Time.time < _lastGroundedTime + _coyoteTime || _remainingJumps > 0) && !_isRolling;

    private void TryJump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
        _remainingJumps--;
        _isJumpCut = false;
    }

    private void ApplyJumpGravity()
    {
        if (_rb.linearVelocity.y < 0)
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime;
        else if (_rb.linearVelocity.y > 0 && _isJumpCut)
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.fixedDeltaTime;
    }

    private void ResetJumps() => _remainingJumps = _maxAirJumps;
    #endregion

    #region Chute Rapide
    private void HandleFastFall()
    {
        bool isPressingDown = Input.GetAxisRaw("Vertical") < -0.5f;
        if (isPressingDown && !_isGrounded && _rb.linearVelocity.y < 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -_fastFallSpeed);
        }
    }
    #endregion

    #region Crouch
    private void HandleCrouch()
    {
        if (_isCrouching)
        {
            _col.size = new Vector2(
                _col.size.x,
                Mathf.MoveTowards(_col.size.y, _crouchHeight, _crouchSpeed * Time.fixedDeltaTime)
            );
        }
        else
        {
            _col.size = new Vector2(
                _col.size.x,
                Mathf.MoveTowards(_col.size.y, _originalColHeight, _crouchSpeed * Time.fixedDeltaTime)
            );
        }
    }
    #endregion

    #region Roulade
    private bool CanRoll() => Time.time >= _nextRollTime && !_isRolling;

    private IEnumerator Roll()
    {
        _isRolling = true;
        float direction = _isFacingRight ? 1 : -1;
        float startTime = Time.time;

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        while (Time.time < startTime + _rollDuration)
        {
            float progress = (Time.time - startTime) / _rollDuration;
            float currentSpeed = Mathf.Lerp(_rollDistance / _rollDuration, 0, progress);
            _rb.linearVelocity = new Vector2(direction * currentSpeed, _rb.linearVelocity.y);
            yield return null;
        }

        EndRoll();
    }

    private void EndRoll()
    {
        _isRolling = false;
        _nextRollTime = Time.time + _rollCooldown;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
    }
    #endregion

    #region Vérification Sol
    private void CheckGrounded()
    {
        bool wasGrounded = _isGrounded;
        RaycastHit2D hit = Physics2D.BoxCast(
            _col.bounds.center,
            new Vector2(_col.bounds.size.x * 0.9f, 0.1f),
            0f,
            Vector2.down,
            _col.bounds.extents.y + 0.05f,
            _groundLayer
        );
        
        _isGrounded = hit.collider != null;

        if (_isGrounded && !wasGrounded)
        {
            ResetJumps();
            _isJumpCut = false;
        }
        else if (!_isGrounded && wasGrounded)
        {
            _lastGroundedTime = Time.time;
        }
    }
    #endregion

    #region UI
    private void UpdateUI()
    {
        if (_weapon == null) return;

        // Munitions
        if (_ammoText != null)
        {
            _ammoText.text = $"{_weapon.CurrentAmmo}/{_weapon.MaxAmmo}";
            _ammoText.color = _weapon.CurrentAmmo > 0 ? Color.green : Color.red;
        }

        // Type de balle
        if (_bulletTypeText != null)
        {
            _bulletTypeText.text = _weapon.CurrentBulletType.ToString();
            _bulletTypeText.color = GetBulletColor(_weapon.CurrentBulletType);
        }
    }

    private Color GetBulletColor(Weapon.BulletType type)
    {
        return type switch
        {
            Weapon.BulletType.Normal => Color.white,
            Weapon.BulletType.Platform => Color.blue,
            Weapon.BulletType.Piercing => Color.red,
            _ => Color.gray
        };
    }
    #endregion

    #region Autres
    private void HandleDeath()
    {
        enabled = false;
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 14;
        style.normal.textColor = Color.white;

        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        GUILayout.BeginVertical("box", style);
        
        GUILayout.Label($"Position: {transform.position}");
        GUILayout.Label($"Velocity: X:{_rb.linearVelocity.x:F1} Y:{_rb.linearVelocity.y:F1}");
        GUILayout.Label($"Grounded: {_isGrounded}");
        GUILayout.Label($"Facing: {(_isFacingRight ? "Right" : "Left")}");
        GUILayout.Label($"Aiming: {_isAiming}");
        GUILayout.Label($"Move Input: {_moveInput:F2}");

        if (_weapon != null)
        {
            GUILayout.Label($"Bullet Type: {_weapon.CurrentBulletType}");
            GUILayout.Label($"Ammo: {_weapon.CurrentAmmo}/{_weapon.MaxAmmo}");
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    #endregion
}