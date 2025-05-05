using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StatModifier
{
    public string modifierName;
    public float strengthMod = 0f;
    public float agilityMod = 0f;
    public float fireRateMod = 0f;
    public float duration = -1f; // -1 = permanent
}

public class XPSystem : MonoBehaviour
{
    [Header("XP Settings")]
    public float baseMaxXP = 100f;
    public float xpGrowthMultiplier = 1.5f;
    public float currentXP;
    public int currentLevel = 1;
    public int skillPoints = 0;
    public float xpGainRate = 10f;
    public AnimationCurve xpCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("UI References")]
    public Slider xpBar;
    public Text levelText;
    public Text skillPointsText;
    public Text strengthText;
    public Text agilityText;
    public Text fireRateText;
    public GameObject levelUpEffect;
    public AudioClip levelUpSound;

    [Header("Player Stats")]
    public int baseStrength = 0;
    public int baseAgility = 0;
    public float baseFireRate = 1.0f;
    public int statPointsPerLevel = 1;
    public int bonusSkillPointsEvery = 5;

    // Propriétés de compatibilité (Solution 2)
    public int strength {
        get { return baseStrength; }
        set { baseStrength = value; }
    }

    public int agility {
        get { return baseAgility; }
        set { baseAgility = value; }
    }

    public float fireRate {
        get { return baseFireRate; }
        set { baseFireRate = value; }
    }

    private float maxXP;
    private AudioSource audioSource;
    private List<StatModifier> activeModifiers = new List<StatModifier>();

    // Propriétés calculées
    public int TotalStrength {
        get {
            float total = baseStrength;
            foreach (var mod in activeModifiers) total += mod.strengthMod;
            return Mathf.RoundToInt(total);
        }
    }
    
    public int TotalAgility {
        get {
            float total = baseAgility;
            foreach (var mod in activeModifiers) total += mod.agilityMod;
            return Mathf.RoundToInt(total);
        }
    }
    
    public float TotalFireRate {
        get {
            float total = baseFireRate;
            foreach (var mod in activeModifiers) total += mod.fireRateMod;
            return Mathf.Clamp(total, 0.1f, 10f);
        }
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Start()
    {
        maxXP = CalculateXPForLevel(currentLevel);
        currentXP = 0f;
        UpdateUI();
    }

    void Update()
    {
        UpdateTemporaryModifiers();
        
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
        skillPoints += statPointsPerLevel;
        
        if (currentLevel % bonusSkillPointsEvery == 0)
        {
            skillPoints++;
            Debug.Log("Bonus skill point for reaching level " + currentLevel);
        }

        currentXP -= maxXP;
        maxXP = CalculateXPForLevel(currentLevel);

        if (levelUpEffect != null)
            Instantiate(levelUpEffect, transform.position, Quaternion.identity);
        
        if (levelUpSound != null)
            audioSource.PlayOneShot(levelUpSound);

        Debug.Log("Level Up! Now level " + currentLevel);

        if (currentXP >= maxXP)
        {
            LevelUp();
        }

        UpdateUI();
    }

    public void AddStatModifier(StatModifier modifier)
    {
        activeModifiers.Add(modifier);
        UpdateUI();
    }

    void UpdateTemporaryModifiers()
    {
        for (int i = activeModifiers.Count - 1; i >= 0; i--)
        {
            if (activeModifiers[i].duration > 0)
            {
                activeModifiers[i].duration -= Time.deltaTime;
                if (activeModifiers[i].duration <= 0)
                {
                    activeModifiers.RemoveAt(i);
                    UpdateUI();
                }
            }
        }
    }

    public void AddStrength()
    {
        if (skillPoints > 0)
        {
            baseStrength++;
            skillPoints--;
            UpdateUI();
        }
    }

    public void AddAgility()
    {
        if (skillPoints > 0)
        {
            baseAgility++;
            skillPoints--;
            UpdateUI();
        }
    }

    public void AddFireRate()
    {
        if (skillPoints > 0)
        {
            baseFireRate += 0.1f;
            skillPoints--;
            UpdateUI();
        }
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
        }

        if (skillPointsText != null)
        {
            skillPointsText.text = "Points : " + skillPoints;
        }

        if (strengthText != null)
        {
            strengthText.text = "Force : " + TotalStrength;
        }
        
        if (agilityText != null)
        {
            agilityText.text = "Agilité : " + TotalAgility;
        }
        
        if (fireRateText != null)
        {
            fireRateText.text = "Vit. Tir : " + TotalFireRate.ToString("F1");
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("PlayerLevel", currentLevel);
        PlayerPrefs.SetFloat("PlayerXP", currentXP);
        PlayerPrefs.SetInt("PlayerStrength", baseStrength);
        PlayerPrefs.SetInt("PlayerAgility", baseAgility);
        PlayerPrefs.SetFloat("PlayerFireRate", baseFireRate);
        PlayerPrefs.SetInt("PlayerSkillPoints", skillPoints);
    }

    public void LoadData()
    {
        currentLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        currentXP = PlayerPrefs.GetFloat("PlayerXP", 0);
        baseStrength = PlayerPrefs.GetInt("PlayerStrength", 0);
        baseAgility = PlayerPrefs.GetInt("PlayerAgility", 0);
        baseFireRate = PlayerPrefs.GetFloat("PlayerFireRate", 1.0f);
        skillPoints = PlayerPrefs.GetInt("PlayerSkillPoints", 0);
        
        maxXP = CalculateXPForLevel(currentLevel);
        UpdateUI();
    }
}