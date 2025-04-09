using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    public bool IsPaused;
    private List<PauseMenu> _pauseMenus = new List<PauseMenu>();
    private void Awake()
    {
        if (_instance == null)
        
            _instance = this;
            else
            
                Destroy(gameObject); // Détruire le GameManager si une autre instance existe déjà

            DontDestroyOnLoad(gameObject); // Ne pas détruire cet objet lors du chargement d'une nouvelle scène

            IsPaused = false; // Initialiser IsPaused à false
    }
    public void setPause(bool pause)
    {
        IsPaused = pause;
        Time.timeScale = pause ? 0 : 1; // Mettre le temps sur pause ou le reprendre
        foreach (var menu in _pauseMenus)
        {
            menu.gameObject.SetActive(pause); // Activer ou désactiver le menu de pause
        }
    }
}
