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
    public bool infiniteAmmo = false;
    public bool lockMovementWhenAiming { get; set; }
    public float aimingMoveSpeedMultiplier = 0.5f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public ObjectPool projectilePool;
    public float projectileSpeed = 10f;
    public int damage = 1;

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

    protected int currentAmmo;
    protected float nextFireTime;
    protected bool isReloading;
    protected AudioSource audioSource;
    protected Rigidbody2D playerRb;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.volume = 0.7f;
        }

        playerRb = GetComponentInParent<Rigidbody2D>();

        // Configuration spécifique au type d'arme
        switch (weaponType)
        {
            case WeaponType.Platform:
                infiniteAmmo = true;
                autoReload = false;
                break;
            case WeaponType.Laser:
                infiniteAmmo = true;
                if (laserLine == null)
                {
                    laserLine = gameObject.AddComponent<LineRenderer>();
                    laserLine.startWidth = 0.1f;
                    laserLine.endWidth = 0.1f;
                    laserLine.material = new Material(Shader.Find("Sprites/Default"));
                    laserLine.startColor = Color.red;
                    laserLine.endColor = Color.red;
                }
                break;
        }
    }

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    public virtual void TryShoot()
    {
        if (Time.time < nextFireTime || isReloading) return;

        if (!infiniteAmmo && currentAmmo <= 0)
        {
            if (autoReload && reserveAmmo > 0)
                Reload();
            return;
        }

        Shoot();
    }

    protected virtual void Shoot()
    {
        if (!infiniteAmmo)
        {
            currentAmmo--;
            if (currentAmmo <= 0 && reserveAmmo > 0 && autoReload)
                Reload();
        }

        nextFireTime = Time.time + fireRate;
        PlayShootEffects();

        switch (weaponType)
        {
            case WeaponType.Laser:
                FireLaser();
                break;
            default:
                FireProjectile();
                break;
        }

        // Effets spécifiques au type d'arme
        if (weaponType == WeaponType.AOE)
        {
            ApplyAOEEffect();
            ApplyRecoil();
        }
    }

    protected virtual void FireProjectile()
    {
        GameObject projectile = null;

        if (projectilePool)
        {
            projectile = projectilePool.Get(firePoint.position, firePoint.rotation);
        }
        else if (projectilePrefab)
        {
            projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }

        if (projectile == null) return;

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = GetShootDirection() * projectileSpeed;

        // Configuration spécifique au projectile
        if (weaponType == WeaponType.Platform && projectile.TryGetComponent(out PlatformProjectile platformProj))
        {
            platformProj.platformPrefab = platformPrefab;
            platformProj.lifeTime = platformDuration;
            if (projectilePool != null)
                platformProj.SetPool(projectilePool);
        }
        else if (projectile.TryGetComponent(out Bullet bullet))
        {
            bullet.damage = damage;
            if (projectilePool != null)
                bullet.SetPool(projectilePool);
        }

        IgnorePlayerCollision(projectile);
    }

    protected void FireLaser()
    {
        Vector2 direction = GetShootDirection();
        Vector2 endPoint = (Vector2)firePoint.position + direction * laserRange;

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, laserRange, crystalLayer);

        if (hit.collider != null)
        {
            endPoint = hit.point;
            if (hit.collider.CompareTag("Crystal"))
            {
                Crystal crystal = hit.collider.GetComponent<Crystal>();
                if (crystal != null)
                {
                    crystal.Activate(); 
                }
            }
        }
        StartCoroutine(DrawLaser(firePoint.position, endPoint));
    }

    private IEnumerator DrawLaser(Vector3 start, Vector3 end)
    {
        laserLine.enabled = true;
        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);

        yield return new WaitForSeconds(laserDuration);

        laserLine.enabled = false;
    }

    private void ApplyAOEEffect()
    {
        if (weaponType != WeaponType.AOE) return;

        Vector2 origin = firePoint.position;
        Vector2 shootDirection = GetShootDirection();

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, aoeRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            iDamageable damageable = hit.GetComponent<iDamageable>();
            if (damageable != null)
            {
                Vector2 toEnemy = (hit.transform.position - firePoint.position).normalized;
                float angle = Vector2.Angle(shootDirection, toEnemy);

                if (angle <= 60f)
                {
                    damageable.Damage(damage);
                }
            }
        }
    }

    private void ApplyRecoil()
    {
        if (weaponType != WeaponType.AOE || playerRb == null || recoilForce <= 0) return;

        Vector2 direction = -GetShootDirection();
        playerRb.AddForce(direction * recoilForce, ForceMode2D.Impulse);
        if (Camera.main != null && Camera.main.TryGetComponent(out CameraShake cameraShake))
        {
            cameraShake.Shake(0.01f, 0.02f);
        }
    }

    protected Vector2 GetShootDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mousePos - firePoint.position).normalized;
    }

    protected void IgnorePlayerCollision(GameObject projectile)
    {
        Collider2D projCol = projectile.GetComponent<Collider2D>();
        Collider2D playerCol = GetComponentInParent<Collider2D>();
        if (projCol && playerCol) Physics2D.IgnoreCollision(projCol, playerCol);
    }

    protected void PlayShootEffects()
    {
        if (muzzleFlash) muzzleFlash.Play();
        if (shootSound) audioSource.PlayOneShot(shootSound);
    }

    public void Reload()
    {
        if (infiniteAmmo || isReloading || currentAmmo == maxAmmo || reserveAmmo <= 0) return;
        StartCoroutine(ReloadRoutine());
    }

    protected virtual IEnumerator ReloadRoutine()
    {
        isReloading = true;
        if (reloadSound) audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        int bulletsNeeded = maxAmmo - currentAmmo;
        int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);

        currentAmmo += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;

        isReloading = false;
    }

    public int GetCurrentAmmo() => infiniteAmmo ? 999 : currentAmmo;
    public int GetReserveAmmo() => infiniteAmmo ? -1 : reserveAmmo;
    public bool IsReloading() => isReloading;

    void OnDrawGizmosSelected()
    {
        if (firePoint)
        {
            if (weaponType == WeaponType.AOE)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(firePoint.position, aoeRadius);
            }
            else if (weaponType == WeaponType.Laser)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(firePoint.position, firePoint.position + (Vector3)GetShootDirection() * laserRange);
            }
        }
    }
}