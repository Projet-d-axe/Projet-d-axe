using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();  
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
