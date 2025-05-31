using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Système complet de santé du joueur avec gestion UI/effets
/// </summary>
public class PlayerHealth : MonoBehaviour, iDamageable
{
    // Configuration de base
    [Header("Core Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invincibilityDuration = 0.5f;
    [SerializeField] private float deathDelay = 1f;

    private bool isInvincible;
    private int currentHealth;
    private float lastDamageTime;
    private bool isDead;

    // Références UI
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image damageVignette;
    [SerializeField] private Text healthText;
    [SerializeField] private HealthBar healthBar;

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

    // Composants
    private Collider2D playerCollider;
    private Rigidbody2D playerRigidbody;
    private PlayerController playerController;
    private Vector3 lastPosition;

    // Propriétés d'accès
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsInvincible => Time.time < lastDamageTime + invincibilityDuration;

    private void Awake()
    {
        // Récupérer les références aux composants
        playerCollider = GetComponent<Collider2D>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();

        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        UpdateHealthUI();
    }

    /// <summary>
    /// Applique des dégâts au joueur avec gestion d'invincibilité
    /// </summary>
    public void TakeDamage(int damage)
    {
        Debug.Log($"[PlayerHealth] TakeDamage appelé avec {damage} dégâts");
        
        if (isDead)
        {
            Debug.Log("[PlayerHealth] Déjà mort, dégâts ignorés");
            return;
        }
        
        if (IsInvincible)
        {
            Debug.Log("[PlayerHealth] Invincible, dégâts ignorés");
            return;
        }

        Debug.Log($"[PlayerHealth] Vie avant dégâts: {currentHealth}");
        lastPosition = transform.position;
        currentHealth = Mathf.Max(0, currentHealth - damage);
        lastDamageTime = Time.time;

        // Feedback
        PlayHurtEffects();
        UpdateHealthUI();
        OnDamageTaken?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("[PlayerHealth] Vie à 0, appel de Die()");
            Die();
        }
    }

    /// <summary>
    /// Soigne le joueur
    /// </summary>
    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthUI();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Réinitialise complètement la santé et l'état du joueur
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        lastDamageTime = 0f;
        isInvincible = false;
        
        UpdateHealthUI();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        Debug.Log("[PlayerHealth] Health reset to full");
    }

    private void Die()
    {
        if (isDead) return;
        
        Debug.Log("[PlayerHealth] Début de la séquence de mort");
        isDead = true;
        
        // Désactiver les composants du joueur
        if (playerController != null)
            playerController.enabled = false;
        
        if (playerRigidbody != null)
        {
            playerRigidbody.simulated = false;
            playerRigidbody.linearVelocity = Vector2.zero;
        }
        
        if (playerCollider != null)
            playerCollider.enabled = false;

        // Effets
        if (deathEffectPrefab) 
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        
        if (deathSound && audioSource) 
            audioSource.PlayOneShot(deathSound);

        // Sauvegarder la position de mort
        lastPosition = transform.position;

        // Vérifier si le CheckpointManager existe
        if (CheckpointManager.Instance == null)
        {
            Debug.LogError("[PlayerHealth] ERREUR CRITIQUE: CheckpointManager.Instance est null!");
            return;
        }

        Debug.Log("[PlayerHealth] CheckpointManager trouvé, appel du respawn");
        
        // Appeler le respawn après un court délai
        Invoke("CallRespawn", deathDelay);
    }

    private void CallRespawn()
    {
        if (CheckpointManager.Instance != null)
        {
            Debug.Log("[PlayerHealth] Appel effectif du respawn");
            CheckpointManager.Instance.RespawnPlayer();
        }
    }

    // Méthodes d'effets visuels
    private void PlayHurtEffects()
    {
        // Son aléatoire
        if (hurtSounds.Length > 0 && audioSource)
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
        if (healthBar) healthBar.UpdateHealth(currentHealth, maxHealth);
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

    public void Damage(int damageAmount)
    {
        Debug.Log($"[PlayerHealth] Méthode Damage appelée avec {damageAmount} dégâts");
        TakeDamage(damageAmount);
    }

    public void StartInvincibility()
    {
        isInvincible = true;
    }

    public void StopInvincibility()
    {
        isInvincible = false;
    }
}