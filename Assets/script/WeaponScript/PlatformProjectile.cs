using UnityEngine;

public class PlatformProjectile : MonoBehaviour
{
    public GameObject platformPrefab;
    public float lifeTime = 15f;

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
            Vector3 enemyPos = collision.transform.position;
            Destroy(collision.gameObject);
            Instantiate(platformPrefab, enemyPos, Quaternion.identity);
            Despawn();
            return;
        }

        Despawn(); // aucune plateforme si ce n’est pas un ennemi
    }

    void Despawn()
    {
        if (pool != null)
            pool.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}
