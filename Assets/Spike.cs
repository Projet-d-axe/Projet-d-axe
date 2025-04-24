using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private LayerMask player;
    [SerializeField] private Vector2 knockbackEffect;
    [SerializeField] private int knockbackForce;
    private new CircleCollider2D collider2D;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider2D = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        iDamageable damageable = collision.gameObject.GetComponent<iDamageable>();
        
        if (damageable != null)
             damageable.Damage(damage);
        collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(knockbackEffect.x * knockbackForce, knockbackEffect.y * knockbackForce);
    }
}
