using UnityEngine;
using System.Collections;

public class WeaponSystem : MonoBehaviour
{
    public enum WeaponType { Standard, AOE, Platform, Laser }
    
    [Header("Weapon Type")]
    public WeaponType weaponType = WeaponType.Standard;

    [Header("UI")]
    public string weaponName = "Weapon";
    public Sprite weaponIcon;

    [Header("Base Settings")]
    public Transform firePoint;
    public float fireRate = 0.2f;
    public int maxAmmo = 10;
    public int reserveAmmo = 30;
    public float reloadTime = 1f;
    public bool autoReload = true;
    public bool lockMovementWhenAiming;
    public float aimingMoveSpeedMultiplier = 0.5f;
    
    [Header("Infinite Ammo")]
    public bool infiniteAmmo = false;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;
    public int damage = 1;
    public float projectileLifetime = 3f;

    [Header("AOE Settings")]
    public float aoeRadius = 3f;
    public float recoilForce = 5f;
    public LayerMask enemyLayer;

    [Header("Platform Settings")]
    public GameObject platformPrefab;
    public float platformDuration = 10f;

    [Header("Laser Settings")]
    public float laserRange = 20f;
    public float laserDuration = 0.1f;
    public LineRenderer laserLine;
    public LayerMask crystalLayer;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    [Range(0,1)] public float volume = 0.7f;

    // État interne
    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;
    private AudioSource audioSource;
    private Rigidbody2D playerRb;

    void Awake()
    {
        InitializeComponents();
        ConfigureWeaponType();
    }

    void Start()
    {
        currentAmmo = maxAmmo;
        ValidateWeaponSetup();
    }

