using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public static class PlayerAnimator
{
    public static class Params
    {
        public const string SpeedX = "SpeedX";
        public const string SpeedY = "SpeedY";
        public const string JumpTrigger = "JumpTrigger";
        public const string IsOnGround = "IsOnGround";
        public const string IsRolling = "IsRolling";
        public const string RollTrigger = "RollTrigger";
        public const string IsOnLadder = "IsOnLadder";
        public const string AttackTrigger = "AttackTrigger";
        public const string AttackCombo = "AttackCombo";
        public const string Attack1 = "Attack1";
        public const string Attack2 = "Attack2";
        public const string Attack3 = "Attack3";
        public const string Attack4 = "Attack4";

        public const string AttackWindowOpen = "AttackWindowOpen";
        public const string WeaponActive = "WeaponActive";
        public const string HitTrigger = "HitTrigger";


        public static class States
        {
            public const string Idle = nameof(Idle);
            public const string Run = nameof(Run);
            public const string Jump = nameof(Jump);
            public const string Fall = nameof(Fall);
            public const string Landing = nameof(Landing);
            public const string Roll = nameof(Roll);
            public const string Climb = nameof(Climb);
            public const string BaseAttack = nameof(BaseAttack);
            public const string SecondAttack = nameof(SecondAttack);
            public const string ThirdAttack = nameof(ThirdAttack);
            public const string FourthAttack = nameof(FourthAttack);
            public const string KnightTakeDamage = nameof(KnightTakeDamage);
        }
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
    [SerializeField] private LayerMask _stairMask;

    private Animator _animator;
    private Vector2 _moveVector;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    private float _groundCheckRadius;
    private float _ladderCheckerRadius;
    private float _rollingTime = 0.5f;
    private float _rollCooldown = 0;
    private float _ladderCenter;
    private float _verticalInput;

    private bool _isfaceRight = true;
    private bool _isCanRoll = true;
    public bool _isOnGround;
    public bool _isIgnoreStairCollider = false;
    public bool _isOnLadder;
    private bool _isRolling;
    private bool _isLadderCheck;
    private bool _isBottomLadderCheck;
    private bool _isLadderPositionCorrected = true;

    private void Awake()
    {
        
    }
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundCheckRadius = _groundChecker.GetComponent<CircleCollider2D>().radius;
        _ladderCheckerRadius = _ladderChecker.GetComponent<CircleCollider2D>().radius;
    }

    private void Update()
    {
        if (!_isRolling)
        {
            Walk();
        }

        Reflect();
        CheckGround();
        CheckLadder();
        CatchOnLadder();
        FindPositionOnLadder();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && _isCanRoll && _isOnGround)
        {
            StartCoroutine(Roll());
        }
    }

    #region BaseMove

    private void Walk()
    {

        _moveVector.x = Input.GetAxisRaw("Horizontal");

        if (!_isOnLadder)
        {
            _rb.velocity = new Vector2(_moveVector.x * _speed, _rb.velocity.y);
        }

        _animator.SetFloat(PlayerAnimator.Params.SpeedX, Mathf.Abs(_moveVector.x));
    }

    private void Reflect()
    {
        if ((_moveVector.x > 0 && !_isfaceRight || (_moveVector.x < 0 && _isfaceRight)))
        {
            transform.localScale *= new Vector2(-1, 1);
            _isfaceRight = !_isfaceRight;
        }
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

    private void Jump()
    {
        if (_isOnGround && !_isOnLadder && !_isRolling)
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

    #region Leadder
    private void CheckLadder()
    {
        _isLadderCheck = Physics2D.OverlapPoint(_ladderChecker.position, _ladderMask);
        _isBottomLadderCheck = Physics2D.OverlapPoint(_bottomLadderChecker.position, _ladderMask);
    }

    private void ChangeBodyTypeOnLadder()
    {
        if (_isOnLadder)
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.velocity = new Vector2(_rb.velocity.x, _verticalInput * _climbSpeed);
        }
        else
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void CatchOnLadder()
    {
        if (!_isRolling)
        {
            _verticalInput = Input.GetAxisRaw("Vertical");
        }

        if (_isLadderCheck || _isBottomLadderCheck)
        {
            if (!_isLadderCheck && _isBottomLadderCheck) // Сверху
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
            else if (_isLadderCheck && _isBottomLadderCheck) // на лестнице
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
            else if (_isLadderCheck && !_isBottomLadderCheck) // внизу
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

    private void FindPositionOnLadder()
    {
        if (_isOnLadder && _isLadderPositionCorrected)
        {
            _isLadderPositionCorrected = !_isLadderPositionCorrected;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_ladderChecker.position, _ladderCheckerRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_bottomLadderChecker.position, _ladderCheckerRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "LadderStairs")
        {
            Physics2D.IgnoreCollision(collision.GetComponent<EdgeCollider2D>(), GetComponent<CapsuleCollider2D>(), true);
            _isIgnoreStairCollider = true;
            _spriteRenderer.sortingOrder = 4;
        }

        if (collision.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(collision.GetComponent<CapsuleCollider2D>(), GetComponent<CapsuleCollider2D>(), true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "LadderStairs")
        {
            Physics2D.IgnoreCollision(collision.GetComponent<EdgeCollider2D>(), GetComponent<CapsuleCollider2D>(), false);
            _isIgnoreStairCollider = false;
            _spriteRenderer.sortingOrder = 5;
        }
    }
}


