using UnityEngine;

public class EnemyBaseState
{
    protected EnemyBase enemy;
    protected string animationName;

    public EnemyBaseState(EnemyBase enemy, string animationName)
    {
        this.enemy = enemy;
        this.animationName = animationName;
    }

    public virtual void Enter()
    {
        Debug.Log("Entered" + animationName);
    }

    public virtual void Exit()
    {

    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {

    }
}
