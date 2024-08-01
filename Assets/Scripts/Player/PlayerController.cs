using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(PlayerHealthManager))]

public static class PlayerAnimator
{
    public const string IsMovingX = "IsMovingX";
    public const string IsMovingY = "IsMovingY";
    public const string IsGrounded = "IsGrounded";
    public const string yVelocity = "yVelocity";
    public const string IsJumping = "IsJumping";
    public const string AttackTrigger = "AttackTrigger";
    public const string IsCanMove = "IsCanMove";
    public const string IsOnLadder = "IsOnLadder";
    public const string RollTrigger = "RollTrigger";
    public const string IsRolling = "IsRolling";
    public const string IsDeath = "IsDeath";
    public const string HitTrigger = "HitTrigger";
    public const string IsAlive = "IsAlive";
    public const string IsLockVelocity = "IsLockVelocity";
    public const string IsAttacking = "IsAttacking";
    public const string IsHealing = "IsHealing";
    public const string IsHealSucceeded = "IsHealSucceeded";
    public const string IsVampirismUse = "IsVampirismUse";
    public const string IsVampirismFinish = "IsVampirismFinish";
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 12f;
    [SerializeField] private float _airMoveSpeed = 10f;
    [SerializeField] private float _ladderSpeed = 4f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _rollPower = 20f;
    [SerializeField] private Vector2 _moveInput;
    [SerializeField] private VampireAbility _vimpireAbility;

    public Action VimpireAbilityUsed;
    public Action VimpireAbilityCanceled;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private LadderController _ladderChecker;
    private PlayerSurfacesChecker _groundChecker;
    private PlayerHealthManager _playerHealthManager;
    private Coroutine _dashCoroutine;

    private float _rollCooldown = 1;
    private float _walkstopRate = 0.2f;
    private float _moveStopRate = 0.1f;
    private bool _isFaceRight = true;
    private bool _isCanRoll = true;
    private bool _isMovingX;

