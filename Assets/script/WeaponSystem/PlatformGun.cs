using UnityEngine;

public class PlatformGun : WeaponSystem
{
    [Header("Platform Settings")]
    public GameObject platformPrefab;
    public float platformDuration = 10f;

    protected override void Awake()
    {
        base.Awake();
        infiniteAmmo = true; // Tir illimité
        autoReload = false;
    }

    protected override void FireProjectile()
    {
        if (!projectilePrefab || !platformPrefab) return;

        GameObject projectile = null;

        // Utilise le pooling s’il est disponible
        if (projectilePool)
        {
            projectile = projectilePool.Get(firePoint.position, firePoint.rotation);
        }
        else
        {
            projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }

        if (projectile == null) return;

        // Applique la vitesse
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = GetShootDirection() * projectileSpeed;

        // Configure le projectile
        PlatformProjectile platformProj = projectile.GetComponent<PlatformProjectile>();
        if (platformProj)
        {
            platformProj.platformPrefab = platformPrefab;
            platformProj.lifeTime = platformDuration;

            if (projectilePool != null)
                platformProj.SetPool(projectilePool);
        }

        IgnorePlayerCollision(projectile);
    }
}
