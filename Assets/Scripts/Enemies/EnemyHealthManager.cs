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
        if (IsAlive )
        {
            CurrentHealth -= damage;
            HitTaken?.Invoke(damage, knockback);
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
            Animator.SetTrigger(EnemyStringsAnimator.HitTrigger);
        }

        if (CurrentHealth <= 0)
        {
            IsAlive = false;
        }
    }
}
