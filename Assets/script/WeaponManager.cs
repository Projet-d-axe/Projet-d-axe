using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Ajout de ce namespace pour IEnumerator

public class WeaponManager : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public GameObject weaponObject;
        public int maxAmmo = 30;
        public int damage = 10;
        public float fireRate = 0.2f;
        public AudioClip shootSound;
        public AudioClip reloadSound;
        public ParticleSystem muzzleFlash;
        [HideInInspector] public int currentAmmo;
    }

    public Weapon[] weapons;
    private int currentWeaponIndex = 0;
    private float nextFireTime = 0f;
    private AudioSource audioSource;
    private Animator animator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        // Initialiser les armes
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].currentAmmo = weapons[i].maxAmmo;
            weapons[i].weaponObject.SetActive(i == currentWeaponIndex);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && Time.time >= nextFireTime)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            NextWeapon();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void Shoot()
    {
        Weapon currentWeapon = weapons[currentWeaponIndex];
        
        if (currentWeapon.currentAmmo <= 0)
        {
            // Jouer un son "clic" quand pas de munitions
            return;
        }

        currentWeapon.currentAmmo--;
        nextFireTime = Time.time + currentWeapon.fireRate;

        // Effets
        if (currentWeapon.muzzleFlash != null)
            currentWeapon.muzzleFlash.Play();
        
        if (currentWeapon.shootSound != null)
            audioSource.PlayOneShot(currentWeapon.shootSound);

        // Animation
        animator.SetTrigger("Shoot");

        // Logique de tir
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<EnemyHealth>().TakeDamage(currentWeapon.damage);
            }
        }
    }

    void Reload()
    {
        Weapon currentWeapon = weapons[currentWeaponIndex];
        
        if (currentWeapon.currentAmmo == currentWeapon.maxAmmo) return;

        animator.SetTrigger("Reload");
        if (currentWeapon.reloadSound != null)
            audioSource.PlayOneShot(currentWeapon.reloadSound);

        currentWeapon.currentAmmo = currentWeapon.maxAmmo;
    }

    void SwitchWeapon(int newIndex)
    {
        if (newIndex == currentWeaponIndex) return;

        animator.SetTrigger("SwitchWeapon");
        StartCoroutine(SwitchWeaponDelay(newIndex));
    }

    // Coroutine pour le changement d'arme avec délai
    IEnumerator SwitchWeaponDelay(int newIndex)
    {
        yield return new WaitForSeconds(0.2f);
        weapons[currentWeaponIndex].weaponObject.SetActive(false);
        
        currentWeaponIndex = newIndex;
        weapons[currentWeaponIndex].weaponObject.SetActive(true);
    }

    void NextWeapon()
    {
        int newIndex = (currentWeaponIndex + 1) % weapons.Length;
        SwitchWeapon(newIndex);
    }

    public string GetCurrentAmmoText()
    {
        Weapon w = weapons[currentWeaponIndex];
        return $"{w.currentAmmo} / {w.maxAmmo}";
    }

    public string GetCurrentWeaponName()
    {
        return weapons[currentWeaponIndex].weaponObject.name;
    }
}