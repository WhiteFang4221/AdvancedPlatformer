using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SurfacesChecker))]
[RequireComponent(typeof(DetectingPlayer), typeof(EnemyHealthManager))]

public class DarkWizardMoving : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 9f;


    private DetectingPlayer _detectingPlayer;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private EnemySurfacesChecker _surfacesChecker;
    private EnemyHealthManager _enemyHealthManager;
    private Coroutine _stayCoroutine;

    private WaitForSeconds _stayTime = new WaitForSeconds(3);
    private bool _isMoving = true;
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
            _animator.SetBool(EnemyStringsAnimator.IsMoving, value);
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

    private void OnEnable()
    {
        _enemyHealthManager.HitTaken += OnHit;
    }

    private void OnDisable()
    {
        _enemyHealthManager.HitTaken -= OnHit;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _surfacesChecker = GetComponent<EnemySurfacesChecker>();
        _enemyHealthManager = GetComponent<EnemyHealthManager>();
        _detectingPlayer = GetComponent<DetectingPlayer>();
    }

    private void Update()
    {
        if (_detectingPlayer.PointToPlayer.isActiveAndEnabled == false)
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
        if (collision.TryGetComponent(out EnemyStayPoint point))
        {
            TurnAround();
            StartStayCoroutine();
        }

        if (collision.TryGetComponent(out PointToPlayer playerPoint) && _detectingPlayer.IsSeePlayer == false)
        {
            playerPoint.gameObject.SetActive(false);
            StartStayCoroutine();
        }
    }

    private void Patrol()
    {
        if (_surfacesChecker.IsOnWall)
        {
            StartStayCoroutine();
        }

        if (IsMoving && IsAttacking == false)
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

        if (IsMoving && IsAttacking == false)
        {
            _rigidbody.velocity = new Vector2(_moveSpeed * transform.localScale.x, _rigidbody.velocity.y);
        }
    }

    private void LookAtTarget(Vector2 target)
    {
        if (IsAttacking == false)
        {
            if (transform.position.x > target.x && _isFaceRight == true)
            {
                TurnAround();
                Debug.Log("Повернулся на врага влево");
            }
            else if (transform.position.x < target.x && _isFaceRight == false)
            {
                TurnAround();
                Debug.Log("Повернулся на врага вправо");
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

    private void OnHit(int damage, Vector2 knockback)
    {
        _rigidbody.velocity = new Vector2(knockback.x, _rigidbody.velocity.y + knockback.y);
    }
}