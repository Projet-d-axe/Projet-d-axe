using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private GameObject impactEffect;
    
    private Vector2 direction;
    private float speed;
    private string shooterTag;

    public void Initialize(Vector2 dir, float spd, string tag)
    {
        direction = dir;
        speed = spd;
        shooterTag = tag;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(shooterTag)) return;
        
        if (impactEffect) Instantiate(impactEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}