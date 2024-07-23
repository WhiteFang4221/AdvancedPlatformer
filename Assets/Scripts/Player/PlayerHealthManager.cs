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
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
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
        if (IsAlive && !_isInvincible)
        {
            CurrentHealth-=damage;
            _isInvincible = true;
            HitTaken?.Invoke(damage, knockback);
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
            Animator.SetTrigger(PlayerAnimator.HitTrigger);
        }

        if ( CurrentHealth <= 0)
        {
            IsAlive = false;
        }
    }

    private void Heal()
    {
        _potionHealQuantity--;

        if (CurrentHealth + _healEffectPoint >= MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        else
        {
            CurrentHealth += _healEffectPoint;
        }
    }
}
