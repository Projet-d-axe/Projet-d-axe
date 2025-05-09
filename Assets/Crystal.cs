using UnityEngine;

public class Crystal : MonoBehaviour
{
    [Header("Settings")]
    public float fallSpeed = 5f;
    public float destroyDelay = 2f;
    public GameObject destructionEffect;
    
    private Rigidbody2D rb;
    private bool isFalling = false;
    private Vector2 originalPosition;
    private Collider2D crystalCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        crystalCollider = GetComponent<Collider2D>();
        originalPosition = transform.position;
    }

    public void DestroyCrystal()
    {
        if (isFalling) return;
        
        isFalling = true;
        rb.isKinematic = false;
        rb.linearVelocity = Vector2.down * fallSpeed;
        
        // Désactive le collider après un petit delai
        Invoke("DisableCollider", 0.1f);
        
        // Destruction après
        Destroy(gameObject, destroyDelay);
        
        // Effet visuel
        if (destructionEffect)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }
    }

    private void DisableCollider()
    {
        crystalCollider.enabled = false;
    }
}