using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        // Trouve automatiquement le PlayerHealth si non assigné
        playerHealth = FindObjectOfType<PlayerHealth>();
        
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth non trouvé dans la scène!");
            return;
        }

        // S'abonne aux événements
        playerHealth.OnHealthChanged.AddListener(UpdateHealthUI);
    }

    private void Start()
    {
        if (healthSlider == null)
        {
            Debug.LogError("Health Slider non assigné!");
            return;
        }

        // Initialisation
        healthSlider.maxValue = playerHealth.MaxHealth;
        healthSlider.value = playerHealth.CurrentHealth;
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        
        Debug.Log($"UI Health Updated: {currentHealth}/{maxHealth}");
    }

    private void OnDestroy()
    {
        // Nettoyage - se désabonne des événements
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthUI);
        }
    }
}