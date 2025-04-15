using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;
    private Weapon.BulletType bulletType;
    private Rigidbody2D rb;
    private int pierceCount = 0;
    private const int maxPierce = 2;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetType(Weapon.BulletType type)
    {
        bulletType = type;
        rb.linearVelocity = transform.right * speed;
        Invoke("DisableBullet", lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null)
        {
            switch (bulletType)
            {
                case Weapon.BulletType.Normal:
                    enemy.OnDeath();
                    DisableBullet();
                    break;

                case Weapon.BulletType.Platform:
                    enemy.GetComponent<Rigidbody2D>().linearVelocity *= 0.3f;
                    enemy.transform.localScale *= 1.5f;
                    DisableBullet();
                    break;

                case Weapon.BulletType.Piercing:
                    enemy.OnDeath();
                    pierceCount++;
                    if (pierceCount >= maxPierce) DisableBullet();
                    break;
            }
        }
    }

    public void DisableBullet()
    {
        CancelInvoke();
        Weapon weapon = FindObjectOfType<Weapon>();
        if (weapon != null) weapon.ReturnBulletToPool(gameObject, bulletType);
    }
}