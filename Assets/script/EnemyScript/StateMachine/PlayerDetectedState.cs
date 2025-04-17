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

        if(!enemy.CheckForPlayer())
            enemy.SwitchStates(enemy.patrolState);
        else
        {
            if (Time.time >= enemy.stateTime + enemy.enemyData.playerDetectedWaitTime)
            {

                enemy.SwitchStates(enemy.attackState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
