using UnityEngine;

public class Platform : MonoBehaviour
{
    private float duration;

    public void Initialize(float lifeTime)
    {
        duration = lifeTime;
        Destroy(gameObject, duration);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}