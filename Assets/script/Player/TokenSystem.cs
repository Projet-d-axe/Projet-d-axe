using UnityEngine;
using System.Collections.Generic;

public class TokenSystem : MonoBehaviour
{
    [Header("Settings")]
    public int tokensCollected { get; private set; }
    public int tokensRequiredPerWeapon = 3;
    
    [Header("Debug")]
    [SerializeField] private List<int> unlockedWeaponIndices = new List<int>() { 0 }; // Arme 0 débloquée par défaut
    
    private WeaponUI weaponUI;
    private PlayerController playerController;

    void Start()
    {
        weaponUI = FindObjectOfType<WeaponUI>();
        playerController = GetComponent<PlayerController>();
        InitializeWeapons();
        UpdateUI();
    }

    private void InitializeWeapons()
    {
        // Désactive toutes les armes sauf celles débloquées
        if (playerController != null && playerController.weapons.Count > 0)
        {
            for (int i = 0; i < playerController.weapons.Count; i++)
            {
                playerController.weapons[i].gameObject.SetActive(IsWeaponUnlocked(i));
            }
        }
    }

    public void AddToken()
    {
        tokensCollected++;
        
        if (CanUnlockNextWeapon())
        {
            UnlockNextWeapon();
        }
        
        UpdateUI();
    }

    private bool CanUnlockNextWeapon()
    {
        return tokensCollected >= tokensRequiredPerWeapon && 
               unlockedWeaponIndices.Count < playerController.weapons.Count;
    }

    private void UnlockNextWeapon()
    {
        tokensCollected -= tokensRequiredPerWeapon;
        int nextWeaponIndex = unlockedWeaponIndices.Count;
        unlockedWeaponIndices.Add(nextWeaponIndex);

        playerController.weapons[nextWeaponIndex].gameObject.SetActive(true);
        
        if (weaponUI != null)
            {
                weaponUI.UnlockWeapon(nextWeaponIndex);
            }

        

    }

    public bool IsWeaponUnlocked(int index)
    {
        return unlockedWeaponIndices.Contains(index);
    }

    public void UpdateWeaponUnlockStatus()
    {
        if (weaponUI != null && playerController != null)
        {
            weaponUI.UpdateWeaponSlotsVisuals(playerController.currentWeaponIndex);
        }
    }

    private void UpdateUI()
    {
        if (weaponUI != null)
        {
            UpdateWeaponUnlockStatus();
        }
    }
    

    public void LoadData()
    {
        tokensCollected = PlayerPrefs.GetInt("TokensCollected", 0);
        
        string savedWeapons = PlayerPrefs.GetString("UnlockedWeapons", "0");
        string[] indices = savedWeapons.Split(',');
        
        unlockedWeaponIndices.Clear();
        foreach (string index in indices)
        {
            if (int.TryParse(index, out int i))
                unlockedWeaponIndices.Add(i);
        }
        
        InitializeWeapons();
        UpdateUI();
    }
}