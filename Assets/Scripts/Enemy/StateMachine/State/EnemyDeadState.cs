using UnityEngine;


public class EnemyDeadState : EnemyBaseState
{
    private float DistoryTime = 10f;
    private float timer = 0;
    
    public override void Enter(EnemyStateManager sm)
    {
        sm.agent.isStopped = true;
        sm.animator.SetTrigger("IsDead");
        sm.col.enabled = false;
        int randomDeadNumber = Random.Range(0, 2);
        sm.animator.SetInteger("DeadNum", randomDeadNumber);
    }
    
    public override void Update(EnemyStateManager sm)
    {
        timer += Time.deltaTime;
        if(timer >= DistoryTime)
        {
            if (sm.gameObject != null)
            {
                sm.gameObject.SetActive(false);
                timer = 0;
            }
                
        }
    }
    
    public override void Exit(EnemyStateManager sm)
    {
    }
}
