using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool isInvulnerable = false;
    [SerializeField] private float invulnerabilityDuration = 0.5f;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float hitFlashDuration = 0.1f;
    [SerializeField] private Color hitFlashColor = Color.red;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent onDamageTaken;
    public UnityEvent onHealthChanged;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private AudioSource audioSource;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Appeler les événements
        onHealthChanged.Invoke();
        onDamageTaken.Invoke();

        // Feedback visuel
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }

        // Vérifier la mort
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Invulnérabilité temporaire après un coup
            StartCoroutine(TriggerInvulnerability());
        }
    }

    private IEnumerator FlashEffect()
    {
        spriteRenderer.color = hitFlashColor;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }

    private IEnumerator TriggerInvulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onHealthChanged.Invoke();
    }

    public void Die()
    {
        // Effet de mort
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Son de mort
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Événement de mort
        onDeath.Invoke();

        // Désactiver ou détruire l'ennemi
        Destroy(gameObject);
        // OU pour désactiver au lieu de détruire :
        // gameObject.SetActive(false);
    }

    public void SetInvulnerable(bool state)
    {
        isInvulnerable = state;
    }

    // Pour l'UI ou le debug
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}