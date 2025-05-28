using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class HealthSystem : MonoBehaviour, iDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isInvulnerable = false;
    public float invulnerabilityDuration = 1f;
    public bool showDamageFlash = true;

    [Header("References")]
    public HealthBar healthBar;
    public SpriteRenderer spriteRenderer;

    [Header("Effects")]
    public Color damageFlashColor = Color.red;
    public float flashDuration = 0.2f;
    public GameObject deathEffect;
    public AudioClip hitSound;
    public AudioClip deathSound;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent<float> onDamaged;
    public UnityEvent<float> onHealed;

    private Color originalColor;
    private bool isFlashing = false;
    private AudioSource audioSource;

    private void Start()
    {
        currentHealth = maxHealth;
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
            
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (hitSound != null || deathSound != null))
            audioSource = gameObject.AddComponent<AudioSource>();

        UpdateHealthBar();
    }

    public void Damage(int amount)
    {
        if (isInvulnerable || currentHealth <= 0) return;

        currentHealth -= amount;
        onDamaged?.Invoke(amount);

        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);

        if (showDamageFlash && spriteRenderer != null && !isFlashing)
            StartCoroutine(FlashRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (invulnerabilityDuration > 0)
        {
            StartCoroutine(InvulnerabilityRoutine());
        }

        UpdateHealthBar();
    }

    public void Heal(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onHealed?.Invoke(amount);
        
        UpdateHealthBar();
    }

    private void Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);

        onDeath?.Invoke();
        
        // Si c'est un ennemi, on le détruit
        if (gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        // Si c'est le joueur, on peut implémenter une logique de game over
        else if (gameObject.CompareTag("Player"))
        {
            // Implémenter la logique de game over ici
            isInvulnerable = true;
        }
    }

    private IEnumerator FlashRoutine()
    {
        isFlashing = true;
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        isFlashing = false;
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }
} 