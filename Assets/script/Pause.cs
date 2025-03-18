using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Référence au Canvas "MenuPause"
    public Mouvement playerMovement; // Référence au script de mouvement du joueur

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    void Update()
    {
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

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}