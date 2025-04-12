using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyController : MonoBehaviour
{
    [Header("Configuration")]
    public EnemyData data;
    private NavMeshAgent agent;
    private Transform player;
    private EnemyHealth health;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float detectionRange = 10f;
    private float lastAttackTime;
    private bool isPlayerInRange = false;

    [Header("Visuals")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject attackEffect;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        
        // Meilleure méthode pour trouver le joueur
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        InitializeFromData();
    }

    private void InitializeFromData()
    {
        if (data != null)
        {
            health.SetMaxHealth(data.pv); // Adjusted to match the correct method signature
            agent.speed = data.speed;
            attackRange = data.attackRange;
            attackCooldown = data.attackSpeed;
        }
    }

    private void Start()
    {
        if (data == null && DataBaseManager.Instance != null)
        {
            // Meilleure gestion des données manquantes
            data = DataBaseManager.Instance.EnemyDataBase.GetRandomEnemyData();
        }
    }

    private void Update()
    {
        if (player == null || health.CurrentHealth <= 0) return;

        CheckPlayerDistance();
        HandleMovement();
        HandleAttack();
    }

    private void CheckPlayerDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerInRange = distanceToPlayer <= detectionRange;
    }

    private void HandleMovement()
    {
        if (!isPlayerInRange) 
        {
            agent.ResetPath();
            animator?.SetBool("IsMoving", false);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer > attackRange)
        {
            agent.SetDestination(player.position);
            animator?.SetBool("IsMoving", true);
        }
        else
        {
            agent.ResetPath();
            animator?.SetBool("IsMoving", false);
            FacePlayer();
        }
    }

    private void HandleAttack()
    {
        if (isPlayerInRange && 
            Vector3.Distance(transform.position, player.position) <= attackRange &&
            Time.time > lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void Attack()
{
    // Joue l'animation
    animator?.SetTrigger("Attack");
    
    // Applique les dégâts
    if (Vector3.Distance(transform.position, player.position) <= attackRange * 1.2f)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth?.TakeDamage(data.damage);
    }
}

    // Appelé par l'événement d'animation
    public void OnAttackAnimationEvent()
    {
        // Logique supplémentaire lors de l'impact de l'attaque
        Debug.Log("Attack landed!");
    }

    private void OnDrawGizmosSelected()
    {
        // Visualisation des ranges dans l'éditeur
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}