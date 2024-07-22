using System;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerController))]
public class PlayerHealthManager : HealthManager
{
    [SerializeField] private float _invinsibilityTime = 0.5f;
    [SerializeField] private int _potionHealQuantity = 0;
    [SerializeField] private int _maxPotionQuantity = 3;
    [SerializeField] private int _healEffectPoint = 30;
    [SerializeField] private float _healCooldown = 1f;

    

    private float _healCooldownLeft;
    private float _timeHitLeft = 0;

    private bool IsCanHeal = true;
    private bool _isInvincible;

    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
        private set
        {
            isAlive = value;
            animator.SetBool(PlayerAnimator.IsAlive, value);
        }
    }

    public int PotionHealQuantity
    {
        get
        {
            return _potionHealQuantity;
        }
        private set
        {
            _potionHealQuantity = value;
        }
    }

    public bool IsHealSuccsed
    {
        get
        {
            return animator.GetBool(PlayerAnimator.IsHealSucceeded);
        }
    }

    private void Update()
    {
        if (_isInvincible)
        {
            if (_timeHitLeft >= _invinsibilityTime)
            {
                _isInvincible = false;
                _timeHitLeft = 0;
            }

            _timeHitLeft += Time.deltaTime;
        }

        if (IsHealSuccsed && IsCanHeal)
        {
            Heal();
            HealthChanged?.Invoke(currentHealth, maxHealth);
            IsCanHeal = false;
        }

        if(IsCanHeal == false)
        {
            if (_healCooldownLeft >= _healCooldown)
            {
                IsCanHeal = true;
                _healCooldownLeft = 0;
            }
            else
            {
                _healCooldownLeft += Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out HealPotion potion))
        {
            if (_potionHealQuantity < _maxPotionQuantity)
            {
                _potionHealQuantity++;
                Destroy(potion.gameObject);
            }
        }
    }

    public override void TakeDamage(int damage, Vector2 knockback)
    {
        if (IsAlive && _isInvincible == false)
        {
            currentHealth-=damage;
            _isInvincible = true;
            HitTaken?.Invoke(damage, knockback);
            HealthChanged?.Invoke(currentHealth, maxHealth);
            animator.SetTrigger(PlayerAnimator.HitTrigger);
        }

        if ( currentHealth <= 0)
        {
            IsAlive = false;
        }
    }

    private void Heal()
    {
        _potionHealQuantity--;

        if (currentHealth + _healEffectPoint >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += _healEffectPoint;
        }
    }
}
