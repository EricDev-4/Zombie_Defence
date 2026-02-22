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
    public CapsuleCollider[] handCol;
    public Rigidbody rb;
    public NavMeshAgent agent;
    public Animator animator;

    public EnemyHealth enemyHealth;

    [SerializeField] private float attackDistance = 3f;
    public float attackSpeed = 1.0f;
    public float damage = 10f;

    private bool _destroyed;

    public float time = 0f;


    private void Start()
    {
        target = FindAnyObjectByType <FirstPersonController>().GetComponent<Transform>();
        enemyHealth = GetComponent<EnemyHealth>();
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
        currentState.Update(this);

        float currentDistance = Vector3.Distance(transform.position , target.transform.position);

        if(currentDistance <= attackDistance && !enemyHealth.isDead && currentState != AttackState)
        {
            SwitchState(AttackState);
        }
        else if(currentDistance > attackDistance && !enemyHealth.isDead && currentState != ChaseState)
        {
            SwitchState(ChaseState);
        }
    }
    void Update()
    {
        time += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(currentState == AttackState && !AttackState.isAttacking)
        {
            AttackState.Attack(this);
            Debug.Log("공격!");
        }
    }

    private void OnDestroy()
    {
        _destroyed = true;
    }

    public void SwitchState(EnemyBaseState sm)
    {
        currentState.Exit(this);
        currentState = sm;
        currentState.Enter(this);
    }


}
