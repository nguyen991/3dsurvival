using UnityEngine;
using UnityEngine.Events;

public class DamageBody : MonoBehaviour
{
    [SerializeField]
    private float health = 10;

    public UnityEvent onDamage;
    public UnityEvent onDeath;

    private float currentHealth;

    private void OnEnable()
    {
        currentHealth = health;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            onDeath.Invoke();
        }
        else
        {
            onDamage.Invoke();
        }
    }
}
