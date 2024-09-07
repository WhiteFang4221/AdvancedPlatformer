using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D), typeof(GroundChecker), typeof(LadderChecker))]

public class PlayerMoving : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 12f;
    [SerializeField] private float _ladderSpeed = 6f;
    [SerializeField] private float _airMoveSpeed = 8f;
    [SerializeField] private float _jumpForce = 18f;
    [SerializeField] private float _rollPower = 30f;
    [SerializeField] private float _rollCooldown = 1;

    public event Action HealTried;
    public event Action VimpireAbilityUsed;

    private Vector2 _moveInput;
    private Rigidbody2D _rigidbody;
    private GroundChecker _groundChecker;
    private LadderChecker _ladderChecker;
    private Animator _animator;

    private Coroutine _dashCoroutine;

    private float _walkstopRate = 0.2f;
    private float _moveStopRate = 0.1f;
    private bool _isFaceRight = true;
    private bool _isCanRoll = true;
    private bool _isMovingX = false;

    #region Свойства
    public Vector2 MoveInput => _moveInput;
    public bool IsCanMove
    {
        get
        {
            if (IsAlive)
            {
                return _animator.GetBool(PlayerAnimationStrings.IsCanMove);
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
                if (_isMovingX)
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
    public bool IsRolling => _animator.GetBool(PlayerAnimationStrings.IsRolling);
    public bool IsAlive => _animator.GetBool(PlayerAnimationStrings.IsAlive);
    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundChecker = GetComponent<GroundChecker>();
        _ladderChecker = GetComponent<LadderChecker>();
    }

    private void FixedUpdate()
    {
        if (IsCanMove && !IsRolling && !_ladderChecker.IsOnLadder)
        {
            TurnAround();
            _rigidbody.velocity = new Vector2(_moveInput.x * CurrentSpeed, _rigidbody.velocity.y);
            _animator.SetFloat(PlayerAnimationStrings.yVelocity, _rigidbody.velocity.y);
        }
        else if (_ladderChecker.IsOnLadder)
        {
            _rigidbody.velocity = new Vector2(0, _moveInput.y * CurrentSpeed);
            _animator.SetFloat(PlayerAnimationStrings.yVelocity, _rigidbody.velocity.y);
        }
        else if (_animator.GetBool(PlayerAnimationStrings.IsAttacking) || _animator.GetBool(PlayerAnimationStrings.IsHealing))
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
        _isMovingX = _moveInput.x != 0;
        _animator.SetBool(PlayerAnimationStrings.IsMovingX, _isMovingX);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_groundChecker.IsGrounded && IsCanMove)
            {
                _animator.SetBool(PlayerAnimationStrings.IsJumping, true);
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpForce);
            }
            else if (_ladderChecker.IsOnLadder == true)
            {
                _animator.SetBool(PlayerAnimationStrings.IsOnLadder, false);
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started && _groundChecker.IsGrounded && !_ladderChecker.IsOnLadder && !IsRolling)
        {
            _animator.SetTrigger(PlayerAnimationStrings.AttackTrigger);
        }
    }

    public void RollOver(InputAction.CallbackContext context)
    {
        if (context.performed && _groundChecker.IsGrounded && IsCanMove && _isCanRoll)
        {
            StartRollOverCoroutine();
        }
    }

    public void Heal(InputAction.CallbackContext context)
    {
        if (context.started && IsCanMove && _groundChecker.IsGrounded)
        {
            HealTried?.Invoke();
        }
    }

    public void UseVimpireAbility(InputAction.CallbackContext context)
    {
        if (context.started && IsCanMove && _groundChecker.IsGrounded)
        {
            _rigidbody.velocity = Vector2.zero;
            VimpireAbilityUsed?.Invoke();
        }
    }
    #endregion

    public void PushOffOnHit(Vector2 knockback)
    {
        if (_animator.GetBool(PlayerAnimationStrings.IsAlive))
        {
            _rigidbody.velocity = new Vector2(knockback.x, _rigidbody.velocity.y + knockback.y);
        }
    }

    private IEnumerator RollOverCoroutine()
    {
        _isCanRoll = false;
        _rigidbody.velocity = new Vector2(0, 0);
        _animator.SetTrigger(PlayerAnimationStrings.RollTrigger);

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

    private void StartRollOverCoroutine()
    {
        if (_dashCoroutine != null)
        {
            StopCoroutine(_dashCoroutine);
        }
        _dashCoroutine = StartCoroutine(RollOverCoroutine());
    }

    private void TurnAround()
    {
        if ((_moveInput.x > 0 && !_isFaceRight) || (_moveInput.x < 0 && _isFaceRight))
        {
            transform.localScale *= new Vector2(-1, 1);
            _isFaceRight = !_isFaceRight;
        }
    }
}
