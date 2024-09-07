using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMoving), typeof(Animator))]
public class PlayerHealPotion : MonoBehaviour
{
    [SerializeField] private int _maxQuantity = 3;
    [SerializeField] private int EffectPoints = 30;
    [SerializeField] private float _cooldown = 1f;

    public event Action<int> Used;
    private float _cooldownLeft = 0;

    private Player _player;
    private PlayerMoving _playerMoving;
    private Animator _animator;

    private bool _isCanHeal = true;
    private bool _isHealing = false;
    private bool _isHealSuccsed => _animator.GetBool(PlayerAnimationStrings.IsHealSucceeded);

    [field: SerializeField] public int PotionHealQuantity { get; private set; } = 0;


    private void Awake()
    {
        _player = GetComponent<Player>();
        _playerMoving = GetComponent<PlayerMoving>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _playerMoving.HealTried += TryToHeal;
    }

    private void OnDisable()
    {
        _playerMoving.HealTried -= TryToHeal;
    }

    private void Update()
    {
        if (_isHealSuccsed && _isCanHeal)
        {
            _isHealing = false;
            _animator.SetBool(PlayerAnimationStrings.IsHealing, _isHealing);
            _isCanHeal = false;
            SpendPotion();
            _player.Heal(EffectPoints);
        }

        if (_isCanHeal == false)
        {
            if (_cooldownLeft >= _cooldown)
            {
                _isCanHeal = true;
                _cooldownLeft = 0;
            }
            else
            {
                _cooldownLeft += Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out HealPotion potion))
        {
            if (PotionHealQuantity < _maxQuantity)
            {
                PotionHealQuantity++;
                Destroy(potion.gameObject);
            }
        }
    }

    private void SpendPotion()
    {
        PotionHealQuantity--;
        Used?.Invoke(EffectPoints);
    }

    private void TryToHeal()
    {
        if (PotionHealQuantity > 0)
        {
            _isHealing = true;
            _animator.SetBool(PlayerAnimationStrings.IsHealing, _isHealing);
        }
    }
}
