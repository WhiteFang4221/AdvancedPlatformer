using UnityEngine;

[RequireComponent (typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected int _damage;
    [SerializeField] private int _maxHealth;

    private Animator _animator;

    private int _health;
    private bool _isDead = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _health = _maxHealth;
    }

    private void TakeDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            _isDead = true;
        }
    }


}
