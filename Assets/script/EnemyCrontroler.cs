using UnityEngine;
using System.Collections;
using System;

public class EnemyController : MonoBehaviour
{
    [Header("Configuration")]
    public EnemyData data;
    public Transform[] patrolPoints;
    public float respawnHeightThreshold = -10f;
    public float groundCheckDistance = 0.2f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private Vector3 spawnPosition;
    private int currentPatrolIndex = 0;
    private bool isWaiting = false;
    private bool isGrounded = true;
    private float fallTimer = 0f;
    private const float maxFallTime = 2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spawnPosition = transform.position;
    }

    private void Start()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        InitializePatrol();
    }

    private void InitializePatrol()
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        if (!isGrounded)
        {
            HandleFalling();
            return;
        }

        UpdateAIBehavior();
        CheckGroundStatus();
    }

    private void FixedUpdate()
    {
        // Applique une gravité manuelle si nécessaire
        if (!isGrounded && rb)
        {
            rb.AddForce(Physics.gravity * rb.mass * 2f); // Chute plus rapide
        }
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );
    }

    private void HandleFalling()
    {
        fallTimer += Time.deltaTime;
        if (fallTimer > maxFallTime || transform.position.y < respawnHeightThreshold)
        {
            RespawnEnemy();
        }
    }

    private void UpdateAIBehavior()
    {
        if (!player) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= data.detectionRange)
        {
            ChasePlayer(distanceToPlayer);
        }
        else if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Patrol();
        }
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Ignore la différence de hauteur

        // Déplacement vers le joueur
        transform.position += direction * data.moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction); // Rotation fluide

        animator.SetBool("IsMoving", true);

        if (distanceToPlayer <= data.attackRange)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (Time.time >= data.lastAttackTime + data.attackCooldown)
        {
            StartCoroutine(AttackSequence());
            data.lastAttackTime = Time.time;
        }
    }

    private IEnumerator AttackSequence()
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.4f); // Sync avec l'animation

        if (player && Vector3.Distance(transform.position, player.position) <= data.attackRange * 1.2f)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(data.damage);
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPatrolIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        // Déplacement vers le point de patrouille
        transform.position += direction * data.moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);

        animator.SetBool("IsMoving", true);

        if (Vector3.Distance(transform.position, target.position) <= 0.5f)
        {
            if (!isWaiting)
            {
                StartCoroutine(WaitAndMoveToNextPoint());
            }
        }
    }

    private IEnumerator WaitAndMoveToNextPoint()
    {
        isWaiting = true;
        animator.SetBool("IsMoving", false);

        yield return new WaitForSeconds(data.patrolWaitTime);

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        isWaiting = false;
    }

    private void RespawnEnemy()
    {
        transform.position = spawnPosition;
        isGrounded = true;
        fallTimer = 0f;
        currentPatrolIndex = 0;

        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void OnDeath()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    // Appelé depuis les animations
    public void OnAttackHitFrame()
    {
        if (player && Vector3.Distance(transform.position, player.position) <= data.attackRange)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(data.damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (data != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, data.attackRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data.detectionRange);
        }

        if (patrolPoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.25f);
                }
            }
        }
    }

    internal void ApplyPlatformEffect(float platformDuration, float platformSizeMultiplier, float speedReduction, Color platformColor)
    {
        throw new NotImplementedException();
    }
}