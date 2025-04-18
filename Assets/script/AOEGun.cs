using UnityEngine;

public class AOEGun : WeaponSystem
{
    [Header("AOE Settings")]
    public float aoeRadius = 3f;
    public float recoilForce = 5f;
    public LayerMask enemyLayer;
    private Rigidbody2D playerRb;

    protected override void Awake()
    {
        base.Awake();
        playerRb = GetComponentInParent<Rigidbody2D>();
    }

    protected override void Shoot()
    {
        base.Shoot();
        ApplyAOEEffect();
        ApplyRecoil();
    }

    private void ApplyAOEEffect()
    {
        Vector2 origin = firePoint.position;
        Vector2 shootDirection = GetShootDirection();

        // Trouve tous les ennemis dans le rayon
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, aoeRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                // Vérifie si l'ennemi est dans le cône de tir (angle large)
                Vector2 toEnemy = (hit.transform.position - firePoint.position).normalized;
                float angle = Vector2.Angle(shootDirection, toEnemy);

                if (angle <= 60f) // 120° de spectre total (±60°)
                {
                    EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                    if (enemy) enemy.TakeDamage(damage);
                }
            }
        }
    }

    private void ApplyRecoil()
    {
        if (playerRb && recoilForce > 0)
        {
            Vector2 direction = -GetShootDirection();
            playerRb.AddForce(direction * recoilForce, ForceMode2D.Impulse);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (firePoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, aoeRadius);
        }
    }
}
