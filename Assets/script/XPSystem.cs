using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


[System.Serializable]

public class XPSystem : MonoBehaviour
{
    [Header("XP Settings")]
    public float baseMaxXP = 100f;
    public float xpGrowthMultiplier = 1.5f;
    public float currentXP;
    public int currentLevel = 1;
    public float xpGainRate = 10f;
    public AnimationCurve xpCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("UI References")]
    public Slider xpBar;
    public Text levelText;
    public GameObject levelUpEffect;
    public AudioClip levelUpSound;


    private float maxXP;
    private AudioSource audioSource;



    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        maxXP = CalculateXPForLevel(currentLevel);
        currentXP = 0f;
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P)) // Debug
        {
            GainXP(xpGainRate);
        }
    }

    float CalculateXPForLevel(int level)
    {
        return baseMaxXP * Mathf.Pow(xpGrowthMultiplier, level - 1);
    }

    public void GainXP(float amount)
    {
        float xpBefore = currentXP / maxXP;
        currentXP += amount;
        float xpAfter = currentXP / maxXP;

        StartCoroutine(AnimateXPGain(xpBefore, xpAfter));

        if (currentXP >= maxXP)
        {
            LevelUp();
        }
    }

    IEnumerator AnimateXPGain(float startValue, float endValue)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            xpBar.value = Mathf.Lerp(startValue, endValue, xpCurve.Evaluate(t));
            yield return null;
        }
        
        xpBar.value = endValue;
    }

    void LevelUp()
    {
        currentLevel++;

        currentXP -= maxXP;
        maxXP = CalculateXPForLevel(currentLevel);

        if (levelUpEffect != null)
            Instantiate(levelUpEffect, transform.position, Quaternion.identity);
        
        if (levelUpSound != null)
            audioSource.PlayOneShot(levelUpSound);

        Debug.Log("Level Up! Now level " + currentLevel);
    }



    void UpdateUI()
    {

        if (xpBar != null)
        {
            xpBar.maxValue = maxXP;
            xpBar.value = currentXP;
        }

        if (levelText != null)
        {
            levelText.text = "Niveau : " + currentLevel;
            Debug.Log("Niveau : " + currentLevel);
        }
        
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("PlayerLevel", currentLevel);
        PlayerPrefs.SetFloat("PlayerXP", currentXP);
    }

    public void LoadData()
    {
        currentLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        currentXP = PlayerPrefs.GetFloat("PlayerXP", 0);
        maxXP = CalculateXPForLevel(currentLevel);
        UpdateUI();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUI();
        LoadData();
    }
}