using UnityEngine;

public class EnemyHealthManager : HealthManager
{
    public bool IsAlive
    {
        get
        {
            return isAlive;
        }

        private set
        {
            isAlive = value;
            animator.SetBool(EnemyStringsAnimator.IsAlive, value);
        }
    }

    public override void TakeDamage(int damage, Vector2 knockback)
    {
        if (IsAlive )
        {
            currentHealth -= damage;
            HitTaken?.Invoke(damage, knockback);
            HealthChanged?.Invoke(currentHealth, maxHealth);
            animator.SetTrigger(EnemyStringsAnimator.HitTrigger);
        }

        if (currentHealth <= 0)
        {
            IsAlive = false;
        }
    }
}
