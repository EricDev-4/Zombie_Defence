using System.Collections;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    PlayerHealth playerHealth;
    public bool isAttacking = false;
    private float attackDuration = 0.5f;
    private float attackTimer = 0f;
    
    public override void Enter(EnemyStateManager sm)
    {
        sm.time = sm.attackSpeed;
        playerHealth = sm.target.GetComponentInChildren<PlayerHealth>();
    }
    
    public override void Update(EnemyStateManager sm)
    {
        
        sm.time += Time.deltaTime;

        if(isAttacking)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer > attackDuration)
            {
                attackTimer = 0f;
                isAttacking = false;
            }
        }

        if(sm.time > sm.attackSpeed)
        {
            sm.animator.SetTrigger("Attack");
            isAttacking = true;
            attackTimer = 0f;
            sm.time = 0f;
        }
    }
    public override void Exit(EnemyStateManager sm)
    {
        // sm.animator.ResetTrigger("Attack");
    }

    public void Attack(EnemyStateManager sm)
    {
        Debug.Log("Attack!");
        float damage = sm.damage;
        
        if(playerHealth != null)
            playerHealth.GetDamage(damage);
    }
    
}
