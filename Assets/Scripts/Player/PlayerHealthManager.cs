using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealthManager : HealthManager
{
    [SerializeField] private VampirismDetectionZone _vampirismDetectionZone;
    [SerializeField] private VampireAbility _vampirismAbility;
    [SerializeField] private int _potionHealQuantity = 0;
    [SerializeField] private int _maxPotionQuantity = 3;
    [SerializeField] private int _potionHealEffect = 30;
    [SerializeField] private float _invinsibilityTime = 0.5f;
    [SerializeField] private float _healCooldown = 1f;

    public Action AbilityCancelled;

    private float _healCooldownLeft;
    private float _timeHitLeft = 0;

    private bool IsCanHeal = true;
    private bool _isInvincible = false;

    public new bool IsAlive
    {
        get
        {
            return base.IsAlive;
        }
        private set
        {
            base.IsAlive = value;
            Animator.SetBool(PlayerAnimator.IsAlive, value);
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
            return Animator.GetBool(PlayerAnimator.IsHealSucceeded);
        }
    }

    private void OnEnable()
    {
        _vampirismAbility.HealthStolen += EraseHealth;
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
            PotionHeal();
            HealthChanged?.Invoke(currentHealth, MaxHealth);
            IsCanHeal = false;
        }

        if (IsCanHeal == false)
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
        if (IsAlive && !_isInvincible)
        {
            currentHealth -= damage;
            _isInvincible = true;
            HitTaken?.Invoke(knockback);
            HealthChanged?.Invoke(currentHealth, MaxHealth);
            Animator.SetTrigger(PlayerAnimator.HitTrigger);
        }

        if (currentHealth <= 0)
        {
            IsAlive = false;
        }
    }

    private void PotionHeal()
    {
        _potionHealQuantity--;
        EraseHealth(_potionHealEffect);
    }

    private void EraseHealth(int healEffectPoint)
    {
        if (currentHealth + healEffectPoint >= MaxHealth)
        {
            currentHealth = MaxHealth;
        }
        else
        {
            currentHealth += healEffectPoint;
        }

        HealthChanged?.Invoke(currentHealth, MaxHealth);
    }
}
