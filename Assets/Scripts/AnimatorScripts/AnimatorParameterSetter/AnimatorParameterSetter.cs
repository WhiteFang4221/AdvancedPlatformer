using UnityEngine;

namespace AnimatorParameters
{
    public abstract class AnimatorParameterSetter : MonoBehaviour
    {
        private Health _health;
        private protected Animator _animator;

         private void OnEnable()
        {
            _health.IsAliveChanged += SetIsAlive;
            _health.HitTaken += SetHitTrigger;
        }

        private void OnDisable()
        {
            _health.IsAliveChanged -= SetIsAlive;
            _health.HitTaken -= SetHitTrigger;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _health = GetComponent<Health>();
        }

        private protected abstract void SetIsAlive(bool isAlive);

        private protected abstract void SetHitTrigger ();
    }
}

