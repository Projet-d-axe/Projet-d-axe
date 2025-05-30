using UnityEngine;

public class FreezeProjectile : Bullet
{
    [Header("Freeze Settings")]
    public float freezeDuration = 3f; // Durée du gel modifiable dans l'inspector

    protected override void OnTriggerEnter2D(Collider2D collision)
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
        }

        // Détruire le projectile
        Despawn();
    }
} 