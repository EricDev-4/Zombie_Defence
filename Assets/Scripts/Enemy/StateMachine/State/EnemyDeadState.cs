using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    
    public override void Enter(EnemyStateManager sm)
    {
        Debug.Log("Entering IdleState");
    }
    
    public override void Updata(EnemyStateManager sm)
    {
        Debug.Log("Upadting IdleState");
    }
    
    public override void Exit(EnemyStateManager sm)
    {
    }
}
