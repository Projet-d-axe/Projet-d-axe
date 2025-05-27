using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingManager : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private Dropdown fpsDropdown;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer audioMixer; // Optionnel pour un mixer audio

    private const string VOLUME_KEY = "VolumePref";
    private const string FPS_KEY = "FPSPref";

    private void Awake()
    {
        // Initialisation des écouteurs d'événements
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (fpsDropdown != null)
        {
            fpsDropdown.onValueChanged.AddListener(SetFPS);
        }
    }

    private void Start()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Chargement du volume
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.75f);
        SetVolume(savedVolume);
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }

        // Chargement des FPS
        int savedFPS = PlayerPrefs.GetInt(FPS_KEY, 1); // 1 = 60FPS par défaut
        SetFPS(savedFPS);
        if (fpsDropdown != null)
        {
            fpsDropdown.value = savedFPS;
        }
    }

    public void SetVolume(float volume)
    {
        // Deux méthodes au choix :

        // 1. Méthode simple (sans AudioMixer)
        AudioListener.volume = volume;

        // 2. Méthode avec AudioMixer (décommentez si utilisé)
        // audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat(VOLUME_KEY, volume);
        Debug.Log($"Volume set to: {volume}");
    }

    public void SetFPS(int fpsIndex)
    {
        int targetFPS = fpsIndex switch
        {
            0 => 30,
            1 => 60,
            2 => 120,
            _ => 60
        };

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
        PlayerPrefs.SetInt(FPS_KEY, fpsIndex);
        Debug.Log($"FPS set to: {targetFPS} (Index: {fpsIndex})");
    }

    public void ResetToDefaults()
    {
        // Réinitialisation aux valeurs par défaut
        SetVolume(0.75f);
        SetFPS(1); // 60 FPS
        
        if (volumeSlider != null) volumeSlider.value = 0.75f;
        if (fpsDropdown != null) fpsDropdown.value = 1;
    }

    private void OnDestroy()
    {
        // Nettoyage des écouteurs d'événements
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
        }

        if (fpsDropdown != null)
        {
            fpsDropdown.onValueChanged.RemoveListener(SetFPS);
        }
        
        PlayerPrefs.Save();
    }
}