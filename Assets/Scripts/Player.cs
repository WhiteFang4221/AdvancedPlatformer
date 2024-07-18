using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _maxHealth;

    public event Action<float> HealthChanged;
    public event Action<float> HealthMaxSet;

    private float _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
        HealthMaxSet?.Invoke(_currentHealth);
    }

    public void TakeDamage(float damagePoint)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damagePoint, 0f, _maxHealth);
        HealthChanged?.Invoke(_currentHealth);
    }

    public void Heal(float healPoint)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + healPoint, 0f, _maxHealth);
        HealthChanged?.Invoke(_currentHealth);
    }
}