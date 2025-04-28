using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Référence au menu pause UI
    public GameObject mainMenuUI; // Référence au menu principal UI

    // Références aux éléments UI pour les stats
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
        }
        isPaused = true;
    }

    // Méthode pour quitter le jeu
    public void QuitGame()
    {
        Application.Quit();
    }
}