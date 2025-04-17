using UnityEngine;

public class AttackState : EnemyBaseState
{
    private float attackTimer = 0f;

    private bool hasAttacked = false;

    public AttackState(EnemyBase enemy, string animationName) : base(enemy, animationName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        attackTimer = 0f;
        
        hasAttacked = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        attackTimer += Time.deltaTime;

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (enemy.enemyData.eType == EnemyType.Melee)
        {
            if (Time.time >= enemy.stateTime + enemy.enemyData.playerDetectedWaitTime || hasAttacked)
            {
                if (!enemy.CheckForPlayer())
                    enemy.SwitchStates(enemy.patrolState);
                else
                    enemy.SwitchStates(enemy.playerDetectedState);
            }
            else
            {
                ChasePlayer();
            }
        }

        else if (enemy.enemyData.eType == EnemyType.Ranged)
        {
            RangedAttackPlayer();
        }
    }

    void ChasePlayer()
    {
        // Run After Player
        enemy.rb.linearVelocity = new Vector2(enemy.enemyData.speed * enemy.orientX * 2, enemy.rb.linearVelocity.y);

        if (PlayerInRange() && attackTimer >= enemy.enemyData.attackCooldown)
        {
            // If cooldown has passed, attack player
            Debug.Log("Attacking player");
            MeleeAttackPlayer();
            attackTimer = 0f; // Reset the timer after attacking
        }
        else
            Debug.Log("Following Player");
    }

    void MeleeAttackPlayer()
    {
        // Attack Player
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(enemy.transform.position, enemy.enemyData.attackRange, enemy.damageableLayer);

        foreach (Collider2D hitCollider in hitColliders)
        {
            iDamageable damageable = hitCollider.GetComponent<iDamageable>();

            hitCollider.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(enemy.enemyData.knockbackAngle.x * enemy.orientX * enemy.enemyData.knockbackForce, 
                enemy.enemyData.knockbackAngle.y * enemy.enemyData.knockbackForce);
            if (damageable != null)
                damageable.Damage(enemy.enemyData.damage);
        }
        
        hasAttacked = true;
            enemy.SwitchStates(enemy.patrolState);
    }

    void RangedAttackPlayer()
    {
        if (attackTimer >= enemy.enemyData.attackCooldown)
        {
            enemy.rb.linearVelocity = Vector2.zero;
            enemy.Shoot(); // Shoot the projectile or perform ranged attack

            attackTimer = 0f; // Reset the timer after attacking
        }
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
