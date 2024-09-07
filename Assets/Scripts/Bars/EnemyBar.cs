using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Bars
{
    [RequireComponent(typeof(Slider))]
    public class EnemyBar : HealthBar
    {
        private WaitForSeconds _timeToDestroyBar = new WaitForSeconds(1);

        protected override void ShowNewValue(float currentHealth, float maxHealth)
        {
            float percentHealth = (currentHealth / maxHealth) * MaxPercentHealth;
            Slider.value = percentHealth;

            if (Slider.value <= 0)
            {
                StartCoroutine(DestroyHealthBar());
            }
        }

        private IEnumerator DestroyHealthBar()
        {
            yield return _timeToDestroyBar;
            Destroy(gameObject);
        }
    }
}

