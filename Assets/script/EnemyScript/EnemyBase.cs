using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private EnemyData eD;
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyData enemyData;
    private NavMeshAgent agent;
    private Collider enemyCollider;

    

    private void Awake()
    {
        

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
