using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int damage;
    public float fireRate;
    public int maxAmmo;
    public GameObject modelPrefab;
    public AudioClip shootSound;
    public ParticleSystem muzzleFlash;
}