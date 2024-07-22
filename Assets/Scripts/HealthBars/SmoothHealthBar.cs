using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]

public class SmoothHealthBar : HealthBar
{
    [SerializeField] private Slider _slider;

    public event Action<float, float> HealthSliderChanged;

    private Coroutine _healthBarChangerRoutine;
    private float _healthSpeedChange = 30f;

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
        float _percentHealth = (currentHealth / maxHealth) * maxPercentHealth;

        while (_slider.value != _percentHealth)
        {
            _slider.value = Mathf.MoveTowards(_slider.value, _percentHealth, Time.deltaTime * _healthSpeedChange);
            yield return null;
        }
    }
}