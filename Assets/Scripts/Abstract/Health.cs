using System;
using System.Collections;
using UnityEngine;

namespace HealthSystem
{
    [RequireComponent(typeof(Animator))]

    public abstract class Health : MonoBehaviour
    {
        [SerializeField, Min(1)] private protected int _maxHealth = 100;
        private protected Animator _animator;

        public event Action<float, float> HealthChanged;

        private protected bool _isAlive = true;
        private protected bool _isinvincible = false;

        private WaitForSeconds _invincibleCooldown = new WaitForSeconds(0.5f);

        [field: SerializeField] public int CurrentHealth { get; private set; }

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            CurrentHealth = _maxHealth;
        }

        public virtual void TakeDamage(int damage)
        {
            LosingHealth(damage);
            StartCoroutine(IsInvincibleReload());
        }


        public void LosingHealth(int damage)
        {
            if (damage > 0)
            {
                CurrentHealth -= damage;
                HealthChanged?.Invoke(CurrentHealth, _maxHealth);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "Значение меньше нуля");
            }

            CheckIsAlive();
        }

        private protected virtual void CheckIsAlive()
        {
            if (CurrentHealth <= 0)
            {
                _isAlive = false;
            }
        }

        public void EraseHealth(int healthPoints)
        {
            if (healthPoints > 0)
            {
                if (CurrentHealth + healthPoints > _maxHealth)
                {
                    CurrentHealth = _maxHealth;
                }
                else
                {
                    CurrentHealth += healthPoints;
                }

                HealthChanged?.Invoke(CurrentHealth, _maxHealth);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(healthPoints), "Значение меньше нуля");
            }
        }

        private protected virtual void Initialize()
        {
            _animator = GetComponent<Animator>();
        }

        private IEnumerator IsInvincibleReload()
        {
            _isinvincible = true;
            yield return _invincibleCooldown;
            _isinvincible = false;
        }
    }
}


