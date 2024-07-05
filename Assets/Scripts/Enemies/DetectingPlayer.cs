using UnityEngine;

[RequireComponent(typeof(Animator))]

public class DetectingPlayer : MonoBehaviour
{
    [SerializeField] private DetectionZone _attackZone;
    [SerializeField] private bool _isHasTarget = false;

    private Animator _animator;

    public bool IsHasTarget
    {
        get
        {
            return _isHasTarget;
        }
        private set
        {
            _isHasTarget = value;
            _animator.SetBool(EnemyStringsAnimator.IsHasTarget, value);
        }
    }

    public bool IsAttacking
    {
        get
        {
            return _animator.GetBool(EnemyStringsAnimator.IsAttacking);
        }
    }

    public bool IsAlive
    {
        get
        {
            return _animator.GetBool(EnemyStringsAnimator.IsAlive);
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (IsAlive)
        {
            IsHasTarget = _attackZone.DetectedColliders.Count > 0;
        }
    }
}
