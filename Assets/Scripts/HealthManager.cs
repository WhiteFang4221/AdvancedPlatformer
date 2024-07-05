using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]

abstract public class HealthManager : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 10000;
    [SerializeField] protected int currentHealth;

    public Action<int, Vector2> HitTaken;

    protected Animator animator;
    protected bool isAlive = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public abstract void TakeDamage(int damage, Vector2 knockback);
}
