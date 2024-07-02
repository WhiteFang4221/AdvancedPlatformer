using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 10000;
    [SerializeField] private float _invinsibilityTime = 0.5f;

    private Animator _animator;
    private int _currentHealth;
    private float _timeHitLeft = 0;

    private bool _isAlive;
    private bool _isInvincible;

    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }

        private set
        {
            _isAlive = value;
            _animator.SetBool(PlayerAnimator.IsAlive, value);
        }
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _currentHealth = _maxHealth;
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
    }

    public void TakeDamage(int damage)
    {
        if (IsAlive && _isInvincible == false)
        {
            _currentHealth-=damage;
            _isInvincible = true;
        }

        if ( _currentHealth <= 0)
        {
            IsAlive = false;
        }
    }
}
