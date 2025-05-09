using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private PlayerHealth playerHealth;

    private void Start()
    {
        // Initialisation
        healthSlider.maxValue = playerHealth.MaxHealth;
        healthSlider.value = playerHealth.CurrentHealth;
    }

    private void UpdateHealthUI(int currentHealth)
    {
        healthSlider.value = currentHealth;
        Debug.Log($"UI mis à jour : {currentHealth}/{healthSlider.maxValue}");
    }
}