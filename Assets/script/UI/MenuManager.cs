using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public Canvas mainMenuCanvas;


    private void Start()
    {
        if (mainMenuCanvas != null)
        {
            Debug.Log("Main Menu Canvas found");
            DisableMainMenu();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed");
            if (mainMenuCanvas != null)
            { 
                if (mainMenuCanvas.gameObject.activeSelf)
                {
                    Debug.Log("Disabling Main Menu");

                    DisableMainMenu();
                }
                else
                {
                    Debug.Log("Enabling Main Menu");
                    EnableMainMenu();
                }

            }
        }
    }

    public void LoadFirstScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main_Menu");
    }
    public void LoadSettingsMenu()
    {
        SceneManager.LoadScene("Settings_Menu");
    }

    public void DisableMainMenu()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        Time.timeScale = 1; // Reprend le temps du jeu
    }
    public void EnableMainMenu()
    {
        mainMenuCanvas.gameObject.SetActive(true);
        Time.timeScale = 0; // Met le temps du jeu sur pause
    }

}