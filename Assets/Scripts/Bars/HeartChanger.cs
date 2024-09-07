using Bars;
using UnityEngine;
[RequireComponent(typeof(Animator))]

public static class AnimatorHeartController
{
    public const string PlayerHealth = "PlayerHealth";
}

public class HeartChanger : MonoBehaviour
{
    [SerializeField] private PlayerBar _bar;

    private Animator _animator;
    private float _maxPercentHealth = 100;

    private void OnEnable()
    {
        _bar.HealthSliderChanged += ChangeState;
    }

    private void OnDisable()
    {
        _bar.HealthSliderChanged -= ChangeState;
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetFloat(AnimatorHeartController.PlayerHealth, _maxPercentHealth);
    }

    private void ChangeState(float currentHealth, float maxHealth)
    {
        float currentPercentHealth = (currentHealth * _maxPercentHealth) / maxHealth;
        _animator.SetFloat(AnimatorHeartController.PlayerHealth, currentPercentHealth);
    }
}
