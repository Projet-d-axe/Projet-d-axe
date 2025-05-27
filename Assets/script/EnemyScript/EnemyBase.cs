using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Microsoft.Win32.SafeHandles;
using System;

public class EnemyBase : MonoBehaviour, iDamageable
{
    [Header("R�f�rences")]
    public EnemyData enemyData;
    public Rigidbody2D rb;
    public Transform edgeDetect;
    public LayerMask ground;
    public LayerMask wall;
    public LayerMask playerLayer;
    public LayerMask damageableLayer;
    private GameObject player;

    public EnemyBaseState currentState;
    public PatrolState patrolState;
    public PlayerDetectedState playerDetectedState;
    public AttackState attackState;



    private NavMeshAgent agent;
    private Collider enemyCollider;

    public float orientX = 1f;
    public float stateTime;
    public bool canAttack = true;


    private void Awake()
    {
        patrolState = new PatrolState(this, "patrol");
        playerDetectedState = new PlayerDetectedState(this, "playerDetected");
        attackState = new AttackState(this, "attacking");
        currentState = patrolState;
        currentState.Enter();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        currentState.LogicUpdate();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        currentState.PhysicsUpdate();
    }

    public bool CheckForObstacles()
    {
        RaycastHit2D hit = Physics2D.Raycast(edgeDetect.position, Vector2.down, enemyData.edgeDetection, ground);
        RaycastHit2D wallHit = Physics2D.Raycast(edgeDetect.position, orientX == 1 ? Vector2.right : Vector2.left, enemyData.wallDetection, wall);
        if (hit.collider == null || wallHit.collider == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckForPlayer()
    {
        RaycastHit2D hitPlayer = Physics2D.Raycast(edgeDetect.position, orientX == 1 ? Vector2.right : Vector2.left, enemyData.detectionRange, playerLayer);

        if (hitPlayer.collider == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SwitchStates(EnemyBaseState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
        stateTime = Time.time;
        
    }

    public void Shoot()
    {
        EnemyProjectile projectile = Instantiate(enemyData.attackObject, transform.position, Quaternion.identity).GetComponent<EnemyProjectile>();
        projectile.InitializeProjectile(player.transform, enemyData.attackSpeed, enemyData.damage);
        Debug.Log("Shooting");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(edgeDetect.position, (orientX == 1 ? Vector2.right : Vector2.left) * enemyData.detectionRange);
    }

    public void Corroutine1()
    {
        StartCoroutine(TurnAfterDelay());
    }
    
    public IEnumerator TurnAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        patrolState.TurnAround();
    }

    public void Detected()
    {
        StartCoroutine(Detect());
    }

    private IEnumerator Detect()
    {
        yield return new WaitForSeconds(enemyData.playerDetectedWaitTime);
        canAttack = true;
    }

    public void Damage(int damageAmount)
    {
        enemyData.pv -= damageAmount;
        Debug.Log(enemyData.pv);

        if (enemyData.pv <= 0)
        {
            Destroy(gameObject);
        }
    }
}
