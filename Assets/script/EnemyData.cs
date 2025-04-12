using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public string enemyName;
    public int pv = 100;
    public int damage = 10;
    public float speed = 3.5f;
    public GameObject prefab;
    public EnemyType type;
    internal float attackRange = 2f;
    internal float attackSpeed = 1f;
    public float detectionRange = 10f;
}

public enum EnemyType
{
    Melee,
    Ranged,
    Boss
}