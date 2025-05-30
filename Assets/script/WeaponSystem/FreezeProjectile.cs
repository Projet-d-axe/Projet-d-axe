using UnityEngine;

public class FreezeProjectile : MonoBehaviour
{
    [Header("Freeze Settings")]
    public float freezeDuration = 3f; // Durée du gel modifiable dans l'inspector
    public GameObject platformPrefab;
    public int damage = 1;

    private ObjectPool pool;

    public void SetPool(ObjectPool poolRef)
    {
        pool = poolRef;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) return;

        // Vérifier si c'est un ennemi
        EnemyBase enemy = collision.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            // Obtenir ou ajouter le composant FreezeEffect
            FreezeEffect freezeEffect = enemy.GetComponent<FreezeEffect>();
            if (freezeEffect == null)
            {
                freezeEffect = enemy.gameObject.AddComponent<FreezeEffect>();
            }

            // Appliquer l'effet de gel
            freezeEffect.Freeze(freezeDuration);

            // Infliger des dégâts
            enemy.Damage(damage);

            // Si l'ennemi est à 1 PV ou moins après les dégâts, le transformer en plateforme
            if (enemy.enemyData.pv <= 1 && platformPrefab != null)
            {
                Vector3 enemyPos = enemy.transform.position;
                Destroy(enemy.gameObject);
                GameObject platform = Instantiate(platformPrefab, enemyPos, Quaternion.identity);
                Platform platformScript = platform.GetComponent<Platform>();
                if (platformScript != null)
                {
                    platformScript.Initialize(freezeDuration);
                }
            }
        }

        // Détruire le projectile
        if (pool != null)
            pool.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}