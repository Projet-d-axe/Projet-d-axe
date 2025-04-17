using UnityEngine;

public class PlayerDetectedState : EnemyBaseState
{
    public PlayerDetectedState(EnemyBase enemy, string animationName) : base(enemy, animationName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        enemy.rb.linearVelocity = Vector2.zero;

    }

    public override void Exit()
    {

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();



        if (Time.time >= enemy.stateTime + enemy.enemyData.playerDetectedWaitTime)
        {
            if (enemy.CheckForPlayer())
            {
                enemy.SwitchStates(enemy.attackState); // Switch to attack after delay
            }
            else
            {
                enemy.SwitchStates(enemy.patrolState); // Player lost, go back to patrol
            }
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
