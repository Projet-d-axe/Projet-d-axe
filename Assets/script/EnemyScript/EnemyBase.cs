using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Microsoft.Win32.SafeHandles;
using System;

public class EnemyBase : MonoBehaviour, iDamageable
{
    [Header("Références")]
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

    //used for enemies with reaction zones
    public bool chase;

    private NavMeshAgent agent;
    private Collider enemyCollider;

    public float orientX = 1f;
    public float stateTime;
    public bool canAttack = true;

    public Transform[] patrolPoints;
    public GameObject chaseArea;
    


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

        if (enemyData.eType == EnemyType.Flying)
        {
            ChaseArea cA = Instantiate(chaseArea, transform.position, Quaternion.identity).GetComponent<ChaseArea>();
            cA.transform.parent = gameObject.transform;
            cA.SetRadiusToDetectionRange(enemyData.detectionRange);
        }
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
        if (enemyData.eType != EnemyType.Flying)
        {
            RaycastHit2D hit = Physics2D.Raycast(edgeDetect.position, Vector2.down, enemyData.edgeDetection, ground);
            RaycastHit2D wallHit = Physics2D.Raycast(edgeDetect.position, orientX == 1 ? Vector2.right : Vector2.left, enemyData.wallDetection, wall);
            if (hit.collider == null || wallHit.collider == true)
            {
                Debug.Log($"hit collider response : {hit.collider}");
                return true;
            }
            else
            {
                return false;
            }
        }

        else
        {
            return false ;
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
