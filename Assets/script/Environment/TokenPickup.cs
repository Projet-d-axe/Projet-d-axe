using UnityEngine;

public class TokenPickup : MonoBehaviour
{
    public int tokenValue = 1;
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TokenSystem tokenSystem = other.GetComponent<TokenSystem>();
            if (tokenSystem != null)
            {
                tokenSystem.AddToken(tokenValue);
                
                if (pickupEffect != null)
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                
                if (pickupSound != null)
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                
                Destroy(gameObject);
            }
        }
    }
}