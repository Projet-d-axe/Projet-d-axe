using UnityEngine;
[CreateAssetMenu(menuName = "EnnemyStats")]
[System.Serializable]
public class EnemyData : ScriptableObject
{
    //info
    [Header("Info")]
    public string enemyName;
    public EnemyType eType;
    public MovementType mType;


    [Header("Health")]
    //health
    public int pv = 100;
    public float iFrameDuration = 0.5f;

    [Header("Fighting Stats")]
    //fight
    public int damage = 10;
    public float attackCooldown;
    public float attackRange = 2f;
    public float attackSpeed = 1f;

    public Vector2 knockbackAngle;
    public float knockbackForce;

    [Header("DetectionStats")]
    //detection
    public float detectionRange = 10f;
    public float playerDetectedWaitTime = 1f;
    public float forgetRange = 15f;
    public bool canForget;
    public float edgeDetection;
    public float wallDetection;
    public bool canFallOffLedges;

    [Header("Movement Stats")]
    //stats
    public float speed = 3.5f;

    [Header("References")]
    //references
    public GameObject prefab;
    public GameObject deathObject;
    public AudioClip deathSfx;
    public GameObject attackObject;
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