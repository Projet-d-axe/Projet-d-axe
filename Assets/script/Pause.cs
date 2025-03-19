using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Référence au menu pause UI
    public GameObject mainMenuUI; // Référence au menu principal UI

    // Références aux éléments UI pour les stats
    public Text strengthText;
    public Text agilityText;
    public Text fireRateText;
    public Text skillPointsText;

    private bool isPaused = false;

    void Start()
    {
        // Désactive le menu pause au démarrage
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // Active le menu principal au démarrage
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
        }
    }

    void Update()
    {
        // Active/désactive le menu pause avec la touche Échap
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Méthode pour reprendre le jeu
    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        isPaused = false;
    }

    // Méthode pour mettre le jeu en pause
    void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            UpdateStatsUI(); // Met à jour l'UI des stats
        }
        isPaused = true;
    }

    // Méthode pour quitter le jeu
    public void QuitGame()
    {
        Application.Quit();
    }

    // Méthodes pour attribuer des points de stats
    public void AddStrength()
    {
        XPSystem xpSystem = FindObjectOfType<XPSystem>();
        if (xpSystem != null && xpSystem.skillPoints > 0)
        {
            xpSystem.AddStrength();
            UpdateStatsUI(); // Met à jour l'UI après attribution
        }
    }

    public void AddAgility()
    {
        XPSystem xpSystem = FindObjectOfType<XPSystem>();
        if (xpSystem != null && xpSystem.skillPoints > 0)
        {
            xpSystem.AddAgility();
            UpdateStatsUI(); // Met à jour l'UI après attribution
        }
    }

    public void AddFireRate()
    {
        XPSystem xpSystem = FindObjectOfType<XPSystem>();
        if (xpSystem != null && xpSystem.skillPoints > 0)
        {
            xpSystem.AddFireRate();
            UpdateStatsUI(); // Met à jour l'UI après attribution
        }
    }

    // Met à jour l'interface utilisateur des stats
    void UpdateStatsUI()
    {
        XPSystem xpSystem = FindObjectOfType<XPSystem>();
        if (xpSystem != null)
        {
            Debug.Log("Points de compétences disponibles : " + xpSystem.skillPoints);
            if (strengthText != null)
            {
                strengthText.text = "Force : " + xpSystem.strength;
            }
            if (agilityText != null)
            {
                agilityText.text = "Agilité : " + xpSystem.agility;
            }
            if (fireRateText != null)
            {
                fireRateText.text = "Vitesse de tir : " + xpSystem.fireRate.ToString("F1");
            }
            if (skillPointsText != null)
            {
                skillPointsText.text = "Points de compétence : " + xpSystem.skillPoints;
            }
        }
        else
        {
            Debug.LogWarning("XPSystem non trouvé dans la scène !");
        }
    }
}