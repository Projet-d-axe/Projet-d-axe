using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int maxPierceCount = 1;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;
    
    private Rigidbody2D rb;
    private int currentPierceCount; // Maintenant utilisé dans OnTriggerEnter2D

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPierceCount = maxPierceCount;
        Destroy(gameObject, lifetime);
    }

    public void Fire(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Dégâts à l'ennemi
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Système de perçage
            currentPierceCount--;
            if (currentPierceCount <= 0)
            {
                Destroy(gameObject);
            }
        }
        else if (collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    // Optionnel : Pour les collisions avec des objets sans trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}