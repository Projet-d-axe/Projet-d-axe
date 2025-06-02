using UnityEngine;
using System.Collections;

public class FreezeProjectile : MonoBehaviour
{
    [Header("Freeze Settings")]
    public float freezeDuration = 3f; // Durée du gel modifiable dans l'inspector
    public GameObject platformPrefab;
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) return;

        // Vérifier si c'est un ennemi
        EnemyBase enemy = collision.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            // Vérifier si l'ennemi a plus de 1 PV avant d'infliger des dégâts
            if (enemy.enemyData.pv > 1)
            {
                enemy.Damage(damage);
                // Appliquer l'effet de gel normal
                FreezeEffect freezeEffect = enemy.GetComponent<FreezeEffect>();
                if (freezeEffect == null)
                {
                    freezeEffect = enemy.gameObject.AddComponent<FreezeEffect>();
                }
                freezeEffect.Freeze(freezeDuration);
            }
            // Si l'ennemi est à 1 PV, le transformer en plateforme
            else if (enemy.enemyData.pv == 1 && platformPrefab != null)
            {
                StartCoroutine(TransformIntoTemporaryPlatform(enemy));
            }
        }

        // Détruire le projectile
        Destroy(gameObject);
    }

    private IEnumerator TransformIntoTemporaryPlatform(EnemyBase enemy)
    {
        Vector3 enemyPos = enemy.transform.position;
        
        // Désactiver l'ennemi temporairement
        enemy.gameObject.SetActive(false);

        // Créer la plateforme
        GameObject platform = Instantiate(platformPrefab, enemyPos, Quaternion.identity);
        Platform platformScript = platform.GetComponent<Platform>();
        if (platformScript != null)
        {
            platformScript.Initialize(freezeDuration * 2);
        }

        // Attendre la durée du gel
        yield return new WaitForSeconds(freezeDuration * 2);

        // Détruire la plateforme si elle existe encore
        if (platform != null)
        {
            Destroy(platform);
        }

        // Réactiver l'ennemi
        if (enemy != null)
        {
            enemy.gameObject.SetActive(true);
        }
    }
}