    void InitializeComponents()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.volume = volume;
        }

        playerRb = GetComponentInParent<Rigidbody2D>();
    }

    void ValidateWeaponSetup()
    {
        if (projectilePrefab == null && weaponType != WeaponType.Laser)
        {
            Debug.LogError($"[{weaponName}] Projectile Prefab manquant !");
        }

        if (firePoint == null)
        {
            Debug.LogError($"[{weaponName}] Fire Point manquant !");
        }

        if (weaponType == WeaponType.Platform && platformPrefab == null)
        {
            Debug.LogWarning($"[{weaponName}] Platform Prefab manquant !");
        }

        if (weaponType == WeaponType.Laser && laserLine == null)
        {
            Debug.LogWarning($"[{weaponName}] Laser Line manquant !");
        }
    }

    void ConfigureWeaponType()
    {
        if (weaponType == WeaponType.Laser && laserLine == null)
        {
            laserLine = gameObject.AddComponent<LineRenderer>();
            laserLine.startWidth = 0.1f;
            laserLine.endWidth = 0.1f;
            laserLine.material = new Material(Shader.Find("Sprites/Default"));
            laserLine.startColor = Color.red;
            laserLine.endColor = Color.red;
        }
    }

    public void TryShoot()
    {
        if (!CanShoot()) return;
        Shoot();
    }

    bool CanShoot()
    {
        if (Time.time < nextFireTime || isReloading) return false;
        if (infiniteAmmo) return true;
        if (currentAmmo <= 0)
        {
            if (autoReload && reserveAmmo > 0) Reload();
            return false;
        }
        return true;
    }

    void Shoot()
    {
        ExecuteShoot();
        UpdateAmmo();
        PlayShootEffects();
        HandleWeaponSpecificEffects();
        
        if (ShouldReload()) Reload();
    }

    void ExecuteShoot()
    {
        switch (weaponType)
        {
            case WeaponType.Laser:
                FireLaser();
                break;
            default:
                FireProjectile();
                break;
        }
    }

    void UpdateAmmo()
    {
        if (!infiniteAmmo) currentAmmo--;
        nextFireTime = Time.time + fireRate;
    }

    bool ShouldReload()
    {
        return !infiniteAmmo && currentAmmo <= 0 && reserveAmmo > 0 && autoReload;
    }

    void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        ConfigureProjectile(projectile);
        Destroy(projectile, projectileLifetime);
    }

    void ConfigureProjectile(GameObject projectile)
    {
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = GetShootDirection() * projectileSpeed;

        Bullet bullet = projectile.GetComponent<Bullet>();
        if (bullet) bullet.damage = damage;

        if (weaponType == WeaponType.Platform)
        {
            PlatformProjectile platform = projectile.GetComponent<PlatformProjectile>();
            if (platform)
            {
                platform.platformPrefab = platformPrefab;
                platform.lifeTime = platformDuration;
            }
        }

        IgnorePlayerCollision(projectile);
    }

    void FireLaser()
    {
        Vector2 direction = GetShootDirection();
        Vector2 endPoint = (Vector2)firePoint.position + direction * laserRange;

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, laserRange, crystalLayer);
        if (hit.collider != null)
        {
            endPoint = hit.point;
            Crystal crystal = hit.collider.GetComponent<Crystal>();
            if (crystal) crystal.Activate();
        }

        StartCoroutine(DrawLaser(firePoint.position, endPoint));
    }

    IEnumerator DrawLaser(Vector3 start, Vector3 end)
    {
        laserLine.enabled = true;
        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);

        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }

    void HandleWeaponSpecificEffects()
    {
        if (weaponType == WeaponType.AOE) ApplyAOEEffect();
        if (recoilForce > 0) ApplyRecoil();
    }

    void ApplyAOEEffect()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(firePoint.position, aoeRadius, enemyLayer);
        Vector2 shootDirection = GetShootDirection();

        foreach (Collider2D hit in hits)
        {
            iDamageable damageable = hit.GetComponent<iDamageable>();
            if (damageable != null && Vector2.Angle(shootDirection, (hit.transform.position - firePoint.position).normalized) <= 60f)
            {
                damageable.Damage(damage);
            }
        }
    }

    void ApplyRecoil()
    {
        if (playerRb == null || recoilForce <= 0) return;
        
        Vector2 recoilDirection = -GetShootDirection() * recoilForce;
        playerRb.AddForce(recoilDirection, ForceMode2D.Impulse);
        
        CameraController cameraController = Camera.main?.GetComponent<CameraController>();
        cameraController?.Shake(0.1f);
    }

    Vector2 GetShootDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mousePos - firePoint.position).normalized;
    }

    void IgnorePlayerCollision(GameObject projectile)
    {
        Collider2D projCol = projectile.GetComponent<Collider2D>();
        Collider2D playerCol = GetComponentInParent<Collider2D>();
        if (projCol && playerCol) Physics2D.IgnoreCollision(projCol, playerCol);
    }

    void PlayShootEffects()
    {
        muzzleFlash?.Play();
        if (shootSound) audioSource.PlayOneShot(shootSound, volume);
    }

    public void Reload()
    {
        if (infiniteAmmo || isReloading || currentAmmo == maxAmmo || reserveAmmo <= 0) return;
        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        if (reloadSound) audioSource.PlayOneShot(reloadSound, volume);

        yield return new WaitForSeconds(reloadTime);

        int bulletsToLoad = Mathf.Min(maxAmmo - currentAmmo, reserveAmmo);
        currentAmmo += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;

        isReloading = false;
    }

    public int GetCurrentAmmo() => infiniteAmmo ? maxAmmo : currentAmmo;
    public int GetReserveAmmo() => infiniteAmmo ? 999 : reserveAmmo;
    public bool IsReloading() => isReloading;

    void OnDrawGizmosSelected()
    {
        if (!firePoint) return;

        Gizmos.color = weaponType == WeaponType.AOE ? Color.red : Color.blue;
        
        if (weaponType == WeaponType.AOE)
        {
            Gizmos.DrawWireSphere(firePoint.position, aoeRadius);
        }
        else if (weaponType == WeaponType.Laser)
        {
            Gizmos.DrawLine(firePoint.position, firePoint.position + (Vector3)GetShootDirection() * laserRange);
        }
    }
}