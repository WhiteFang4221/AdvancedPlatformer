using System;

namespace HealthSystem
{
    public class PlayerHealth : Health
    {
        public event Action GotHit;

        private PlayerHealPotion _healPotion;

        private void OnEnable()
        {
            _healPotion.PotionUsed += EraseHealth;
        }

        private void OnDisable()
        {
            _healPotion.PotionUsed -= EraseHealth;
        }

        public override void TakeDamage(int damage)
        {
            if(_isAlive && !_isinvincible)
            {
                GotHit?.Invoke();
                _animator.SetTrigger(PlayerAnimationStrings.HitTrigger);
                base.TakeDamage(damage);
            }
        }

        private protected override void CheckIsAlive()
        {
            base.CheckIsAlive();
            _animator.SetBool(PlayerAnimationStrings.IsAlive, _isAlive);
        }

        private protected override void Initialize()
        {
            base.Initialize();
            _healPotion = GetComponent<PlayerHealPotion>(); 
        }
    }
}
