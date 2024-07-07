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
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 12f;
    [SerializeField] private float _airMoveSpeed = 10f;
    [SerializeField] private float _ladderSpeed = 4f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _rollPower = 20f;
    [SerializeField] private Vector2 _moveInput;

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
    public Vector2 MoveInput
    {
        get
        {
            return _moveInput;
        }
    }

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

    public bool IsRolling
    {
        get
        {
            return _animator.GetBool(PlayerAnimator.IsRolling);
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

    public bool IsAlive
    {
        get
        {
            return _animator.GetBool(PlayerAnimator.IsAlive);
        }
    }

    public bool IsAttacking
    {
        get
        {
            return _animator.GetBool(PlayerAnimator.IsAttacking);
        }
    }
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
        if (IsCanMove && IsRolling == false && _ladderChecker.IsOnLadder == false)
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

        else if (IsAttacking || _animator.GetBool(PlayerAnimator.IsHealing) == true)
        {
            _rigidbody.velocity = new Vector2(Mathf.Lerp(_rigidbody.velocity.x, 0, _walkstopRate), _rigidbody.velocity.y);
        }
        else if (IsAlive == false)
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
        if (context.started && _groundChecker.IsGrounded && _ladderChecker.IsOnLadder == false && IsRolling == false)
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
        if ((_moveInput.x > 0 && _isFaceRight == false || (_moveInput.x < 0 && _isFaceRight == true)))
        {
            transform.localScale *= new Vector2(-1, 1);
            _isFaceRight = !_isFaceRight;
        }
    }

    private void OnHit(int damage, Vector2 knockback)
    {
        _rigidbody.velocity = new Vector2(knockback.x, _rigidbody.velocity.y + knockback.y);
    }
}
