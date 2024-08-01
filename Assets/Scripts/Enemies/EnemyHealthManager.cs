using System.Collections;
using UnityEngine;

public class EnemyHealthManager : HealthManager
{
    public bool IsAlive
    {
        get
        {
            return base.IsAlive;
        }

        private set
        {
            base.IsAlive = value;
            Animator.SetBool(EnemyStringsAnimator.IsAlive, value);
        }
    }

    public override void TakeDamage(int damage, Vector2 knockback)
    {
        if (IsAlive)
        {
            LosingHealth(damage);
            HitTaken?.Invoke(knockback);
            Animator.SetTrigger(EnemyStringsAnimator.HitTrigger);
        }
    }

    public void LosingHealth(int damage)
    {
        currentHealth -= damage;
        HealthChanged?.Invoke(currentHealth, MaxHealth);

        if (currentHealth <= 0)
        {
            IsAlive = false;
        }
    }
}
