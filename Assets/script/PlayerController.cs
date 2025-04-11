using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Mouvement
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _acceleration = 15f;
    [SerializeField] private float _deceleration = 20f;
    [Range(0, 1)] [SerializeField] private float _airControl = 0.6f;

    // Saut
    [SerializeField] private float _jumpForce = 16f;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _coyoteTime = 0.15f;
    [SerializeField] private int _maxAirJumps = 1;

    // Composants
    private Rigidbody2D _rb;
    private bool _isGrounded;
    private int _remainingJumps;
    private float _lastGroundedTime;
    private float _moveInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _remainingJumps = _maxAirJumps;
    }

    private void Update()
    {
        // Input basique
        _moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump")) TryJump();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        Move();
        ApplyJumpGravity();
    }

    private void Move()
    {
        float targetSpeed = _moveInput * _speed;
        float acceleration = (Mathf.Abs(_moveInput) > 0.1f) ? _acceleration : _deceleration;
        float control = _isGrounded ? 1f : _airControl;

        float newSpeed = Mathf.Lerp(_rb.linearVelocity.x, targetSpeed, acceleration * control * Time.fixedDeltaTime);
        _rb.linearVelocity = new Vector2(newSpeed, _rb.linearVelocity.y);
    }

    private void TryJump()
    {
        if (_isGrounded || Time.time < _lastGroundedTime + _coyoteTime || _remainingJumps > 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            if (!_isGrounded) _remainingJumps--;
        }
    }

    private void CheckGrounded()
    {
        bool wasGrounded = _isGrounded;
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);

        if (_isGrounded && !wasGrounded) _remainingJumps = _maxAirJumps;
        else if (!_isGrounded && wasGrounded) _lastGroundedTime = Time.time;
    }

    private void ApplyJumpGravity()
    {
        if (_rb.linearVelocity.y < 0)
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime;
    }
}