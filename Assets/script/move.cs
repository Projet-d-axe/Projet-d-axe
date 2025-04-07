using UnityEngine;
using System.Collections;

public class AdvancedMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 8f;
    public int maxJumpCount = 1;
    public float fastFallMultiplier = 2f;
    
    [Header("Roll Settings")]
    public float rollSpeed = 20f;
    public float rollDuration = 0.3f;
    public float rollCooldown = 1f;
    public float airRollDistance = 3f;
    public float longJumpForce = 10f;
    
    [Header("Crouch Settings")]
    public float crouchSpeedMultiplier = 0.5f;
    public float crouchHeightMultiplier = 0.5f;
    
    [Header("Aiming Settings")]
    public bool lockMovementWhenAiming = false;
    public float aimingMoveSpeedMultiplier = 0.5f;
    
    [Header("Shooting Settings")]
    public bool canShootWhileMoving = true;
    public bool canShootWhileRolling = false;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.2f;
    public int maxAmmo = 10;
    public float reloadTime = 1f;
    
    private Rigidbody2D rb;
    private CapsuleCollider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    private bool isGrounded;
    private int jumpCount;
    private Vector3 originalScale;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    
    private float lastRollTime = -Mathf.Infinity;
    private bool isRolling = false;
    private bool isCrouching = false;
    private bool isAiming = false;
    private bool isFastFalling = false;
    private bool isReloading = false;
    private int currentAmmo;
    private float nextFireTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        originalScale = transform.localScale;
        originalColliderSize = playerCollider.size;
        originalColliderOffset = playerCollider.offset;
        
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (!isRolling) // Only allow other movements when not rolling
        {
            GroundCheck();
            AimCheck();
            MoveCheck();
            FlipCheck();
            CrouchCheck();
            JumpCheck();
            FastFallCheck();
        }
        
        RollCheck();
        ShootCheck();
        AnimCheck();
    }

    void GroundCheck()
    {
        isGrounded = false;
        float rayLength = playerCollider.size.x * 0.45f;
        
        Vector2 rayOrigin = (Vector2)transform.position + 
                          Vector2.up * (playerCollider.offset.y + rayLength * 0.8f - (playerCollider.size.y / 2));
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(rayOrigin, rayLength);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider != playerCollider && !collider.isTrigger)
            {
                isGrounded = true;
                jumpCount = 0;
                isRolling = false;
                break;
            }
        }
    }

    void AimCheck()
    {
        isAiming = Input.GetMouseButton(1) || Input.GetAxis("Aim") > 0.1f;
        
        // Si on vise avec la souris, orienter le personnage vers le curseur
        if (isAiming && Camera.main != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mousePos.x < transform.position.x)
                transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            else
                transform.localScale = originalScale;
        }
    }

    void MoveCheck()
    {
        // Si on est en train de viser et que le mouvement est verrouillé pendant la visée
        if (isAiming && lockMovementWhenAiming) 
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // On garde seulement la vélocité verticale
            return;
        }

        float moveInput = Input.GetAxis("Horizontal"); 
        float currentSpeed = speed;

        // Application des modificateurs de vitesse
        if (isCrouching) currentSpeed *= crouchSpeedMultiplier;
        if (isAiming) currentSpeed *= aimingMoveSpeedMultiplier;

        // Application du mouvement
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    void CrouchCheck()
    {
        bool crouchInput = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.S);
        
        if (crouchInput && !isCrouching && isGrounded)
        {
            isCrouching = true;
            playerCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * crouchHeightMultiplier);
            playerCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y * crouchHeightMultiplier);
        }
        else if (!crouchInput && isCrouching)
        {
            if (!CheckCeiling())
            {
                isCrouching = false;
                playerCollider.size = originalColliderSize;
                playerCollider.offset = originalColliderOffset;
            }
        }
    }

            bool CheckCeiling()
    {
        float rayLength = originalColliderSize.y - playerCollider.size.y;
        Vector2 rayOriginLeft = (Vector2)transform.position + 
                              new Vector2(-playerCollider.size.x * 0.4f, playerCollider.offset.y);
        Vector2 rayOriginRight = (Vector2)transform.position + 
                               new Vector2(playerCollider.size.x * 0.4f, playerCollider.offset.y);

        // Tirez deux rayons (gauche et droite) pour une meilleure détection
        RaycastHit2D hitLeft = Physics2D.Raycast(
            rayOriginLeft, 
            Vector2.up, 
            rayLength, 
            ~LayerMask.GetMask("Player")
        );
        RaycastHit2D hitRight = Physics2D.Raycast(
            rayOriginRight, 
            Vector2.up, 
            rayLength, 
            ~LayerMask.GetMask("Player")
        );
        
        return hitLeft.collider != null || hitRight.collider != null;
    }

    void JumpCheck()
    {
        if (Input.GetButtonDown("Jump") && (isGrounded || jumpCount < maxJumpCount))
        {
            if (isRolling && isGrounded)
            {
                rb.linearVelocity = new Vector2(transform.localScale.x > 0 ? longJumpForce : -longJumpForce, jumpForce);
                isRolling = false;
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            jumpCount++;
        }
    }

    void FastFallCheck()
    {
        if (!isGrounded && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.LeftControl)))
        {
            isFastFalling = true;
            rb.linearVelocity += Vector2.down * fastFallMultiplier;
        }
        else
        {
            isFastFalling = false;
        }
    }

    void RollCheck()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastRollTime + rollCooldown)
        {
            float direction = Mathf.Sign(Input.GetAxis("Horizontal"));
            if (direction == 0) direction = transform.localScale.x > 0 ? 1 : -1;
            
            StartCoroutine(PerformRoll(direction));
        }
    }

    IEnumerator PerformRoll(float direction)
    {
        isRolling = true;
        lastRollTime = Time.time;
        
        float startTime = Time.time;
        
        if (isGrounded)
        {
            while (Time.time < startTime + rollDuration)
            {
                rb.linearVelocity = new Vector2(rollSpeed * direction, 0);
                yield return null;
            }
        }
        else
        {
            while (Time.time < startTime + rollDuration * 1.5f)
            {
                rb.linearVelocity = new Vector2(rollSpeed * direction * 0.8f, rb.linearVelocity.y);
                yield return null;
            }
        }
        
        isRolling = false;
    }

    void FlipCheck()
    {
        if (isAiming && lockMovementWhenAiming) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        
        if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (horizontalInput > 0)
        {
            transform.localScale = originalScale;
        }
    }

    void ShootCheck()
    {
        // Rechargement
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }

        // Tir
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && !isReloading)
        {
            bool canShoot = canShootWhileMoving && (!isRolling || canShootWhileRolling);
            
            if (canShoot && currentAmmo > 0)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
                currentAmmo--;
                
                if (currentAmmo <= 0)
                {
                    StartCoroutine(Reload());
                }
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Projectile prefab or fire point not set!");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rbProjectile = projectile.GetComponent<Rigidbody2D>();
        
        // Déterminer la direction du tir en fonction de l'orientation du personnage
        float direction = transform.localScale.x > 0 ? 1 : -1;
        
        if (isAiming && Camera.main != null)
        {
            // Si on vise avec la souris, calculer la direction vers le curseur
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 directionVector = (mousePos - firePoint.position).normalized;
            rbProjectile.linearVelocity = directionVector * projectileSpeed;
        }
        else
        {
            // Tirer dans la direction du personnage
            rbProjectile.linearVelocity = new Vector2(direction * projectileSpeed, 0);
        }
        
        // Animation de tir
        animator.SetTrigger("Shoot");
    }

    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("IsReloading", true);
        
        yield return new WaitForSeconds(reloadTime);
        
        currentAmmo = maxAmmo;
        isReloading = false;
        animator.SetBool("IsReloading", false);
    }

    void AnimCheck()
    {
        animator.SetFloat("VelocityX", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("VelocityY", rb.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsRolling", isRolling);
        animator.SetBool("IsAiming", isAiming);
        animator.SetBool("IsFastFalling", isFastFalling);
        animator.SetInteger("CurrentAmmo", currentAmmo);
    }

    private void OnDrawGizmos()
    {
        if (playerCollider == null)
            playerCollider = GetComponent<CapsuleCollider2D>();

        float rayLength = playerCollider.size.x * 0.45f;
        Gizmos.color = Color.magenta;
        
        Vector2 rayOrigin = (Vector2)transform.position + 
                          Vector2.up * (playerCollider.offset.y + rayLength * 0.8f - (playerCollider.size.y / 2));
        
        Gizmos.DrawWireSphere(rayOrigin, rayLength);

        if (isCrouching)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, Vector2.up * (originalColliderSize.y - playerCollider.size.y));
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label($"State: {gameObject.name}");
        GUILayout.Label($"Grounded: {isGrounded}");
        GUILayout.Label($"Jumps: {jumpCount}/{maxJumpCount}");
        GUILayout.Label($"Speed: {rb.linearVelocity.magnitude:F1}");
        GUILayout.Label($"Rolling: {isRolling}");
        GUILayout.Label($"Roll Cooldown: {Mathf.Max(0, rollCooldown - (Time.time - lastRollTime)):F1}");
        GUILayout.Label($"Crouching: {isCrouching}");
        GUILayout.Label($"Aiming: {isAiming}");
        GUILayout.Label($"Fast Falling: {isFastFalling}");
        GUILayout.Label($"Ammo: {currentAmmo}/{maxAmmo}");
        GUILayout.Label($"Reloading: {isReloading}");
        GUILayout.EndVertical();
    }
}