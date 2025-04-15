using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public enum BulletType { Normal, Platform, Piercing }

    [Header("Settings")]
    public float fireRate = 0.2f;
    public int maxAmmo = 30;
    public float reloadTime = 1f;
    public float bulletSpeed = 20f;
    public int poolSize = 10;

    [Header("Prefabs")]
    public GameObject normalBulletPrefab;
    public GameObject platformBulletPrefab;
    public GameObject piercingBulletPrefab;

    [Header("Effects")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public ParticleSystem muzzleFlash;
    public Transform firePoint;

    [Header("State")]
    [SerializeField] private int currentAmmo;
    [SerializeField] private BulletType currentBulletType = BulletType.Normal;
    private float nextFireTime;
    private bool isReloading;
    private Dictionary<BulletType, Queue<GameObject>> bulletPools;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public BulletType CurrentBulletType => currentBulletType;

    void Start()
    {
        currentAmmo = maxAmmo;
        InitializePools();
    }

    void Update()
    {
        // Rechargement avec la touche R
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    #region Pooling System
    void InitializePools()
    {
        bulletPools = new Dictionary<BulletType, Queue<GameObject>>()
        {
            { BulletType.Normal, CreatePool(normalBulletPrefab) },
            { BulletType.Platform, CreatePool(platformBulletPrefab) },
            { BulletType.Piercing, CreatePool(piercingBulletPrefab) }
        };
    }

    Queue<GameObject> CreatePool(GameObject prefab)
    {
        Queue<GameObject> pool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(prefab);
            bullet.SetActive(false);
            pool.Enqueue(bullet);
        }
        return pool;
    }
    #endregion

    #region Shooting System
    public void AttemptShoot(Vector2 direction)
    {
        if (isReloading) return;
        
        if (Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot(direction);
            nextFireTime = Time.time + fireRate;
            currentAmmo--;
        }
        else if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot(Vector2 direction)
    {
        if (bulletPools[currentBulletType].Count == 0) 
            ExpandPool(currentBulletType, 1);

        GameObject bullet = bulletPools[currentBulletType].Dequeue();
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        bullet.SetActive(true);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = direction * bulletSpeed;

        if (muzzleFlash != null) muzzleFlash.Play();
        if (shootSound != null) AudioSource.PlayClipAtPoint(shootSound, firePoint.position);
    }
    #endregion

    #region Reload System
    IEnumerator Reload()
    {
        isReloading = true;
        
        if (reloadSound != null)
            AudioSource.PlayClipAtPoint(reloadSound, transform.position);
        

        yield return new WaitForSeconds(reloadTime);
        
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void ExpandPool(BulletType type, int amount)
    {
        GameObject prefab = GetPrefabByType(type);
        for (int i = 0; i < amount; i++)
        {
            GameObject bullet = Instantiate(prefab);
            bullet.SetActive(false);
            bulletPools[type].Enqueue(bullet);
        }
    }
    #endregion

    #region Bullet Management
    public void SetBulletType(BulletType type)
    {
        currentBulletType = type;
    }

    GameObject GetPrefabByType(BulletType type)
    {
        return type switch
        {
            BulletType.Normal => normalBulletPrefab,
            BulletType.Platform => platformBulletPrefab,
            BulletType.Piercing => piercingBulletPrefab,
            _ => normalBulletPrefab
        };
    }

    public void ReturnBulletToPool(GameObject bullet, BulletType type)
    {
        bullet.SetActive(false);
        bulletPools[type].Enqueue(bullet);
    }
    #endregion
}