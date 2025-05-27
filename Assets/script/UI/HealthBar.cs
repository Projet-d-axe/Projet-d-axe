using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Configuration")]
    public bool isPlayerHealthBar = false;
    public bool followTarget = true;
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    [Header("References")]
    public Image fillImage;
    public Gradient colorGradient;
    
    private Transform target;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        
        // Si c'est une barre de vie d'ennemi, elle doit suivre l'ennemi
        if (!isPlayerHealthBar && followTarget)
        {
            target = transform.parent;
            transform.SetParent(null); // Détacher du parent pour que la barre reste droite
        }
    }

    private void LateUpdate()
    {
        if (followTarget && target != null)
        {
            // Mettre à jour la position pour suivre la cible
            Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + offset);
            transform.position = screenPos;

            // Cacher la barre si l'ennemi est derrière la caméra
            bool isBehind = Vector3.Dot(target.position - mainCamera.transform.position, mainCamera.transform.forward) < 0;
            gameObject.SetActive(!isBehind);
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        float fillAmount = currentHealth / maxHealth;
        fillImage.fillAmount = fillAmount;
        
        // Mettre à jour la couleur en fonction du pourcentage de vie
        if (colorGradient != null)
        {
            fillImage.color = colorGradient.Evaluate(fillAmount);
        }
    }
} 