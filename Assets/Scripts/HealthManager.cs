using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]

abstract public class HealthManager : MonoBehaviour
{
    [SerializeField] protected float MaxHealth = 100;
    [SerializeField] protected float currentHealth;

    protected Animator Animator;
    protected bool IsAlive = true;

    public float CurrentHealth => currentHealth;

    public Action<Vector2> HitTaken;
    public Action<float, float> HealthChanged;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        currentHealth = MaxHealth;
    }

    public abstract void TakeDamage(int damage, Vector2 knockback);
}
