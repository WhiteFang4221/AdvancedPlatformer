using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerMoving), typeof(Player))]
public class VampireAbility : MonoBehaviour
{
    [SerializeField] private VampirismAura _vampirismAura;
    [SerializeField] private float _timeAbility = 6f;
    [SerializeField] private int _abilityDamage = 5;
    [SerializeField] private float _cooldownAbility = 10f;

    public event Action<float> Activated;

    private Animator _animator;
    private Player _player;
    private PlayerMoving _playerMoving;

    private Coroutine _abilityCoroutine;
    private Coroutine _reloadAbilityCoroutine;
    private Coroutine _vampireCoroutine;
    private WaitForSeconds _timeTickHealth = new WaitForSeconds(1);

    public float TimeLeft { get; private set; } = 0;
    public bool IsCanUseAbility { get; private set; } = true;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMoving = GetComponent<PlayerMoving>();
        _player = GetComponent<Player>();   
    }

    private void OnEnable()
    {
        _playerMoving.VimpireAbilityUsed += UseAbility;        
        _vampirismAura.Catched += StartStealHealth;
    }

    private void OnDisable()
    {
        _playerMoving.VimpireAbilityUsed -= UseAbility;
        _vampirismAura.Catched -= StartStealHealth;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        _playerMoving.VimpireAbilityUsed -= UseAbility;
    }

    private void UseAbility()
    {
        if (!IsCanUseAbility)
        {
            return;
        }

        _animator.SetTrigger(PlayerAnimationStrings.VampireTrigger);
        StartAbilityCoroutine();
    }

    private IEnumerator AbilityCoroutine()
    {
        while (!_animator.GetBool(PlayerAnimationStrings.IsVampirismActive))
        {
            yield return null;
        }

        IsCanUseAbility = false;
        Activated?.Invoke(_timeAbility);
        _vampirismAura.gameObject.SetActive(true);
        TimeLeft = _timeAbility;

        while (TimeLeft >= 0)
        {
            TimeLeft -= Time.deltaTime;

            if (_animator.GetBool(PlayerAnimationStrings.IsVampirismBroken))
            {
                CancelAbility();
            }

            yield return null;
        }

        StopAbilityCoroutine();
    }

    private IEnumerator ReloadAbility()
    {
        TimeLeft = 0;
        Activated?.Invoke(_cooldownAbility);
        _animator.SetBool(PlayerAnimationStrings.IsVampirismFinish, true);
        _vampirismAura.gameObject.SetActive(false);

        while (TimeLeft <= _cooldownAbility)
        {
            TimeLeft += Time.deltaTime;
            yield return null;
        }

        TimeLeft = 0;
        IsCanUseAbility = true;
    }

    private void StartAbilityCoroutine()
    {
        if (_abilityCoroutine != null)
        {
            StopCoroutine(_abilityCoroutine);
        }
        _abilityCoroutine = StartCoroutine(AbilityCoroutine());
    }

    private void StopAbilityCoroutine()
    {
        if (_abilityCoroutine != null)
        {
            StopCoroutine(_abilityCoroutine);
            _abilityCoroutine = null;

            if (_reloadAbilityCoroutine != null)
            {
                StopCoroutine(_reloadAbilityCoroutine);
            }

            _reloadAbilityCoroutine = StartCoroutine(ReloadAbility());
        }
    }

    private void CancelAbility()
    {
        StopAbilityCoroutine();
    }

    private IEnumerator StealHealthCoroutine(float healthPoints)
    {
        while (_vampirismAura.DetectedColliders.Count != 0)
        {
            _player.Heal(healthPoints);
            yield return _timeTickHealth;
        }
    }

    private void StartStealHealth(float healthPoints)
    {
        if (_vampireCoroutine != null)
        {
            StopCoroutine(_vampireCoroutine);
        }

        _vampireCoroutine = StartCoroutine(StealHealthCoroutine(healthPoints));
    }
}
