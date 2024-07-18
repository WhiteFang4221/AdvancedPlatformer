using TMPro;
using UnityEngine;

public class TextHealthBar : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private TextMeshProUGUI _healthText;
    private float _maxHealth;

    private void Awake()
    {
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
        _healthText.text = health + "/" + _maxHealth;
    }

    private void SetMaxHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        _healthText.text = _maxHealth + "/" + _maxHealth;
    }
}
