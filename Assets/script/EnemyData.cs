// EnemyData.cs
using UnityEngine;

[System.Serializable]
public struct EnemyData
{
    public float maxHealth;
    public float movementSpeed;
    public bool isFrozen;
    public bool canBePlatform;
}

// WeaponStats.cs
[System.Serializable]
public struct WeaponStats
{
    public float damage;
    public float fireRate;
    public float projectileSpeed;
    public float freezeDuration;
    public int pierceCount;
    public float platformDuration;
    internal float reloadTime;
    internal object maxAmmo;
    internal float ProjectileSpeed;
    internal Color Color;

    public int Damage { get; internal set; }
}