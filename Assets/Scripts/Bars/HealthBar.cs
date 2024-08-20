using UnityEngine;

public abstract class HealthBar : MonoBehaviour
{
    [SerializeField] protected Health Health;
    
    protected float MaxPercentHealth = 100;

    private void OnEnable()
    {
        Health.HealthChanged += ShowNewValue;
    }

    private void OnDisable()
    {
        Health.HealthChanged -= ShowNewValue;
    }

    protected abstract void ShowNewValue(float currentHealth, float MaxHealth);
}
