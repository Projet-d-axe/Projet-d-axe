using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gère la santé du joueur, avec des événements pour les changements de santé et la mort.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Events")]
    public UnityEvent<int> OnHealthChanged; // Paramètre : nouvelle santé
    public UnityEvent OnDeath;

    // Propriétés pour accéder aux valeurs de santé
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        // Initialiser la santé au maximum au démarrage
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Applique des dégâts au joueur.
    /// </summary>
    /// <param name="damage">Montant des dégâts à infliger.</param>
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // Déjà mort

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Empêche la santé d'être négative

        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Soigne le joueur.
    /// </summary>
    /// <param name="healAmount">Montant de soin à appliquer.</param>
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Empêche de dépasser la santé max
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Réinitialise la santé du joueur au maximum.
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Modifie la santé maximale du joueur.
    /// </summary>
    /// <param name="newMaxHealth">Nouvelle valeur maximale de santé.</param>
    /// <param name="shouldHeal">Si vrai, restaure également la santé actuelle.</param>
    public void SetMaxHealth(int newMaxHealth, bool shouldHeal = false)
    {
        maxHealth = newMaxHealth;
        if (shouldHeal) currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        // Ici, vous pourriez ajouter d'autres logiques comme désactiver le joueur, jouer une animation, etc.
        Debug.Log("Player has died!");
    }

    // Méthode optionnelle pour vérifier si le joueur est mort
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}