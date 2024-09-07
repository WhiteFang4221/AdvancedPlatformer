using System;
using UnityEngine;

[System.Serializable]
public class Health : IHealth
{
    public event Action<float, float> Changed;

    [field: SerializeField] public float CurrentValue { get; private set; }
    [field: SerializeField] public float MaxValue { get; private set; } = 100;
    public bool IsAlive { get; private set; } = true;

    public Health()
    {
        CurrentValue = MaxValue;
    }

    public void Decrease(float damage)
    {
        if (damage > 0)
        {
            CurrentValue -= damage;

            if (CurrentValue <= 0)
            {
                IsAlive = false;
            }

            Changed?.Invoke(CurrentValue, MaxValue);
        }
    }

    public void Erase(float healthPoints)
    {
        if (IsAlive)
        {
            if (healthPoints > 0)
            {
                if (CurrentValue + healthPoints > MaxValue)
                {
                    CurrentValue = MaxValue;
                }
                else
                {
                    CurrentValue += healthPoints;
                }

                Changed?.Invoke(CurrentValue, MaxValue);
            }
        }
    }
}



