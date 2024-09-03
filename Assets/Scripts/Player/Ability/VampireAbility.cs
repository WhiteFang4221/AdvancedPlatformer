using HealthSystem;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerMoving), typeof(PlayerHealth))]
public class VampireAbility : MonoBehaviour
{
    [SerializeField] private VampirismAura _vampirismAura;
    [SerializeField] private float _timeAbility = 6f;
    [SerializeField] private int _abilityDamage = 5;
    [SerializeField] private float _cooldownAbility = 10f;

    public event Action<float> AbilityActivated;

    private Animator _animator;
    private PlayerMoving _player;
    private PlayerHealth _health;


    private Coroutine _vampirismCoroutine;
    private Coroutine _reloadAbilityCoroutine;
    private Coroutine _stealHealthCoroutine;
    private WaitForSeconds _timeTickHealth = new WaitForSeconds(1);

    public float TimeLeft { get; private set; } = 0;
    public bool IsCanUseAbility { get; private set; } = true;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponent<PlayerMoving>();
        _health = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        _player.VimpireAbilityUsed += UseAbility;
        _health.GotHit += CancelAbility;
    }

    private void OnDisable()
    {
        _player.VimpireAbilityUsed -= UseAbility;
        _health.GotHit -= CancelAbility;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        _player.VimpireAbilityUsed -= UseAbility;
        _health.GotHit -= CancelAbility;
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
        AbilityActivated?.Invoke(_timeAbility);
        _vampirismAura.gameObject.SetActive(true);
        TimeLeft = _timeAbility;

        while (TimeLeft >= 0)
        {
            TimeLeft -= Time.deltaTime;
            yield return null;
        }

        StopAbilityCoroutine();
    }

    private IEnumerator ReloadAbility()
    {
        TimeLeft = 0;
        AbilityActivated?.Invoke(_cooldownAbility);
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
        if (_vampirismCoroutine != null)
        {
            StopCoroutine(_vampirismCoroutine);
            _vampirismCoroutine = null;
        }
        _vampirismCoroutine = StartCoroutine(AbilityCoroutine());
    }

    private void StopAbilityCoroutine()
    {
        if (_vampirismCoroutine != null)
        {
            StopCoroutine(_vampirismCoroutine);
            _vampirismCoroutine = null;

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
}
