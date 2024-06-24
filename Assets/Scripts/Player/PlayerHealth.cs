using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 10000;

    public Action PlayerDied;
    public Action PlayerHit;

    private int _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth-=damage;
        PlayerHit.Invoke();

        if ( _currentHealth <= 0)
        {
            PlayerDied.Invoke();
        }
    }

    public void Heal(int healPoints)
    {
        _currentHealth+=healPoints;

        if (_currentHealth > _maxHealth)
        {
            _currentHealth=_maxHealth;
        }
    }
}
