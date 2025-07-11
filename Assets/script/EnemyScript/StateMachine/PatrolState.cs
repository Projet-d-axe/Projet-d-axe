using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PatrolState : EnemyBaseState
{
    private int TargetPoint;

    public PatrolState(EnemyBase enemy, string animationName) : base(enemy, animationName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void LogicUpdate()
    {
        if(enemy.enemyData.eType == EnemyType.Melee || enemy.enemyData.eType == EnemyType.Ranged)
        {
            if (enemy.CheckForPlayer())
                enemy.SwitchStates(enemy.playerDetectedState);

            if (enemy.CheckForObstacles())
                TurnAround();
        }
        
        else if (enemy.enemyData.eType == EnemyType.Flying)
        {
            if (enemy.chase)
            {
                enemy.SwitchStates(enemy.playerDetectedState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        if (enemy.enemyData.mType == MovementType.Stationary)
        {
            enemy.Corroutine1();
        }

        else if (enemy.enemyData.mType == MovementType.Random)
        {
            enemy.rb.linearVelocity = new Vector2(enemy.enemyData.speed * enemy.orientX, enemy.rb.linearVelocity.y);
        }

        else if (enemy.enemyData.mType == MovementType.Patrol)
        {
         


            if (enemy.transform.position == enemy.patrolPoints[TargetPoint].position)
            {
                IncreaseTargetInt();
                Debug.Log("I am at my target Location");
            }
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.patrolPoints[TargetPoint].position, enemy.enemyData.speed * Time.deltaTime);
        }
    }


    public void TurnAround()
    {
        enemy.transform.Rotate(0, 180, 0);
        enemy.orientX = enemy.orientX * -1;
        Debug.Log("Flipping");
    }



    // Flying Enemy Logic
    void IncreaseTargetInt()
    {
        TargetPoint++;
        TurnAround();
        if (TargetPoint != enemy.patrolPoints.Length)
        {
            
        }
        else
        {
            TargetPoint = 0;
        }

        Debug.Log($"Number of Patrol Points : {enemy.patrolPoints.Length}. Value of TargetPoint : {TargetPoint}");
    }

    void GoToStartPoint()
    {
        TargetPoint = 0;
    }
}
