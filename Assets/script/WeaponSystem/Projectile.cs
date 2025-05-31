using UnityEngine;

public class Bullet : MonoBehaviour, iDamageable
{
    public int damage = 1;
    public float lifeTime = 3f;

    public void Damage(int amount)
    {
        Despawn();
    }

    private float timer;

    void OnEnable()
    {
        timer = lifeTime;
        Debug.Log("[Bullet] Bullet spawned");
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            Despawn();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[Bullet] Collision detected with {collision.gameObject.name}");
        
        if (collision.isTrigger)
        {
            Debug.Log("[Bullet] Ignoring trigger collision");
            return;
        }

        iDamageable damageable = collision.GetComponent<iDamageable>();
        if (damageable != null)
        {
            Debug.Log($"[Bullet] Found iDamageable component, applying {damage} damage");
            damageable.Damage(damage);
            Despawn();
        }
        else
        {
            Debug.Log("[Bullet] No iDamageable component found on collision target");
        }
    }

    void Despawn()
    {
        Debug.Log("[Bullet] Bullet despawned");
        Destroy(gameObject);
    }
}