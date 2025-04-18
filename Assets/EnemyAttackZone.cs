using UnityEngine;

public class EnemyAttackZone : MonoBehaviour
{
    private Collider2D collider;
    private float lifeTime = 0.5f;
    private bool hasDealtDamaged;
    public int damage;
    public LayerMask player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<Collider2D>();
        hasDealtDamaged = false;

        Debug.Log("Damage Area Spawned");
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime < 0)
        {
            Destroy(gameObject);
        }

        if (!hasDealtDamaged )
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.2f, player);

            foreach (Collider2D hitCollider in hitColliders)
            {
                iDamageable damageable = hitCollider.GetComponent<iDamageable>();

                if (damageable != null)
                {
                    damageable.Damage(damage);
                    Destroy(gameObject);
                }
            }
            
        }
    }
}
