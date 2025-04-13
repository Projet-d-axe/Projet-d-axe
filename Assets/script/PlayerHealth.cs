using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Système complet de santé du joueur avec gestion UI/effets
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    // Configuration de base
    [Header("Core Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invincibilityDuration = 0.5f;
    private int currentHealth;
    private float lastDamageTime;
    private bool isDead;

    // Références UI
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image damageVignette;
    [SerializeField] private Text healthText;

    // Effets
    [Header("VFX/SFX")]
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] hurtSounds;
    [SerializeField] private AudioClip deathSound;

    // Événements
    [System.Serializable]
    public class HealthEvent : UnityEvent<int, int> {} // Current, Max
    public HealthEvent OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent OnDamageTaken;

    // Propriétés d'accès
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsInvincible => Time.time < lastDamageTime + invincibilityDuration;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    /// <summary>
    /// Applique des dégâts au joueur avec gestion d'invincibilité
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead || IsInvincible) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        lastDamageTime = Time.time;

        // Feedback
        PlayHurtEffects();
        UpdateHealthUI();
        OnDamageTaken?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0) Die();
    }

    /// <summary>
    /// Soigne le joueur
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthUI();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Réinitialise complètement la santé
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        UpdateHealthUI();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke();
        
        // Effets
        if (deathEffectPrefab) Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        if (deathSound) audioSource.PlayOneShot(deathSound);
    }

    // Méthodes d'effets visuels
    private void PlayHurtEffects()
    {
        // Son aléatoire
        if (hurtSounds.Length > 0)
        {
            audioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
        }

        // Vignette rouge
        if (damageVignette)
        {
            damageVignette.color = new Color(1, 0, 0, 0.5f);
            LeanTween.alpha(damageVignette.rectTransform, 0f, 0.5f).setEase(LeanTweenType.easeOutCubic);
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider) healthSlider.value = (float)currentHealth / maxHealth;
        if (healthText) healthText.text = $"{currentHealth}/{maxHealth}";
    }

    /// <summary>
    /// Modifie la santé maximale (pour power-ups)
    /// </summary>
    public void SetMaxHealth(int newMax, bool healToFull = true)
    {
        maxHealth = newMax;
        if (healToFull) currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        UpdateHealthUI();
    }
}