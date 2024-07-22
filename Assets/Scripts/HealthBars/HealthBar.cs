using UnityEngine;

public abstract class HealthBar : MonoBehaviour
{
    [SerializeField] protected HealthManager healthManager;
    
    protected float maxPercentHealth = 100;

    private void OnEnable()
    {
        healthManager.HealthChanged += ShowNewValue;
    }

    private void OnDisable()
    {
        healthManager.HealthChanged -= ShowNewValue;
    }

    protected abstract void ShowNewValue(float currentHealth, float MaxHealth);
}
