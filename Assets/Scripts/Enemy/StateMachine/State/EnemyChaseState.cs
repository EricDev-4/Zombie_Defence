using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{   
    private FirstPersonController player;
    public override void Enter(EnemyStateManager sm)
    {
    }
    
    public override void Updata(EnemyStateManager sm)
    {
        sm.agent.SetDestination(sm.target.position);
        sm.animator.SetFloat("MoveSpeed", sm.agent.velocity.magnitude);
    }
    
    public override void Exit(EnemyStateManager sm)
    {
    }
}
