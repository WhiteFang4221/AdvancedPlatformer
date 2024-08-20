using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(WallChecker))]
[RequireComponent(typeof(DetectingPlayer), typeof(Health))]

public class DarkWizardMoving : MonoBehaviour, IPushable
{
    [SerializeField] private float _moveSpeed = 9f;

    private DetectingPlayer _detectingPlayer;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private Coroutine _stayCoroutine;
    private WaitForSeconds _stayTime = new WaitForSeconds(3);
    private WallChecker _surfacesChecker;
    private Health _enemyHealth;

    private bool _isMoving = true;
    private bool _isTrapped = false;
    private bool _isFaceRight = true;

    public bool IsMoving
    {
        get
        {
            if (_detectingPlayer.IsHasTarget == true || IsAlive == false)
            {
                return false;
            }
            else
            {
                return _isMoving;
            }
        }
        private set
        {
            _isMoving = value;
            _animator.SetBool(EnemyAnimationStrings.IsMoving, value);
        }
    }

    public bool IsAttacking => _animator.GetBool(EnemyAnimationStrings.IsAttacking);

    public bool IsAlive => _animator.GetBool(EnemyAnimationStrings.IsAlive);

    public bool IsTrapped
    {
        get
        {
            return _isTrapped;
        }
        private set
        {
            _isTrapped = value;
            _animator.SetBool(EnemyAnimationStrings.IsTrapped, value);
        }
    }

    private void OnEnable()
    {
        //_enemyHealth.HitTaken += OnHit;
    }

    private void OnDisable()
    {
        //_enemyHealth.HitTaken -= OnHit;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _surfacesChecker = GetComponent<WallChecker>();
        _enemyHealth = GetComponent<Health>();
        _detectingPlayer = GetComponent<DetectingPlayer>();
    }

    private void Update()
    {
        if (!_detectingPlayer.PointToPlayer.isActiveAndEnabled)
        {
            Patrol();
        }
        else
        {
            LookAtTarget(_detectingPlayer.PointToPlayer.transform.position);
            GoToPlayer();
        }
    }

    private void FixedUpdate()
    {
        if (_surfacesChecker.IsOnWall)
        {
            TurnAround();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyStayPoint point) && _detectingPlayer.PointToPlayer.isActiveAndEnabled == false)
        {
            StartStayCoroutine();
        }

        if (collision.TryGetComponent(out PointToPlayer playerPoint) && _detectingPlayer.IsSeePlayer == false)
        {
            playerPoint.gameObject.SetActive(false);
            StartStayCoroutine();
        }


        if (collision.TryGetComponent(out VampireAbility ability))
        {
            IsTrapped = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out VampireAbility ability))
        {
            IsTrapped = false;
        }
    }

    private void Patrol()
    {
        if (_surfacesChecker.IsOnWall)
        {
            StartStayCoroutine();
        }

        if (IsMoving && !IsAttacking)
        {
            _rigidbody.velocity = new Vector2(_moveSpeed * transform.localScale.x, _rigidbody.velocity.y);
        }
        else
        {
            _rigidbody.velocity = Vector2.zero;
        }
    }

    private void GoToPlayer()
    {
        StopStayCoroutine();
        IsMoving = true;

        if (IsMoving && !IsAttacking)
        {
            _rigidbody.velocity = new Vector2(_moveSpeed * transform.localScale.x, _rigidbody.velocity.y);
        }
    }

    private void LookAtTarget(Vector2 target)
    {
        if (!IsAttacking)
        {
            if (transform.position.x > target.x && _isFaceRight)
            {
                TurnAround();
            }
            else if (transform.position.x < target.x && !_isFaceRight)
            {
                TurnAround();
            }
        }
    }

    private void TurnAround()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        _isFaceRight = !_isFaceRight;
    }

    private IEnumerator Stay()
    {
        IsMoving = false;
        yield return _stayTime;
        IsMoving = true;
    }

    private void StartStayCoroutine()
    {
        if (_stayCoroutine != null)
        {
            StopCoroutine(_stayCoroutine);
        }
        _stayCoroutine = StartCoroutine(Stay());
    }

    private void StopStayCoroutine()
    {
        if (_stayCoroutine != null)
        {
            StopCoroutine(_stayCoroutine);
            _stayCoroutine = null;
        }
    }

    public void PushOffOnHit(Vector2 knockback)
    {
        _rigidbody.velocity = new Vector2(knockback.x, _rigidbody.velocity.y + knockback.y);
    }
}