using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMoving), typeof(Animator), typeof(Health))]
public class PlayerHealPotion : MonoBehaviour
{
    [SerializeField] private int _maxPotionQuantity = 3;
    [SerializeField] private int _potionHealEffect = 30;
    [SerializeField] private float _healCooldown = 1f;

    public event Action<int> PotionUsed;
    private float _healCooldownLeft = 0;

    private Health _health;
    private PlayerMoving _playerMoving;
    private Animator _animator;

    private bool _isCanHeal = true;
    private bool _isHealing = false;

    [field: SerializeField] public int PotionHealQuantity { get; private set; } = 0;


    public bool IsHealing
    {
        get
        {
            return _isHealing;
        }
        private set
        {
            _isHealing = value;
            _animator.SetBool(PlayerAnimationStrings.IsHealing, true);
        }
    }

    public bool IsHealSuccsed => _animator.GetBool(PlayerAnimationStrings.IsHealSucceeded);

    private void Awake()
    {
        _health = GetComponent<Health>();
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
        if (IsHealSuccsed && _isCanHeal)
        {
            SpendPotion();
            _isCanHeal = false;
        }

        if (_isCanHeal == false)
        {
            if (_healCooldownLeft >= _healCooldown)
            {
                _isCanHeal = true;
                _healCooldownLeft = 0;
            }
            else
            {
                _healCooldownLeft += Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out HealPotion potion))
        {
            if (PotionHealQuantity < _maxPotionQuantity)
            {
                PotionHealQuantity++;
                Destroy(potion.gameObject);
            }
        }
    }

    private void SpendPotion()
    {
        PotionHealQuantity--;
        PotionUsed?.Invoke(_potionHealEffect);
    }

    private void TryToHeal()
    {
        IsHealing = true;
    }
}
