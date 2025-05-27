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
    private ObjectPool pool;

    public void SetPool(ObjectPool poolRef)
    {
        pool = poolRef;
    }

    void OnEnable()
    {
        timer = lifeTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            Despawn();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) return;

        
        iDamageable damageable = collision.GetComponent<iDamageable>();
        if (damageable != null)
        {
            damageable.Damage(damage);
            Despawn();
        }
    }

    void Despawn()
    {
        if (pool != null)
            pool.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}