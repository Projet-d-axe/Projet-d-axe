using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Référence au Canvas "MenuPause"

    private bool isPaused = false;

    void Start()
    {
        // Désactiver le menu pause au démarrage
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    void Update()
    {
        // Détecter l'appui sur la touche "Escape" pour activer/désactiver la pause
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

    public void Resume()
    {
        // Désactiver le menu pause
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        // Reprendre le temps
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Game is resumed");
    }

    void Pause()
    {
        // Activer le menu pause
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        // Mettre le jeu en pause
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("Game is paused");
    }

    public void LoadMenu()
    {
        // Reprendre le temps avant de charger une nouvelle scène
        Time.timeScale = 1f;
        // Charger la scène du menu principal (remplacez "MainMenu" par le nom de votre scène)
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        // Quitter l'application
        Application.Quit();
        Debug.Log("Quitting game...");
    }
}