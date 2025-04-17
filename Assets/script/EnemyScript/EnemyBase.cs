using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Microsoft.Win32.SafeHandles;

public class EnemyBase : MonoBehaviour
{
    [Header("Références")]
    public EnemyData enemyData;
    public Rigidbody2D rb;
    public Transform edgeDetect;
    public LayerMask ground;
    public LayerMask wall;
    public LayerMask playerLayer;

    public EnemyBaseState currentState;
    public PatrolState patrolState;
    public PlayerDetectedState playerDetectedState;
    public AttackState attackState;



    private NavMeshAgent agent;
    private Collider enemyCollider;

    public float orientX = 1f;
    public float stateTime;



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

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(edgeDetect.position, (orientX == 1 ? Vector2.right : Vector2.left) * enemyData.detectionRange);
    }
}
