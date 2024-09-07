using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IDamageable, IPushable, IHealable, IHealthProvider
{
    [field: SerializeField] public Health Health { get; private set; }

    public event Action Died;

    private Animator _animator;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        Health = new Health();
    }

    public void TakeDamage(int damage)
    {
        if (Health.IsAlive)
        {
            _animator.SetTrigger(PlayerAnimationStrings.HitTrigger);
            Health.Decrease(damage);
            
            if (!Health.IsAlive)
            {
                Die();
            }
        }
    }

    public void Heal(float healPoints)
    {
        Health.Erase(healPoints);
    }

    public void PushOffOnHit(Vector2 knockback)
    {
        if (Health.IsAlive)
        {
            _rigidbody.velocity = new Vector2(knockback.x, _rigidbody.velocity.y + knockback.y);
        }
    }

    private void Die()
    {
        _animator.SetBool(PlayerAnimationStrings.IsAlive, false);
        this.enabled = false;
        Died?.Invoke();
    }
}
