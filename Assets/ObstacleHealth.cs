using UnityEngine;

public class ObstacleHealth : MonoBehaviour, iDamageable
{
    [SerializeField] private float health;

    public void Damage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
