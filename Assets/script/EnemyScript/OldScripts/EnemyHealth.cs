using UnityEngine;

public class EnemyHealth : MonoBehaviour, iDamageable
{
    public int maxHealth = 3;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"[EnemyHealth] Enemy initialized with {currentHealth} HP");
    }

    public void Damage(int amount)
    {
        Debug.Log($"[EnemyHealth] Enemy taking {amount} damage. Current health: {currentHealth}");
        currentHealth -= amount;
        Debug.Log($"[EnemyHealth] Health after damage: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Debug.Log("[EnemyHealth] Enemy health reached 0, calling Die()");
            Die();  
        }
    }

    private void Die()
    {
        Debug.Log("[EnemyHealth] Enemy destroyed");
        Destroy(gameObject);
    }
}
