using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public float moveSpeed = 3.5f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float detectionRange = 15f;
    public float groundCheckDistance = 0.2f;
    public float patrolWaitTime = 2f;
    public int damage = 10;
    [HideInInspector] public float lastAttackTime;
}