using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Microsoft.Win32.SafeHandles;

public class EnemyBase : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform edgeDetect;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask wall;

    private NavMeshAgent agent;
    private Collider enemyCollider;

    private float orientX = 1f;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(edgeDetect.position, Vector2.down, enemyData.edgeDetection, ground);
        RaycastHit2D wallRightHit = Physics2D.Raycast(edgeDetect.position, Vector2.right, enemyData.wallDetection, wall);
        RaycastHit2D wallLeftHit = Physics2D.Raycast(edgeDetect.position, Vector2.left, enemyData.wallDetection, wall);

        if (hit.collider == null || wallRightHit.collider == true || wallLeftHit == true)
        {
            Debug.Log("Ground Not Found");
            TurnAround();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(enemyData.speed * orientX, rb.linearVelocity.y);
    }


    void TurnAround()
    {
        transform.Rotate(0, 180, 0);
        Debug.Log("Turning Around");
        orientX = orientX * -1;

    }
}
