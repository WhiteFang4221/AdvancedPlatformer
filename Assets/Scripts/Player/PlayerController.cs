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
    public const string IsMoving = "IsMoving";
    public const string IsGrounded = "IsGrounded";
    public const string yVelocity = "yVelocity";
    public const string JumpTrigger = "JumpTrigger";
    public const string AttackTrigger = "AttackTrigger";
    public const string IsCanMove = "IsCanMove";
    public const string IsOnLadder = "IsOnLadder";
    public const string RollTrigger = "RollTrigger";
    public const string IsRolling = "IsRolling";
    public const string IsDeath = "IsDeath";
    public const string HitTrigger = "HitTrigger";
    public const string IsAlive = "IsAlive";
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 12f;
    [SerializeField] private float _airMoveSpeed = 10f;
    [SerializeField] private float _ladderSpeed = 4f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _rollPower = 20f;
    [SerializeField] private Vector2 _moveInput;

    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private LadderController _ladderChecker;
    private SurfacesChecker _groundChecker;

    private float _rollCooldown = 1;
    private bool _isFaceRight = true;
    private bool _isCanRoll = true;
    private bool _isMoving;

    #region Свойства
    public Vector2 MoveInput
    {
        get
        {
            return _moveInput;
        }
    }

    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            _animator.SetBool(PlayerAnimator.IsMoving, value);
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
                if (_isMoving)
                {
                    if (_groundChecker.IsGrounded)
                    {
                        return _moveSpeed;
                    }
                    else
                    {
                        return _airMoveSpeed;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else if (_ladderChecker.IsOnLadder)
            {
                return _ladderSpeed;
            }
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
    #endregion

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundChecker = GetComponent<SurfacesChecker>();
        _ladderChecker = GetComponent<LadderController>();
    }

    private void FixedUpdate()
    {
        if (IsCanMove && IsRolling == false)
        {
            TurnAround();
            _rigidBody.velocity = new Vector2(_moveInput.x * CurrentSpeed, _rigidBody.velocity.y);
            _animator.SetFloat(PlayerAnimator.yVelocity, _rigidBody.velocity.y);
        }
        else if (_ladderChecker.IsOnLadder)
        {
            _rigidBody.velocity = new Vector2(0, _moveInput.y * CurrentSpeed);
            _animator.SetFloat(PlayerAnimator.yVelocity, _rigidBody.velocity.y);
        }
    }

    #region Input System
    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        IsMoving = MoveInput.x != 0;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_groundChecker.IsGrounded && IsCanMove)
            {
                _animator.SetTrigger(PlayerAnimator.JumpTrigger);
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _jumpForce);
            }
            else if (_ladderChecker.IsOnLadder == true)
            {
                _animator.SetBool(PlayerAnimator.IsOnLadder, false);
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started && _groundChecker.IsGrounded && _ladderChecker.IsOnLadder == false)
        {
            _animator.SetTrigger(PlayerAnimator.AttackTrigger);
        }
    }

    public void RollOver(InputAction.CallbackContext context)
    {
        if (context.performed && _groundChecker.IsGrounded && IsCanMove && _isCanRoll)
        {
            StartCoroutine(Dash());
        }
    }
    #endregion

    private IEnumerator Dash()
    {
        _isCanRoll = false;
        _rigidBody.velocity = new Vector2(0, 0);
        _animator.SetTrigger(PlayerAnimator.RollTrigger);

        if (_isFaceRight)
        {
            _rigidBody.AddForce(new Vector2(_rollPower, 0f), ForceMode2D.Impulse);
        }
        else
        {
            _rigidBody.AddForce(new Vector2(_rollPower * (-1), 0f), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(_rollCooldown);

        _isCanRoll = true;
    }

    private void TurnAround()
    {
        if ((_moveInput.x > 0 && _isFaceRight == false || (_moveInput.x < 0 && _isFaceRight == true)))
        {
            transform.localScale *= new Vector2(-1, 1);
            _isFaceRight = !_isFaceRight;
        }
    }
}
