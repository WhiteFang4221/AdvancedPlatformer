using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]

abstract public class HealthManager : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] protected float currentHealth;

    public Action<int, Vector2> HitTaken;
    public Action<float, float> HealthChanged;

    protected Animator animator;
    protected bool isAlive = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public abstract void TakeDamage(int damage, Vector2 knockback);
}
