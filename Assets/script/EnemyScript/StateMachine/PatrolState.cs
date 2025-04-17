using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PatrolState : EnemyBaseState
{
    public PatrolState(EnemyBase enemy, string animationName) : base(enemy, animationName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void LogicUpdate()
    {

        if (enemy.CheckForPlayer())
            enemy.SwitchStates(enemy.playerDetectedState);
        if (enemy.CheckForObstacles())
        {
            TurnAround();
        }
    }

    public override void PhysicsUpdate()
    {
        if (enemy.enemyData.mType == MovementType.Stationary)
        {
            enemy.Corroutine1();
        }

        else
        {
            enemy.rb.linearVelocity = new Vector2(enemy.enemyData.speed * enemy.orientX, enemy.rb.linearVelocity.y);
        }
        
    }

    

    public void TurnAround()
    {
        enemy.transform.Rotate(0, 180, 0);
        enemy.orientX = enemy.orientX * -1;

    }
}
