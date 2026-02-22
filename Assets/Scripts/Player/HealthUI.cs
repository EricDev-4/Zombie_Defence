using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Color Settings")]
    [SerializeField] private Color nomalColor = Color.white;
    [SerializeField] private Color lowHpColor = Color.red;
    [SerializeField] private int lowHealthThreshold = 40;

    private float maxHealth;

    void Start()
    {
        maxHealth = playerHealth.maxHealth;
    }

    void Update()
    {
        if(playerHealth == null || healthText == null)
            return;

        float currentHealth = playerHealth.currentHealth;
        
        healthText.text = $"{currentHealth + "/" + maxHealth}";

        if(currentHealth <= lowHealthThreshold)
        {
            healthText.color = lowHpColor;
        }
        else
        {
            healthText.color = nomalColor;
        }

    }
}
