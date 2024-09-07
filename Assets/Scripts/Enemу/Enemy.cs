using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable, IPushable, IHealthProvider, IVampirable
{
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private EnemyMoving _enemyMoving;
    private WaitForSeconds _vampireTick = new WaitForSeconds(1);
    private Coroutine _vampirismCoroutine;

    public event Action Died;
    [field: SerializeField] public Health Health { get; private set; }

    public bool IsVampireTarget { get; private set; } = false;



    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _enemyMoving = GetComponent<EnemyMoving>();
        Health = new Health();
    }

    public void TakeDamage(int damage)
    {
        if (Health.IsAlive)
        {
            _animator.SetTrigger(EnemyAnimationStrings.HitTrigger);
            Health.Decrease(damage);

            if (!Health.IsAlive)
            {
                Die();
            }
        }
    }

    public void PushOffOnHit(Vector2 knockback)
    {
        if (Health.IsAlive)
        {
            _rigidbody.velocity = new Vector2(knockback.x, _rigidbody.velocity.y + knockback.y);
        }
    }

    public void ReactToVampirism(float damage)
    {
        IsVampireTarget = true;
        _enemyMoving.GettingStuck();
        StartLoseHealth(damage);
    }

    public void GetOutVampirism()
    {
        IsVampireTarget = false;
        _enemyMoving.GetOutStuck();
        StopLoseHealth();
    }

    private IEnumerator LoseHealthCoroutine(float damage)
    {
        while (IsVampireTarget)
        {
            Health.Decrease(damage);
            yield return _vampireTick;
        }
    }

    private void StartLoseHealth(float damage)
    {
        if (_vampirismCoroutine != null)
        {
            StopCoroutine(_vampirismCoroutine);
        }

        _vampirismCoroutine = StartCoroutine(LoseHealthCoroutine(damage));
    }

    private void StopLoseHealth()
    {
        if (_vampirismCoroutine != null)
        {
            StopCoroutine(_vampirismCoroutine);
            _vampirismCoroutine = null;
        }
    }

    private void Die()
    {
        _animator.SetBool(EnemyAnimationStrings.IsAlive, false);
        this.enabled = false;
        Died?.Invoke();
    }
}
