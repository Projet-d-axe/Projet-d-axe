using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weapons; // Tableau des armes disponibles
    private int currentWeaponIndex = 0; // Index de l'arme actuelle

    void Start()
    {
        // Désactiver toutes les armes au début sauf la première
        for (int i = 1; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        // Activer la première arme
        if (weapons.Length > 0)
        {
            weapons[currentWeaponIndex].SetActive(true);
        }
    }

    void Update()
    {
        // Tirer avec la touche X
        if (Input.GetKeyDown(KeyCode.X))
        {
            Shoot();
        }

        // Changer d'arme avec la touche Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            NextWeapon();
        }
    }

    void Shoot()
    {
        // Récupérer l'arme actuelle
        GameObject currentWeapon = weapons[currentWeaponIndex];

        // Logique de tir (exemple : instancier une balle ou jouer une animation)
        Debug.Log("Tir avec l'arme : " + currentWeapon.name);
        // Vous pouvez ajouter ici la logique spécifique pour chaque arme (ex : instancier une balle, jouer un son, etc.)
    }

    

    void SwitchWeapon(int newIndex)
    {
        // Désactiver l'arme actuelle
        weapons[currentWeaponIndex].SetActive(false);

        // Mettre à jour l'index de l'arme actuelle
        currentWeaponIndex = newIndex;

        // Activer la nouvelle arme
        weapons[currentWeaponIndex].SetActive(true);
    }

    void NextWeapon()
    {
        int newIndex = (currentWeaponIndex + 1) % weapons.Length;
        SwitchWeapon(newIndex);
    }
}