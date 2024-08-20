using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Health : MonoBehaviour
{
    [SerializeField, Min(1)] private int _maxHealth = 100;

    public event Action HitTaken;
    public event Action<float, float> HealthChanged;
    public event Action<bool> IsAliveChanged;

    private bool _isAlive = true;

    [field: SerializeField] public int CurrentHealth { get; private set; }

    private void Start()
    {
        CurrentHealth = _maxHealth;
    }

    public void LosingHealth(int damage)
    {
        if (damage > 0)
        {
            CurrentHealth -= damage;
            HealthChanged?.Invoke(CurrentHealth, _maxHealth);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(damage), "Значение меньше нуля");
        }

        if (CurrentHealth <= 0)
        {
            _isAlive = false;
            IsAliveChanged?.Invoke(_isAlive);
        }
    }

    private void EraseHealth(int healthPoints)
    {
        if (healthPoints > 0)
        {
            if (CurrentHealth + healthPoints > _maxHealth)
            {
                CurrentHealth = _maxHealth;
            }
            else
            {
                CurrentHealth += healthPoints;
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(healthPoints), "Значение меньше нуля");
        }
    }
}
