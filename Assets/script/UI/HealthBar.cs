using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Configuration")]
    public bool isPlayerHealthBar = false;
    public bool followTarget = true;
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    [Header("References")]
    public Image fillImage;        // Remplissage de la barre
    public Image borderImage;      // Contour qui sera au-dessus
    public Gradient colorGradient;
    
    private Transform target;
    private Camera mainCamera;
    private bool isInitialized = false;

    private void Awake()
    {
        if (fillImage == null)
        {
            Debug.LogError($"[HealthBar] Fill Image non assignée sur {gameObject.name}! Cherchez le composant Image 'Fill' et assignez-le au champ Fill Image.");
            enabled = false;
            return;
        }

        // S'assurer que le contour est bien au-dessus
        if (borderImage != null)
        {
            borderImage.transform.SetAsLastSibling();
        }

        isInitialized = true;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("[HealthBar] Pas de Camera.main trouvée!");
        }
        
        // Si c'est une barre de vie d'ennemi, elle doit suivre l'ennemi
        if (!isPlayerHealthBar && followTarget)
        {
            target = transform.parent;
            if (target == null)
            {
                Debug.LogError($"[HealthBar] La barre de vie {gameObject.name} n'a pas de parent!");
                enabled = false;
                return;
            }
            transform.SetParent(null); // Détacher du parent pour que la barre reste droite
        }
    }

    private void LateUpdate()
    {
        if (!isInitialized) return;

        if (followTarget && target != null && mainCamera != null)
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
        if (!isInitialized)
        {
            Debug.LogWarning($"[HealthBar] Tentative de mise à jour de la santé sur {gameObject.name} avant l'initialisation!");
            return;
        }

        if (fillImage == null)
        {
            Debug.LogError($"[HealthBar] Fill Image manquante sur {gameObject.name}!");
            return;
        }

        float fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        fillImage.fillAmount = fillAmount;
        
        // Mettre à jour la couleur en fonction du pourcentage de vie
        if (colorGradient != null)
        {
            fillImage.color = colorGradient.Evaluate(fillAmount);
        }
    }
} 