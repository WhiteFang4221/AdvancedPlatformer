using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]

abstract public class HealthManager : MonoBehaviour
{
    [SerializeField] protected float MaxHealth = 100;
    [SerializeField] protected float CurrentHealth;

    protected Animator Animator;
    protected bool IsAlive = true;

    public Action<int, Vector2> HitTaken;
    public Action<float, float> HealthChanged;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        CurrentHealth = MaxHealth;
    }

    public abstract void TakeDamage(int damage, Vector2 knockback);
}
