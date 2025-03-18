using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject mainMenuUI;
    public Mouvement playerMovement;
    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
        }
        if (playerMovement != null)
        {
            playerMovement.SetPaused(true);
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
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        Time.timeScale = 1f;
        if (playerMovement != null)
        {
            playerMovement.SetPaused(false);
        }
        isPaused = false;
    }

    void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        Time.timeScale = 0f;
        if (playerMovement != null)
        {
            playerMovement.SetPaused(true);
        }
        isPaused = true;
    }

    public void PlayGame()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(false);
        }
        if (playerMovement != null)
        {
            playerMovement.SetPaused(false);
        }
        Time.timeScale = 1f;
    }

    public void ReturnToMainMenu()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
        }
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        if (playerMovement != null)
        {
            playerMovement.SetPaused(true);
        }
        Time.timeScale = 0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}