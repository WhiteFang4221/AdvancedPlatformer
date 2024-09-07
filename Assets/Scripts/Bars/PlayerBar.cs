using System;
using System.Collections;
using UnityEngine;

namespace Bars
{
    public class PlayerBar : HealthBar
    {
        private float _healthSpeedChange = 30f;
        private Coroutine _healthBarChangerRoutine;

        public event Action <float, float> HealthSliderChanged;

        protected override void ShowNewValue(float currentHealth, float maxHealth)
        {
            if (_healthBarChangerRoutine != null)
            {
                StopCoroutine(_healthBarChangerRoutine);
            }

            _healthBarChangerRoutine = StartCoroutine(ChangeHealthBarState(currentHealth, maxHealth));
        }

        private IEnumerator ChangeHealthBarState(float currentHealth, float maxHealth)
        {
            float percentHealth = (currentHealth / maxHealth) * MaxPercentHealth;

            while (Slider.value != percentHealth)
            {
                Slider.value = Mathf.MoveTowards(Slider.value, percentHealth, Time.deltaTime * _healthSpeedChange);

                yield return null;
            }

            HealthSliderChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}
