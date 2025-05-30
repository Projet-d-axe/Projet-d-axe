using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "ScriptableObjects/EnemyConfig", order = 1)]
public class EnemyConfigSO : ScriptableObject
{
    public BaseStats baseStats;
    public Material frozenMaterial;
    public Material platformMaterial;
}

[System.Serializable]
public class BaseStats
{
    public float maxHealth;
    public bool isFrozen;
    public bool canBePlatform;
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyConfigSO config;
    
    private float currentHealth;
    private bool isPlatform = false;
    private Material originalMaterial;
    private Renderer enemyRenderer;
    private Rigidbody enemyRigidbody;

    public bool IsPlatform { get; internal set; }

    private void Awake()
    {
        currentHealth = config.baseStats.maxHealth;
        enemyRenderer = GetComponent<Renderer>();
        originalMaterial = enemyRenderer.material;
        enemyRigidbody = GetComponent<Rigidbody>();
    }
    
    public void TakeDamage(float damage)
    {
        if (isPlatform) return;
        
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Freeze(float duration)
    {
        if (isPlatform) return;
        
        StartCoroutine(FreezeRoutine(duration));
    }
    
    public void ConvertToPlatform(float duration)
    {
        if (isPlatform) return;
        
        StartCoroutine(PlatformRoutine(duration));
    }
    
    private IEnumerator FreezeRoutine(float duration)
    {
        config.baseStats.isFrozen = true;
        enemyRenderer.material = config.frozenMaterial;
        enemyRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        
        yield return new WaitForSeconds(duration);
        
        config.baseStats.isFrozen = false;
        enemyRenderer.material = originalMaterial;
        enemyRigidbody.constraints = RigidbodyConstraints.None;
    }
    
    private IEnumerator PlatformRoutine(float duration)
    {
        isPlatform = true;
        config.baseStats.canBePlatform = true;
        enemyRenderer.material = config.platformMaterial;
        enemyRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        
        // Change layer to "Platform" so player can jump on it
        gameObject.layer = LayerMask.NameToLayer("Platform");
        
        yield return new WaitForSeconds(duration);
        
        isPlatform = false;
        config.baseStats.canBePlatform = false;
        enemyRenderer.material = originalMaterial;
        enemyRigidbody.constraints = RigidbodyConstraints.None;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
    
    private void Die()
    {
        // Death effects, scoring, etc.
        Destroy(gameObject);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}