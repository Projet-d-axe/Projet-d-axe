using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private int damage;
    private Vector2 direction;

    public void Initialize(Vector2 dir, int dmg)
    {
        direction = dir;
        damage = dmg;
        Destroy(gameObject, 3f); // Auto-destroy after 3 seconds
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<IDamageable>()?.TakeDamage(damage); // Ensure the object implements IDamageable
            Destroy(gameObject);
        }
    }
}

internal interface IDamageable
{
    void TakeDamage(int damage);
}