using UnityEngine;
using UnityEngine.UI;

namespace Bars
{
    [RequireComponent(typeof(Slider))]
    public abstract class HealthBar : MonoBehaviour
    {
        private IHealth _health;
        private protected Slider Slider;
        protected float MaxPercentHealth = 100;

        private void Awake()
        {
            Slider = GetComponent<Slider>();
        }


        private void OnDisable()
        {
            _health.Changed -= ShowNewValue;
        }

        public virtual void Init(IHealth health)
        {
            _health = health;
            _health.Changed += ShowNewValue;
        }

        protected abstract void ShowNewValue(float currentHealth, float MaxHealth); 
    }
}
