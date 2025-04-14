using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    [Header("Références")]
    [SerializeField] private EnemyData data;
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject deathEffect;
    private NavMeshAgent agent;
    private Collider enemyCollider;

    [Header("Paramètres de Détection")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float forgetRange = 20f;
    private bool playerDetected;

    [Header("Combat")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask playerLayer;
    private float lastAttackTime;
    private bool isAttacking;

    [Header("Patrouille")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waitTime = 2f;
    private int currentPatrolIndex;
    private bool isWaiting;
    private Vector3 lastKnownPlayerPosition;

    [Header("Physique")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    private bool isGrounded;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyCollider = GetComponent<Collider>();
        
        // Configuration initiale
        agent.radius = 0.5f;
        agent.height = 1.8f;
        agent.baseOffset = 0.1f;
        agent.stoppingDistance = 1f;

        if (data != null)
        {
            agent.speed = data.speed;
            attackRange = data.attackRange;
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(GroundCheckRoutine());
    }

    private void Update()
    {
        if (!isGrounded) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Système de mémoire du joueur
        if (distanceToPlayer <= detectionRange)
        {
            playerDetected = true;
            lastKnownPlayerPosition = player.position;
        }
        else if (distanceToPlayer > forgetRange)
        {
            playerDetected = false;
        }

        if (playerDetected)
        {
            EngagePlayer();
        }
        else if (patrolPoints.Length > 0)
        {
            Patrol();
        }
    }

    private void EngagePlayer()
    {
        float distance = Vector3.Distance(transform.position, lastKnownPlayerPosition);
        
        // Rotation progressive vers le joueur
        Vector3 direction = (lastKnownPlayerPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            TryAttack();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(lastKnownPlayerPosition);
        }

        animator.SetBool("IsMoving", !agent.isStopped);
    }

    private void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        // Délai synchronisé avec l'animation
        yield return new WaitForSeconds(0.4f);

        // Vérification finale avant application des dégâts
        if (Physics.CheckSphere(transform.position + transform.forward, attackRange, playerLayer))
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(data.damage);
                Debug.Log($"Dégâts infligés: {data.damage}");
            }
        }

        isAttacking = false;
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (agent.remainingDistance <= agent.stoppingDistance && !isWaiting)
        {
            StartCoroutine(WaitAtPoint());
        }

        if (!isWaiting)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            animator.SetBool("IsMoving", true);
        }
    }

    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        animator.SetBool("IsMoving", false);
        
        yield return new WaitForSeconds(waitTime);
        
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        isWaiting = false;
    }

    private IEnumerator GroundCheckRoutine()
    {
        while (true)
        {
            isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, 
                                       Vector3.down, 
                                       groundCheckDistance);
            
            if (!isGrounded)
            {
                Debug.LogWarning("Enemy is falling! Attempting recovery...");
                RespawnAtNearestNavMesh();
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void RespawnAtNearestNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            Debug.Log("Enemy respawned at: " + hit.position);
        }
        else
        {
            Debug.LogError("Failed to find valid NavMesh position!");
        }
    }

    // Appelé depuis l'Animation
    public void OnAttackEvent()
    {
        // Pour une synchronisation précise avec les frames d'attaque
        if (Physics.CheckSphere(transform.position + transform.forward * 1.5f, 
                              attackRange, 
                              playerLayer))
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(data.damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Détection
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Attaque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1.5f, attackRange);
        
        // Patrouille
        if (patrolPoints != null)
        {
            Gizmos.color = Color.blue;
            foreach (Transform point in patrolPoints)
            {
                Gizmos.DrawSphere(point.position, 0.3f);
            }
        }
    }
}