using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TokenSystem : MonoBehaviour
{
    [Header("Token Settings")]
    public int tokensCollected = 0;
    public int tokensRequiredPerWeapon = 3;
    public List<WeaponSystem> allWeapons; // Toutes les armes disponibles

    [Header("UI References")]
    public Text tokensText;
    public Image[] weaponIcons; // Icônes des armes dans l'UI
    public Color lockedWeaponColor = Color.gray;
    public Color unlockedWeaponColor = Color.white;

    [Header("Feedback")]
    public GameObject unlockEffect;
    public AudioClip unlockSound;
    public AudioClip lockedWeaponSound;
    public GameObject lockedWeaponEffect;

    private AudioSource audioSource;
    private PlayerController playerController;
    private List<WeaponSystem> lockedWeapons = new List<WeaponSystem>();

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        InitializeWeapons();
        UpdateUI();
    }

    private void InitializeWeapons()
    {
        // La première arme est toujours débloquée
        if (allWeapons.Count > 0)
        {
            allWeapons[0].gameObject.SetActive(true);
            
            // Verrouille les autres armes
            for (int i = 1; i < allWeapons.Count; i++)
            {
                allWeapons[i].gameObject.SetActive(false);
                lockedWeapons.Add(allWeapons[i]);
            }
        }
    }

    public void AddToken(int amount = 1)
    {
        tokensCollected += amount;
        UpdateUI();

        CheckWeaponUnlock();
    }

    private void CheckWeaponUnlock()
    {
        if (tokensCollected >= tokensRequiredPerWeapon && lockedWeapons.Count > 0)
        {
            UnlockNextWeapon();
        }
    }

    private void UnlockNextWeapon()
    {
        tokensCollected -= tokensRequiredPerWeapon;
        WeaponSystem weaponToUnlock = lockedWeapons[0];
        
        lockedWeapons.RemoveAt(0);
        weaponToUnlock.gameObject.SetActive(true);

        PlayUnlockEffects();
        UpdateUI();
    }

    public bool IsWeaponUnlocked(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= allWeapons.Count) return false;
        return !lockedWeapons.Contains(allWeapons[weaponIndex]);
    }

    public void PlayUnlockEffects()
    {
        if (unlockEffect != null)
            Instantiate(unlockEffect, transform.position, Quaternion.identity);
        
        if (unlockSound != null)
            audioSource.PlayOneShot(unlockSound);
    }

    public void PlayLockedWeaponFeedback()
    {
        if (lockedWeaponSound != null)
            audioSource.PlayOneShot(lockedWeaponSound);
        
        if (lockedWeaponEffect != null)
            Instantiate(lockedWeaponEffect, transform.position, Quaternion.identity);
    }

    private void UpdateUI()
    {
        // Mise à jour du texte des tokens
        if (tokensText != null)
        {
            tokensText.text = $"Tokens: {tokensCollected}/{tokensRequiredPerWeapon}";
        }

        // Mise à jour des icônes d'armes
        if (weaponIcons != null && weaponIcons.Length == allWeapons.Count)
        {
            for (int i = 0; i < weaponIcons.Length; i++)
            {
                weaponIcons[i].color = IsWeaponUnlocked(i) ? unlockedWeaponColor : lockedWeaponColor;
            }
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("TokensCollected", tokensCollected);
        
        // Sauvegarde l'état de chaque arme
        for (int i = 0; i < allWeapons.Count; i++)
        {
            PlayerPrefs.SetInt($"WeaponUnlocked_{i}", IsWeaponUnlocked(i) ? 1 : 0);
        }
    }

    public void LoadData()
    {
        tokensCollected = PlayerPrefs.GetInt("TokensCollected", 0);
        lockedWeapons.Clear();

        for (int i = 0; i < allWeapons.Count; i++)
        {
            bool isUnlocked = PlayerPrefs.GetInt($"WeaponUnlocked_{i}", i == 0 ? 1 : 0) == 1;
            allWeapons[i].gameObject.SetActive(isUnlocked);
            
            if (!isUnlocked && i > 0)
            {
                lockedWeapons.Add(allWeapons[i]);
            }
        }
        
        UpdateUI();
    }
}