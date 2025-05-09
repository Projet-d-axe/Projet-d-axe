using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponConfig", menuName = "FPS Weapons/Weapon Configuration")]
public class WeaponConfigSO : ScriptableObject
{
    [Header("IDENTIFICATION")]
    [Tooltip("Nom d'affichage de l'arme")]
    public string weaponName = "New Weapon";
    [Tooltip("Identifiant unique pour la sauvegarde")]
    public string weaponID = "weapon_001";

    [Header("VISUALS")]
    [Tooltip("Icône pour l'UI")]
    public Sprite weaponIcon;
    [Tooltip("Modèle 3D de l'arme")]
    public GameObject weaponPrefab;
    [Tooltip("Position relative au joueur")]
    public Vector3 equipPosition = new Vector3(0.1f, -0.2f, 0.5f);
    [Tooltip("Rotation relative au joueur")]
    public Vector3 equipRotation = new Vector3(0f, 90f, 0f);

    [Header("PROJECTILE")]
    [Tooltip("Prefab du projectile principal")]
    public GameObject projectilePrefab;
    [Tooltip("Projectile alternatif si le principal n'est pas assigné")]
    public GameObject fallbackProjectile;
    [Tooltip("Effet visuel lors du tir")]
    public GameObject muzzleFlashEffect;
    [Tooltip("Point d'origine du projectile (si différent du canon)")]
    public Vector3 projectileSpawnOffset;

    [Header("COMBAT STATS")]
    [Range(1, 1000)] public float damage = 10f;
    [Range(0.01f, 20f)] public float fireRate = 0.5f;
    [Range(0f, 5f)] public float spreadAngle = 0.5f;
    [Tooltip("Dégâts supplémentaires à courte portée")]
    public float closeRangeDamageBonus = 5f;
    [Tooltip("Distance maximale pour le bonus de dégâts")]
    public float closeRangeThreshold = 5f;

    [Header("AMMUNITION")]
    [Range(1, 500)] public int magazineSize = 30;
    [Range(0, 500)] public int startingAmmo = 30;
    [Range(0, 500)] public int maxReserveAmmo = 120;
    [Range(0.1f, 10f)] public float reloadTime = 2f;
    public bool autoReload = true;
    public bool infiniteAmmo = false;

    [Header("RECOIL")]
    [Range(0f, 10f)] public float recoilForce = 1f;
    [Range(0f, 1f)] public float recoilRecoverySpeed = 0.2f;
    public Vector3 recoilPattern = new Vector3(0.1f, 0.5f, 0f);

    [Header("AUDIO")]
    public AudioClip shootSound;
    public AudioClip reloadStartSound;
    public AudioClip reloadFinishSound;
    public AudioClip emptyClickSound;
    [Range(0f, 1f)] public float volume = 0.8f;

    [Header("ANIMATION")]
    public AnimatorOverrideController animatorController;
    [Tooltip("Nom du paramètre de trigger pour l'animation de tir")]
    public string shootTrigger = "Fire";
    [Tooltip("Nom du paramètre booléen pour le rechargement")]
    public string reloadingBool = "IsReloading";

    [Header("SPECIAL FEATURES")]
    public bool isAutomatic = false;
    public int burstCount = 1;
    [Tooltip("Délai entre les tirs en mode rafale")]
    public float burstDelay = 0.1f;
    public bool hasAltFire = false;
    [Tooltip("Configuration pour le tir alternatif")]
    public AlternateFireConfig altFireConfig;

    [System.Serializable]
    public class AlternateFireConfig
    {
        public GameObject altProjectilePrefab;
        public float altDamage = 15f;
        public float altFireRate = 0.3f;
        public int altAmmoCost = 2;
    }

    // Méthode pour obtenir le prefab du projectile en fonction du mode de tir
    public GameObject GetProjectilePrefab(bool isAltFire = false)
    {
        if (isAltFire && hasAltFire)
        {
            return altFireConfig.altProjectilePrefab != null ? 
                   altFireConfig.altProjectilePrefab : 
                   projectilePrefab;
        }
        
        return projectilePrefab != null ? projectilePrefab : fallbackProjectile;
    }

    // Calcule les dégâts en fonction de la distance
    public float GetDamageAtRange(float distance)
    {
        if (distance <= closeRangeThreshold)
        {
            return damage + closeRangeDamageBonus;
        }
        return damage;
    }
}