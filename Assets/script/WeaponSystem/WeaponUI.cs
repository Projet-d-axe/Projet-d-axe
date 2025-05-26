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
        if (playerController == null) return;

        var weapons = playerController.weapons;

        for (int i = 0; i < weaponSlotIcons.Length && i < weapons.Count; i++)
        {
            var weapon = weapons[i];
            if (weapon.weaponIcon != null)
            {
                weaponSlotIcons[i].sprite = weapon.weaponIcon;
            }

            weaponSlotIcons[i].color = (i == 0) ? equippedColor : lockedColor;
        }

        currentEquippedIndex = 0;
    }


    public void SetCurrentWeapon(WeaponSystem weapon, int weaponIndex)
    {
        currentWeaponIcon.sprite = weapon.weaponIcon;

        weaponNameText.text = weapon.weaponName;
        
        UpdateWeaponSlots(weaponIndex);

        PlayEquipEffect(weaponIndex);
    }

    private void UpdateWeaponSlots(int equippedIndex)
    {
        currentEquippedIndex = equippedIndex;
        
        for (int i = 0; i < weaponSlotIcons.Length; i++)
        {
            if (tokenSystem.IsWeaponUnlocked(i))
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
        if (weaponIndex < weaponSlotIcons.Length)
        {
            weaponSlotIcons[weaponIndex].color = unlockedColor;
            PlayUnlockEffect(weaponIndex);
        }
    }

    private void PlayUnlockEffect(int index)
    {
        if (unlockEffect != null)
        {
            Instantiate(unlockEffect, weaponSlotIcons[index].transform.position, Quaternion.identity);
        }
    }

    private void PlayEquipEffect(int index)
    {
        if (equipEffect != null)
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