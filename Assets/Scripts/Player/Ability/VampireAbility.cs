using System;
using System.Collections;
using UnityEngine;


[RequireComponent (typeof(VampirismDetectionZone))]
public class VampireAbility : MonoBehaviour
{
    [SerializeField] private PlayerMoving _playerController;
    [SerializeField] private PlayerHealPotion _playerHealthManager;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _timeAbility = 6f;
    [SerializeField] private int _abilityDamage = 5;
    [SerializeField] private float _cooldownAbility = 10f;

    private VampirismDetectionZone _detectionZone;
    private Coroutine _vampirismCoroutine;
    private Coroutine _reloadAbilityCoroutine;
    private Coroutine _stealHealthCoroutine;
    private WaitForSeconds _timeTickHealth = new WaitForSeconds(1);

    public Action<float> AbilityActivated;
    public Action<int> HealthStolen;

    public int AbilityDamage => _abilityDamage;

    public float TimeLeft { get; private set; } = 0;

    public bool IsCanUseAbility { get; private set; } = true;

    private void Awake()
    {
        _detectionZone = GetComponent<VampirismDetectionZone>();
    }

    private void OnEnable()
    {
        _playerController.VimpireAbilityUsed += UseAbility;
        _detectionZone.EnemyCatched += StartStealHealthCoroutine;
        _detectionZone.EnemyLost += StopStealHealthCoroutine;
    }

    private void OnDisable()
    {
        _playerController.VimpireAbilityUsed -= UseAbility;
        _detectionZone.EnemyCatched -= StartStealHealthCoroutine;
         _detectionZone.EnemyLost -= StopStealHealthCoroutine;
    }

    private void UseAbility()
    {
        IsCanUseAbility = false;
        _vampirismCoroutine = StartCoroutine(Vampirism());
    }

    private IEnumerator Vampirism()
    {
        TimeLeft = _timeAbility;
        AbilityActivated?.Invoke(_timeAbility);

        while (TimeLeft >= 0)
        {
            TimeLeft -= Time.deltaTime;
            yield return null;
        }

        _reloadAbilityCoroutine = StartCoroutine(ReloadAbility());
    }

    private IEnumerator ReloadAbility()
    {
        TimeLeft = 0;
        AbilityActivated?.Invoke(_cooldownAbility);
        _animator.SetBool(PlayerAnimationStrings.IsVampirismFinish, true);

        while (TimeLeft <= _cooldownAbility)
        {
            TimeLeft += Time.deltaTime;
            yield return null;
        }
      
        IsCanUseAbility = true;
    }

    private void StartReloadAbility()
    {
        if (_reloadAbilityCoroutine != null)
        {
            StopCoroutine(_reloadAbilityCoroutine);
        }

        _reloadAbilityCoroutine = StartCoroutine(ReloadAbility());
    }

    private void StopVampirismCoroutine()
    {
        if (_vampirismCoroutine != null)
        {
            StopCoroutine(_vampirismCoroutine);
        }

        StartReloadAbility();
    }

    private void CancelAbility()
    {
        TimeLeft = 0;
        _animator.SetBool(PlayerAnimationStrings.IsVampirismFinish, true);
        StopVampirismCoroutine();
    }

    private IEnumerator StealHealth(Health enemy)
    {
        while (_animator.GetBool(PlayerAnimationStrings.IsVampirismUse))
        {
            enemy.LosingHealth(AbilityDamage);
            HealthStolen?.Invoke(AbilityDamage);

            if (enemy.CurrentHealth <= 0)
            {
                StopStealHealthCoroutine(enemy);
            }

            yield return _timeTickHealth;
        }
    }

    private void StartStealHealthCoroutine(Health enemy)
    {
        _stealHealthCoroutine = StartCoroutine(StealHealth(enemy));
    }

    private void StopStealHealthCoroutine(Health enemy)
    {
        StopCoroutine(_stealHealthCoroutine);
    }
}
