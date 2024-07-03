using System;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 10000;
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _invinsibilityTime = 0.5f;

    public Action <int, Vector2> HitTaken;

    private Animator _animator;


    private bool _isAlive = true;

    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }

        private set
        {
            _isAlive = value;
            _animator.SetBool(EnemyStringsAnimator.IsAlive, value);
        }
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (IsAlive )
        {
            _currentHealth -= damage;
            HitTaken?.Invoke(damage, knockback);
            _animator.SetTrigger(EnemyStringsAnimator.HitTrigger);
        }

        if (_currentHealth <= 0)
        {
            IsAlive = false;
        }
    }
}
