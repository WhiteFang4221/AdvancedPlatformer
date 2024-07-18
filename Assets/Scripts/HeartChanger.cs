using UnityEngine;
[RequireComponent(typeof(Animator))]

public static class AnimatorHeartController
{
    public const string PlayerHealth = "PlayerHealth";
}

public class HeartChanger : MonoBehaviour
{
    [SerializeField] private SmoothHealthBar _healthBar;


    private Animator _animator;
    private float _maxPercentHealth = 100;

    private void Awake()
    {
        _healthBar.HeartStateChanged += ChangeState;
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetFloat(AnimatorHeartController.PlayerHealth, _maxPercentHealth);
    }

    private void ChangeState(float maxHealth, float currentHealth)
    {
        float currentPercentHealth = (currentHealth * _maxPercentHealth) / maxHealth;
        _animator.SetFloat(AnimatorHeartController.PlayerHealth, currentPercentHealth);
    }
}
