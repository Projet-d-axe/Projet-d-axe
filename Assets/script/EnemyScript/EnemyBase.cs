using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private EnemyData eD;
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    private GameObject deathObject;
    private AudioClip deathSFX;
    private NavMeshAgent agent;
    private Collider enemyCollider;

    [Header("Health")]
    private int health;

    [Header("Fight")]
    private int damage;
    private float attackCD;
    private bool canAttack;
    private float attackRange;
    private float attackSpeed;

    [Header("Detection")]
    private float detectionRange;
    private float forgetRange;
    private bool canForget;

    [Header("Movement")]
    private float speed;

    [Header("Others")]
    private EnemyType enemyType;
    private MovementType movementType;

    private void Awake()
    {
        deathObject = eD.deathObject;
        deathSFX = eD.deathSfx;
        health = eD.pv;
        damage = eD.damage;
        attackCD = eD.attackCooldown;
        attackRange = eD.attackRange;
        attackSpeed = eD.attackSpeed;
        detectionRange = eD.detectionRange;
        forgetRange = eD.forgetRange;
        canForget = eD.canForget;
        speed = eD.speed;
        enemyType = eD.eType;
        movementType = eD.mType;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
