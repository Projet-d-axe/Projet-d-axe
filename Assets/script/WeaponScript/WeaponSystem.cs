using UnityEngine;
using System.Collections;

public class WeaponSystem : MonoBehaviour
{

    [Header("UI")]
    public string weaponName = "Weapon";
    
    [Header("Base Settings")]
    public Transform firePoint;
    public float fireRate = 0.2f;
    public int maxAmmo = 10;
    public int reserveAmmo = 30;
    public float reloadTime = 1f;
    public bool autoReload = true;
    public bool infiniteAmmo = false; // <-- Ajouté
    public bool lockMovementWhenAiming { get; set; }
    public float aimingMoveSpeedMultiplier = 0.5f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public ObjectPool projectilePool;
    public float projectileSpeed = 10f;
    public int damage = 1;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    protected int currentAmmo;
    protected float nextFireTime;
    protected bool isReloading;
    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.volume = 0.7f;
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
        FireProjectile();
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

        projectile.GetComponent<Bullet>()?.SetPool(projectilePool);
        projectile.GetComponent<PlatformProjectile>()?.SetPool(projectilePool);

        IgnorePlayerCollision(projectile);
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
}
