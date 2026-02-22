using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;
    [SerializeField] float invincibilityTime = 1.5f;
    public bool invincibility = false;
    private float lastAttackTime = 0f;

    Animator animator;

    public bool isDie = false;

    private void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        lastAttackTime += Time.deltaTime;

        if (invincibility && lastAttackTime >= invincibilityTime)
        {
            invincibility = false;
        }
    }
    public void GetDamage(float damage)
    {
        if(invincibility || isDie) return;


        if(lastAttackTime >= invincibilityTime)
        {
            invincibility = true;
            currentHealth -= damage;
            lastAttackTime = 0f;
            Debug.Log("CurrentHealth : " + currentHealth);
            if(currentHealth < 0)
            {
                isDie = true;
            }
        }
    }
}
