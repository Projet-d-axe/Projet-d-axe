using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string weaponName;
        public GameObject weaponObject;
        public int maxAmmo;
        [HideInInspector] public int currentAmmo;
    }

    public Weapon[] weapons;
    public float switchDelay = 0.3f;
    
    private int currentWeaponIndex;
    private bool isSwitching;

    void Start()
    {
        InitializeWeapons();
    }

    void InitializeWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].currentAmmo = weapons[i].maxAmmo;
            weapons[i].weaponObject.SetActive(i == currentWeaponIndex);
        }
    }

    void Update()
    {
        if (isSwitching) return;

        HandleWeaponSwitchInput();
    }

    void HandleWeaponSwitchInput()
    {
        // Changement avec molette
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int newIndex = currentWeaponIndex + (scroll > 0 ? -1 : 1);
            if (newIndex < 0) newIndex = weapons.Length - 1;
            else if (newIndex >= weapons.Length) newIndex = 0;
            
            StartCoroutine(SwitchWeapon(newIndex));
        }

        // Changement avec touches 1-9
        for (int i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                StartCoroutine(SwitchWeapon(i));
            }
        }
    }

    IEnumerator SwitchWeapon(int newIndex)
    {
        if (newIndex == currentWeaponIndex || newIndex < 0 || newIndex >= weapons.Length)
            yield break;

        isSwitching = true;

        // Désactiver l'arme actuelle
        weapons[currentWeaponIndex].weaponObject.SetActive(false);

        // Attendre le délai
        yield return new WaitForSeconds(switchDelay);

        // Activer la nouvelle arme
        currentWeaponIndex = newIndex;
        weapons[currentWeaponIndex].weaponObject.SetActive(true);

        isSwitching = false;
    }
}