using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]

public class SmoothHealthBar : HealthBar
{
    [SerializeField] private Slider _slider;

    private Coroutine _healthBarChangerRoutine;
    private float _healthSpeedChange = 30f;

    public event Action<float, float> HealthSliderChanged;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    protected override void ShowNewValue(float currentHealth, float maxHealth)
    {
        if (_healthBarChangerRoutine != null)
        {
            StopCoroutine(_healthBarChangerRoutine);
        }

        _healthBarChangerRoutine = StartCoroutine(ChangeHealthBarState(currentHealth, maxHealth));
        HealthSliderChanged?.Invoke(_slider.maxValue, currentHealth);
    }

    private IEnumerator ChangeHealthBarState(float currentHealth, float maxHealth)
    {
        float percentHealth = (currentHealth / maxHealth) * MaxPercentHealth;

        while (_slider.value != percentHealth)
        {
            _slider.value = Mathf.MoveTowards(_slider.value, percentHealth, Time.deltaTime * _healthSpeedChange);
            yield return null;
        }
    }
}