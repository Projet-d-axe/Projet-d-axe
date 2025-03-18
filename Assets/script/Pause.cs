using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Référence au Canvas "PauseMenuUI"
    public GameObject mainMenuUI; // Référence au Canvas "MainMenuUI"
    public Mouvement playerMovement; // Référence au script de mouvement du joueur

    private bool isPaused = false;

    void Start()
    {
        // Désactiver le menu pause au démarrage
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // Activer le menu principal au démarrage
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
        }

        // Désactiver le mouvement du joueur au démarrage
        if (playerMovement != null)
        {
            playerMovement.SetPaused(true);
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
        Debug.Log("Reprendre le jeu");
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        Time.timeScale = 1f; // Reprendre le temps
        if (playerMovement != null)
        {
            playerMovement.SetPaused(false); // Réactiver le mouvement du joueur
        }
        isPaused = false;
    }

    void Pause()
    {
        Debug.Log("Mettre le jeu en pause");
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        Time.timeScale = 0f; // Mettre le jeu en pause
        if (playerMovement != null)
        {
            playerMovement.SetPaused(true); // Désactiver le mouvement du joueur
        }
        isPaused = true;
    }

    // Fonction pour démarrer le jeu
    public void PlayGame()
    {
        Debug.Log("Démarrer le jeu");
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(false); // Désactiver le menu principal
        }
        if (playerMovement != null)
        {
            playerMovement.SetPaused(false); // Réactiver le mouvement du joueur
        }
        Time.timeScale = 1f; // Reprendre le temps
    }

    // Fonction pour retourner au menu principal
    public void ReturnToMainMenu()
    {
        Debug.Log("Retour au menu principal");
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true); // Activer le menu principal
        }
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false); // Désactiver le menu pause
        }
        if (playerMovement != null)
        {
            playerMovement.SetPaused(true); // Désactiver le mouvement du joueur
        }
        Time.timeScale = 0f; // Mettre le jeu en pause
    }

    public void QuitGame()
    {
        Debug.Log("Quitter le jeu");
        Application.Quit();
    }
}