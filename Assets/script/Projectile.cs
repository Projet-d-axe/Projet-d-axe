using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 3f;

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

        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy) enemy.TakeDamage(damage);
        }

        Despawn();
    }

    void Despawn()
    {
        if (pool != null)
            pool.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}
