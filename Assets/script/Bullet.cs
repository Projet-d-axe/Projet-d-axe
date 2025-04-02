using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;
    public int damage = 1;
    public float lifetime = 3f;
    public LayerMask collisionLayers;
    public GameObject impactEffect;
    public bool destroyOnImpact = true;

    [Header("Advanced")]
    public bool piercing = false;
    public int maxPierceCount = 3;
    private int currentPierceCount = 0;
    public string[] validTags = { "Enemy", "Boss", "Destructible" };

    private Rigidbody2D rb;
    private Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime); // Auto-destruction après la durée de vie
    }

    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        rb.linearVelocity = direction * speed;
        
        // Orienter le projectile dans la direction du tir
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifications de sécurité
        if (other == null) return;
        if (other.isTrigger) return;
        if (!IsInLayerMask(other.gameObject.layer, collisionLayers)) return;

        // Vérification des tags valides
        bool validCollision = false;
        foreach (string tag in validTags)
        {
            if (other.CompareTag(tag))
            {
                validCollision = true;
                break;
            }
        }

        if (!validCollision) return;
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    // Pour le débogage
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * 0.5f);
    }
}