using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{

    public TextMeshProUGUI weaponNameText;
    public Image ammoIcon;
    public Sprite[] weaponIcons;

    private WeaponSystem currentWeapon;

    public void SetCurrentWeapon(WeaponSystem weapon, int weaponIndex)
    {
        currentWeapon = weapon;

        if (weaponIcons != null && weaponIndex < weaponIcons.Length)
        {
            ammoIcon.sprite = weaponIcons[weaponIndex];
        }

        if (weaponNameText && currentWeapon != null)
        {
            weaponNameText.text = currentWeapon.weaponName;
        }
    }

    void Update()
    {
        if (currentWeapon != null)
        {
            int reserve = currentWeapon.GetReserveAmmo();
        }
    }
}
