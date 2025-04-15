using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    [SerializeField] private bool destroyOnDeath = true;

    [Header("Feedback")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hitSound;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent<float> onHealthChanged; // Passes health percentage (0-1)
    public UnityEvent onHit;

    private int currentHealth;
    private bool isInvulnerable;
    private AudioSource audioSource;

    public int CurrentHealth => currentHealth;
    public float HealthPercentage => (float)currentHealth / maxHealth;

    public bool IsAlive => throw new System.NotImplementedException();

    private void Awake()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        onHealthChanged.Invoke(HealthPercentage);
        onHit.Invoke();

        // Play hit sound if not dying
        if (currentHealth > 0 && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    public void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        onDeath.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        onHealthChanged.Invoke(HealthPercentage);
    }

    public void SetMaxHealth(int newMaxHealth, bool healToFull = false)
    {
        maxHealth = newMaxHealth;
        if (healToFull)
        {
            currentHealth = maxHealth;
        }
        onHealthChanged.Invoke(HealthPercentage);
    }
}