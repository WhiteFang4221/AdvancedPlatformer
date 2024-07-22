using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class DefaultHealthBar : HealthBar
{
    private Slider _slider;
    private WaitForSeconds _timeToDestroyBar = new WaitForSeconds(1);

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    protected override void ShowNewValue(float currentHealth, float maxHealth)
    {
        float percentHealth = (currentHealth/ maxHealth) * maxPercentHealth;
        _slider.value = percentHealth;

        if (_slider.value <= 0)
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
