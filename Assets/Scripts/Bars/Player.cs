using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _maxHealth;

    public event Action<float> HealthChanged;
    public event Action<float> HealthMaxSet;

    public float CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = _maxHealth;
        HealthMaxSet?.Invoke(CurrentHealth);
    }

    public void TakeDamage(float damagePoint)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - damagePoint, 0f, _maxHealth);
        HealthChanged?.Invoke(CurrentHealth);
    }

    public void Heal(float healPoint)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + healPoint, 0f, _maxHealth);
        HealthChanged?.Invoke(CurrentHealth);
    }
}