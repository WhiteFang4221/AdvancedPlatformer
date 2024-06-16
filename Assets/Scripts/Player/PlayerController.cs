using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]

public static class PlayerAnimator
{
    public static class Params
    {
        public const string SpeedX = "SpeedX";
        public const string SpeedY = "SpeedY";
        public const string IsOnGround = "IsOnGround";
        public const string IsOnLadder = "IsOnLadder";
        public const string IsRolling = "IsRolling";
        public const string IsDeath = "IsDeath";
        public const string JumpTrigger = "JumpTrigger";
        public const string RollTrigger = "RollTrigger";
        public const string HitTrigger = "HitTrigger"; 
        public const string AttackTrigger = "AttackTrigger";
    }

    public static class States
    {
        public const string Idle = nameof(Idle);
        public const string Run = nameof(Run);
        public const string Jump = nameof(Jump);
        public const string Fall = nameof(Fall);
        public const string Landing = nameof(Landing);
        public const string Roll = nameof(Roll);
        public const string BaseAttack = nameof(BaseAttack);
        public const string KnightTakeDamage = nameof(KnightTakeDamage);
    }
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 12f;
    [SerializeField] private float _climbSpeed = 4f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _rollPower = 15;
    [SerializeField] private Transform _groundChecker;
    [SerializeField] private Transform _ladderChecker;
    [SerializeField] private Transform _bottomLadderChecker;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _ladderMask;

    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private CapsuleCollider2D _capsuleCollider;
    private Vector2 _moveVector;
    private Coroutine _rollCoroutine;

    private float _groundCheckRadius;
    private float _rollingTime = 0.5f;
    private float _rollCooldown = 0;
    private float _ladderCenter;
    private float _verticalInput;

