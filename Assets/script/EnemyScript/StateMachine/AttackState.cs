using UnityEngine;

public class AttackState : EnemyBaseState
{
    public AttackState(EnemyBase enemy, string animationName) : base(enemy, animationName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.enemyData.eType == EnemyType.Melee)
        { 
            ChasePlayer();
        }

        else if (enemy.enemyData.eType == EnemyType.Ranged)
        {
            RangedAttackPlayer();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!enemy.CheckForPlayer())
            enemy.SwitchStates(enemy.patrolState);

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    void ChasePlayer()
    {
        // Run After Player

        if (PlayerInRange())
            MeleeAttackPlayer();
    }

    void MeleeAttackPlayer()
    {
        // Attack Player

    }

    void RangedAttackPlayer()
    {
        enemy.rb.linearVelocity = Vector2.zero;
    }

    private bool PlayerInRange()
    {
        RaycastHit2D playerInRange = Physics2D.Raycast(enemy.transform.position, enemy.transform.forward * enemy.enemyData.attackRange, enemy.playerLayer);
        if (playerInRange.collider == true)
        {
            return true;
        }
        else
        {
            return false;
        } 
    }
}
