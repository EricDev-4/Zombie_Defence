using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateManager : MonoBehaviour
{
    EnemyBaseState currentState;
    public EnemyChaseState ChaseState  = new EnemyChaseState();
    public EnemyAttackState AttackState  = new EnemyAttackState();
    public EnemyDeadState DeadState  = new EnemyDeadState();

    public Transform target;
    public Collider col;
    public Rigidbody rb;
    public NavMeshAgent agent;
    public Animator animator;

    private bool _destroyed;

    private void Start()
    {
        target = FindAnyObjectByType <FirstPersonController>().GetComponent<Transform>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        currentState = ChaseState;

        // currentState.Enter(this);
    }

    private void FixedUpdate()
    {
        if (_destroyed) return;
        currentState.Updata(this);
    }

    private void OnDestroy()
    {
        _destroyed = true;
    }

    public void SwitchState(EnemyBaseState sm)
    {
        currentState = sm;
        currentState.Enter(this);
    }
}
