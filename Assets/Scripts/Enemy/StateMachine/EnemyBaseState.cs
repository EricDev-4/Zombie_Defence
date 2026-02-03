using UnityEngine;

public abstract class EnemyBaseState
{
    public abstract void Enter(EnemyStateManager sm);      // 상태 시작
    public abstract void Exit(EnemyStateManager sm);       // 상태 종료
    public abstract void Updata(EnemyStateManager sm);     // 매 프레임
}
