using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [Header("Current Weapon Display")]
    public Image currentWeaponIcon;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI weaponNameText;

    [Header("Weapon Slots")]
    public Image[] weaponSlotIcons; // Les 4 slots d'armes (doivent être dans l'ordre 1-4)
    public Color equippedColor = Color.white;
    public Color unlockedColor = new Color(1, 1, 1, 0.4f);
    public Color lockedColor = Color.clear;

    [Header("Effects")]
    public ParticleSystem unlockEffect;
    public ParticleSystem equipEffect;

    private TokenSystem tokenSystem;
    private int currentEquippedIndex = -1;

    void Start()
    {
        tokenSystem = FindObjectOfType<TokenSystem>();
        InitializeUI();
    }

    private void InitializeUI()
    {
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("[WeaponUI] PlayerController not found!");
            return;
        }

        var weapons = playerController.weapons;

        if (weaponSlotIcons == null || weaponSlotIcons.Length == 0)
        {
            Debug.LogWarning("[WeaponUI] No weapon slot icons assigned!");
            return;
        }

        for (int i = 0; i < weaponSlotIcons.Length && i < weapons.Count; i++)
        {
            if (weaponSlotIcons[i] == null)
            {
                Debug.LogWarning($"[WeaponUI] Weapon slot icon at index {i} is null!");
                continue;
            }

            var weapon = weapons[i];
            if (weapon != null && weapon.weaponIcon != null)
            {
                weaponSlotIcons[i].sprite = weapon.weaponIcon;
            }

            weaponSlotIcons[i].color = (i == 0) ? equippedColor : lockedColor;
        }

        currentEquippedIndex = 0;
    }

    public void SetCurrentWeapon(WeaponSystem weapon, int weaponIndex)
    {
        if (weapon == null)
        {
            Debug.LogWarning("[WeaponUI] Trying to set null weapon!");
            return;
        }

        if (currentWeaponIcon != null)
        {
            currentWeaponIcon.sprite = weapon.weaponIcon;
        }

        if (weaponNameText != null)
        {
            weaponNameText.text = weapon.weaponName;
        }
        
        UpdateWeaponSlots(weaponIndex);
        PlayEquipEffect(weaponIndex);
    }

    private void UpdateWeaponSlots(int equippedIndex)
    {
        if (weaponSlotIcons == null || weaponSlotIcons.Length == 0)
        {
            Debug.LogWarning("[WeaponUI] No weapon slot icons to update!");
            return;
        }

        currentEquippedIndex = equippedIndex;
        
        for (int i = 0; i < weaponSlotIcons.Length; i++)
        {
            if (weaponSlotIcons[i] == null) continue;

            if (tokenSystem != null && tokenSystem.IsWeaponUnlocked(i))
            {
                weaponSlotIcons[i].color = (i == equippedIndex) ? equippedColor : unlockedColor;
                weaponSlotIcons[i].transform.localScale = (i == equippedIndex) ? Vector3.one * 1.1f : Vector3.one;
            }
            else
            {
                weaponSlotIcons[i].color = lockedColor;
            }
        }
    }

    public void UpdateWeaponSlotsVisuals(int equippedIndex)
    {
        UpdateWeaponSlots(equippedIndex);
    }

    public void UnlockWeapon(int weaponIndex)
    {
        if (weaponSlotIcons == null || weaponIndex >= weaponSlotIcons.Length)
        {
            Debug.LogWarning($"[WeaponUI] Cannot unlock weapon at index {weaponIndex}: invalid index or no slots!");
            return;
        }

        if (weaponSlotIcons[weaponIndex] != null)
        {
            weaponSlotIcons[weaponIndex].color = unlockedColor;
            PlayUnlockEffect(weaponIndex);
        }
    }

    private void PlayUnlockEffect(int index)
    {
        if (unlockEffect != null && index < weaponSlotIcons.Length && weaponSlotIcons[index] != null)
        {
            Instantiate(unlockEffect, weaponSlotIcons[index].transform.position, Quaternion.identity);
        }
    }

    private void PlayEquipEffect(int index)
    {
        if (equipEffect != null && index < weaponSlotIcons.Length && weaponSlotIcons[index] != null)
        {
            Instantiate(equipEffect, weaponSlotIcons[index].transform.position, Quaternion.identity);
        }
    }

    void Update()
    {
        if (currentEquippedIndex >= 0)
        {
        }
    }

}