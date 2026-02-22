using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;
    public float currentHealth { get; private set; }
    public bool isDead = false;
    private EnemyStateManager stateManager;

    private void Start()
    {
        stateManager = GetComponent<EnemyStateManager>();
        currentHealth = maxHealth;
    }

    public void Damaged(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        //Debug.Log(GetComponent<GameObject>().name + " : " + currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            stateManager.SwitchState(stateManager.DeadState);
        }
    }
}


