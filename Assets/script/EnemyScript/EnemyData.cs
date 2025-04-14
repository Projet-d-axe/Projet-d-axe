using UnityEngine;

[System.Serializable]
public class EnemyData
{
    //info
    public string enemyName;
    public GameObject prefab;
    public EnemyType eType;
    public MovementType mType;

    //health
    public int pv = 100;
    public float iFrameDuration = 0.5f;
    //fight
    public int damage = 10;
    public float attackCooldown;
    public float attackRange = 2f;
    public float attackSpeed = 1f;
    
    //detection
    public float detectionRange = 10f;
    public float forgetRange = 15f;
    public bool canForget;

    //stats
    public float speed = 3.5f;

    //references
    public GameObject deathObject;
    public AudioClip deathSfx;
}

public enum EnemyType
{
    Melee,
    Ranged,
    Boss
}

public enum MovementType
{
    Stationary,
    Patrol,
    Random
}