    private bool _isfaceRight = true;
    private bool _isCanRoll = true;
    private bool _isOnGround;
    private bool _isIgnoreStairCollider = false;
    private bool _isOnLadder;
    private bool _isRolling = false;
    private bool _isLadderCheck;
    private bool _isBottomLadderCheck;
    private bool _isLadderPositionCorrected = true;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundCheckRadius = _groundChecker.GetComponent<CircleCollider2D>().radius;
    }

    private void Update()
    {
        if (!_isRolling)
        {
            Move();
        }

        TurnAround();

        CheckLadder();
        FindPositionOnLadder();

        if (Input.GetKeyDown(KeyCode.Space) )
        {
            Jump();
        }
        
         CheckGround();

        if (Input.GetKeyDown(KeyCode.LeftShift) && _isCanRoll == true && _isOnGround == true)
        {
            _rollCoroutine = StartCoroutine(Roll());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Platform platform))
        {
            Physics2D.IgnoreCollision(collision.GetComponent<EdgeCollider2D>(), _capsuleCollider, true);
            _isIgnoreStairCollider = true;
            _spriteRenderer.sortingOrder = 3;
        }

        if (collision.TryGetComponent(out DarkWizard enemy))
        {
            Physics2D.IgnoreCollision(collision.GetComponent<CapsuleCollider2D>(), _capsuleCollider, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Platform platform))
        {
            Physics2D.IgnoreCollision(collision.GetComponent<EdgeCollider2D>(), _capsuleCollider, false);
            _isIgnoreStairCollider = false;
            _spriteRenderer.sortingOrder = 7;
        }
    }

    public void TakeHit(float damage)
    {
        _animator.SetTrigger(PlayerAnimator.Params.HitTrigger);
    }

    private void CheckGround()
    {
        _isOnGround = Physics2D.OverlapCircle(_groundChecker.position, _groundCheckRadius, _groundLayer);

        if (_isIgnoreStairCollider == true)
        {
            _isOnGround = false;
        }

        _animator.SetBool(PlayerAnimator.Params.IsOnGround, _isOnGround);
        _animator.SetFloat(PlayerAnimator.Params.SpeedY, (int)_rb.velocity.y);
    }

    #region BaseMove
    private void Move()
    {
        _moveVector.x = Input.GetAxisRaw("Horizontal");

        if (_isOnLadder == false)
        {
            _rb.velocity = new Vector2(_moveVector.x * _speed, _rb.velocity.y);
        }
        else
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _verticalInput * _climbSpeed);
        }

        _animator.SetFloat(PlayerAnimator.Params.SpeedX, Mathf.Abs(_moveVector.x));
    }

    private void TurnAround()
    {
        if ((_moveVector.x > 0 && !_isfaceRight || (_moveVector.x < 0 && _isfaceRight)))
        {
            transform.localScale *= new Vector2(-1, 1);
            _isfaceRight = !_isfaceRight;
        }
    }

    private void Jump()
     {
        if (!_isOnLadder && !_isRolling && _isOnGround == true)
        {
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _animator.SetTrigger(PlayerAnimator.Params.JumpTrigger);
        }
        else if (_isOnLadder)
        {
            _isOnLadder = false;
        }
    }

    private IEnumerator Roll()
    {
        _isCanRoll = false;
        _isRolling = true;
        _rb.velocity = new Vector2(0, 0);
        _animator.SetTrigger(PlayerAnimator.Params.RollTrigger);

        if (_isfaceRight)
        {
            _rb.AddForce(new Vector2(_rollPower, 0f), ForceMode2D.Impulse);
        }
        else
        {
            _rb.AddForce(new Vector2(_rollPower * (-1), 0f), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(_rollingTime);
        _isRolling = false;
        yield return new WaitForSeconds(_rollCooldown);
        _isCanRoll = true;
    }
    #endregion

    #region Ladder
    private void CheckLadder()
    {
        _isLadderCheck = Physics2D.OverlapPoint(_ladderChecker.position, _ladderMask);
        _isBottomLadderCheck = Physics2D.OverlapPoint(_bottomLadderChecker.position, _ladderMask);
        CatchLadder();
    }

    private void CatchLadder()
    {
        if (!_isRolling)
        {
            _verticalInput = Input.GetAxisRaw("Vertical");
        }

        if (_isLadderCheck || _isBottomLadderCheck)
        {
            if (_isLadderCheck == false && _isBottomLadderCheck == true) // Сверху
            {
                if (_verticalInput > 0)
                {
                    _isOnLadder = false;
                }
                else if (_verticalInput < 0)
                {
                    _isOnLadder = true;
                }
            }
            else if (_isLadderCheck == true && _isBottomLadderCheck == true) // на лестнице
            {
                if (_verticalInput > 0)
                {
                    _isOnLadder = true;
                }
                else if (_verticalInput < 0)
                {
                    _isOnLadder = true;
                }
            }
            else if (_isLadderCheck == true && _isBottomLadderCheck == false) // внизу
            {
                if (_verticalInput > 0)
                {
                    _isOnLadder = true;
                }
                else if (_verticalInput < 0)
                {
                    _isOnLadder = false;
                }
            }
        }
        else
        {
            _isOnLadder = false;
        }

        ChangeBodyTypeOnLadder();

        _animator.SetBool(PlayerAnimator.Params.IsOnLadder, _isOnLadder);
    }

    private void ChangeBodyTypeOnLadder()
    {
        if (_isOnLadder)
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void FindPositionOnLadder()
    {
        if (_isOnLadder && _isLadderPositionCorrected == true)
        {
            _isLadderPositionCorrected = false;
            _rb.velocity = Vector2.zero;
            ChangePositionOnLadder();
        }
        else if (!_isOnLadder && !_isLadderPositionCorrected)
        {
            _isLadderPositionCorrected = true;
        }
    }

    private void ChangePositionOnLadder()
    {
        if (_isLadderCheck)
        {
            _ladderCenter = Physics2D.OverlapPoint(_ladderChecker.position, _ladderMask).GetComponent<BoxCollider2D>().bounds.center.x;
        }
        else if (_bottomLadderChecker)
        {
            _ladderCenter = Physics2D.OverlapPoint(_bottomLadderChecker.position, _ladderMask).GetComponent<BoxCollider2D>().bounds.center.x;
        }

        transform.position = new Vector2(_ladderCenter, transform.position.y);
    }
    #endregion
}


