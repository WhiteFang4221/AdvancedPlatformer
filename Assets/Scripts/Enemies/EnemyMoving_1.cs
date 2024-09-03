
namespace HealthSystem
{
    public class EnemyHealth : Health
    {
        public override void TakeDamage(int damage)
        {
            if (_isAlive && !_isinvincible)
            {
                _animator.SetTrigger(EnemyAnimationStrings.HitTrigger);
                base.TakeDamage(damage);
            }
        }

        private protected override void CheckIsAlive()
        {
            base.CheckIsAlive();
            _animator.SetBool(EnemyAnimationStrings.IsAlive, _isAlive);
        }
    }
}

