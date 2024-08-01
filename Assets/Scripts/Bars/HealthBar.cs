using UnityEngine;

public abstract class HealthBar : MonoBehaviour
{
    [SerializeField] protected HealthManager HealthManager;
    
    protected float MaxPercentHealth = 100;

    private void OnEnable()
    {
        HealthManager.HealthChanged += ShowNewValue;
    }

    private void OnDisable()
    {
        HealthManager.HealthChanged -= ShowNewValue;
    }

    protected abstract void ShowNewValue(float currentHealth, float MaxHealth);
}
