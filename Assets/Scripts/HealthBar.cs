using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Image _healthBarFill;

    private Slider _slider;
    private Color _colorFillDefault;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider = GetComponent<Slider>();
        _colorFillDefault = _healthBarFill.color;
        _player.HealthMaxSet += SetMaxHealth;
        _player.HealthChanged += SetHealth;
    }

    private void OnDestroy()
    {
        _player.HealthMaxSet -= SetMaxHealth;
        _player.HealthChanged -= SetHealth;
    }

    private void SetHealth(float health)
    {
        _slider.value = health;
        
        if (health <= 0)
        {
            Color color = _healthBarFill.color;
            color.a = 0f;
            _healthBarFill.color = color;
        }
        else
        {
            _healthBarFill.color = _colorFillDefault;
        }
    }

    private void SetMaxHealth(float maxHealth)
    {
        _slider.maxValue = maxHealth;
        _slider.value = maxHealth;
    }
}