    #region Свойства
    public Vector2 MoveInput => _moveInput;
    public bool IsMovingX
    {
        get
        {
            return _isMovingX;
        }
        private set
        {
            _isMovingX = value;
            _animator.SetBool(PlayerAnimator.IsMovingX, value);
        }
    }
    public bool IsCanMove
    {
        get
        {
            if (IsAlive)
            {
                return _animator.GetBool(PlayerAnimator.IsCanMove);
            }
            else
            {
                return false;
            }
        }
    }
    public float CurrentSpeed
    {
        get
        {
            if (IsCanMove)
            {
                if (IsMovingX)
                {
                    if (_groundChecker.IsGrounded)
                    {
                        return _moveSpeed;
                    }
                    else if (_ladderChecker.IsOnLadder)
                    {
                        return _ladderSpeed;
                    }
                    else
                    {
                        return _airMoveSpeed;
                    }
                }
                else if (_ladderChecker.IsOnLadder)
                {
                    return _ladderSpeed;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }
    public bool IsRolling => _animator.GetBool(PlayerAnimator.IsRolling);
    public bool IsAlive => _animator.GetBool(PlayerAnimator.IsAlive);
    public bool IsAttacking => _animator.GetBool(PlayerAnimator.IsAttacking);
    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundChecker = GetComponent<PlayerSurfacesChecker>();
        _ladderChecker = GetComponent<LadderController>();
        _playerHealthManager = GetComponent<PlayerHealthManager>();
    }

    private void OnEnable()
    {
        _playerHealthManager.HitTaken += OnHit;
    }

    private void OnDisable()
    {
        _playerHealthManager.HitTaken -= OnHit;
    }

    private void FixedUpdate()
    {
        if (IsCanMove && !IsRolling && !_ladderChecker.IsOnLadder)
        {
            TurnAround();
            _rigidbody.velocity = new Vector2(_moveInput.x * CurrentSpeed, _rigidbody.velocity.y);
            _animator.SetFloat(PlayerAnimator.yVelocity, _rigidbody.velocity.y);
        }
        else if (_ladderChecker.IsOnLadder)
        {
            _rigidbody.velocity = new Vector2(0, _moveInput.y * CurrentSpeed);
            _animator.SetFloat(PlayerAnimator.yVelocity, _rigidbody.velocity.y);
        }

        else if (IsAttacking || _animator.GetBool(PlayerAnimator.IsHealing) || _animator.GetBool(PlayerAnimator.IsVampirismUse))
        {
            _rigidbody.velocity = new Vector2(Mathf.Lerp(_rigidbody.velocity.x, 0, _walkstopRate), _rigidbody.velocity.y);
        }
        else if (!IsAlive)
        {
            _rigidbody.velocity = new Vector2(Mathf.Lerp(_rigidbody.velocity.x, 0, _moveStopRate), _rigidbody.velocity.y);
        }
    }

    #region Input System
    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        IsMovingX = MoveInput.x != 0;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_groundChecker.IsGrounded && IsCanMove)
            {
                _animator.SetBool(PlayerAnimator.IsJumping, true);
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpForce);
            }
            else if (_ladderChecker.IsOnLadder == true)
            {
                _animator.SetBool(PlayerAnimator.IsOnLadder, false);
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started && _groundChecker.IsGrounded && !_ladderChecker.IsOnLadder && !IsRolling && !_animator.GetBool(PlayerAnimator.IsVampirismUse))
        {
            _animator.SetTrigger(PlayerAnimator.AttackTrigger);
        }
    }

    public void RollOver(InputAction.CallbackContext context)
    {
        if (context.performed && _groundChecker.IsGrounded && IsCanMove && _isCanRoll)
        {
            StartDashCoroutine();
        }
    }

    public void Heal(InputAction.CallbackContext context)
    {
        if (context.started && IsCanMove && _groundChecker.IsGrounded && _playerHealthManager.PotionHealQuantity > 0)
        {
            _animator.SetBool(PlayerAnimator.IsHealing, true);
        }
    }

    public void UseVimpireAbility(InputAction.CallbackContext context)
    {
        if (context.started && _vimpireAbility.IsCanUseAbility && IsCanMove && _groundChecker.IsGrounded)
        {
            _animator.SetBool(PlayerAnimator.IsVampirismUse, true);
            VimpireAbilityUsed?.Invoke();
        }
    }

    public void CancelAbility(InputAction.CallbackContext context)
    {
        if (context.started && _animator.GetBool(PlayerAnimator.IsVampirismUse))
        {
            VimpireAbilityCanceled?.Invoke();
        }
    }
    #endregion

    private IEnumerator Dash()
    {
        _isCanRoll = false;
        _rigidbody.velocity = new Vector2(0, 0);
        _animator.SetTrigger(PlayerAnimator.RollTrigger);

        if (_isFaceRight)
        {
            _rigidbody.AddForce(new Vector2(_rollPower, 0f), ForceMode2D.Impulse);
        }
        else
        {
            _rigidbody.AddForce(new Vector2(_rollPower * (-1), 0f), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(_rollCooldown);

        _isCanRoll = true;
    }

    private void StartDashCoroutine()
    {
        if (_dashCoroutine != null)
        {
            StopCoroutine(_dashCoroutine);
        }
        _dashCoroutine = StartCoroutine(Dash());
    }

    private void TurnAround()
    {
        if ((_moveInput.x > 0 && !_isFaceRight) || (_moveInput.x < 0 && _isFaceRight))
        {
            transform.localScale *= new Vector2(-1, 1);
            _isFaceRight = !_isFaceRight;
        }
    }

    private void OnHit(Vector2 knockback)
    {
        if (_animator.GetBool(PlayerAnimator.IsVampirismUse))
        {
            VimpireAbilityCanceled?.Invoke();
        }

        _rigidbody.velocity = new Vector2(knockback.x, _rigidbody.velocity.y + knockback.y);
    }
}
