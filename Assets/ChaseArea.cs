using UnityEngine;

public class ChaseArea : MonoBehaviour
{
    public CircleCollider2D CC2D;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CC2D = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            gameObject.GetComponentInParent<EnemyBase>().chase = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.GetComponentInParent<EnemyBase>().chase = false;
        }
    }

    public void SetRadiusToDetectionRange(float range)
    {
        CC2D.radius = range;
    }
}
