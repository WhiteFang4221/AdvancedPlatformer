using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(WallChecker))]
[RequireComponent(typeof(DetectingPlayer1))]

public class EnemyMoving1 : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 6f;

    private DetectingPlayer1 _detectingPlayer;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private Coroutine _stayCoroutine;
    private WaitForSeconds _stayTime = new WaitForSeconds(3);
    private WallChecker _surfacesChecker;

    private bool _isMoving = true;
    private bool _isTrapped = false;
    private bool _isFaceRight = true;
    private bool _isAttacking => _animator.GetBool(EnemyAnimationStrings.IsAttacking);

    public bool IsMoving
    {
        get
        {
            if (_detectingPlayer.IsHasTarget == true || IsAlive == false || _isTrapped)
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


    public bool IsAlive => _animator.GetBool(EnemyAnimationStrings.IsAlive);

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _surfacesChecker = GetComponent<WallChecker>();
        _detectingPlayer = GetComponent<DetectingPlayer1>();
    }

    private void Update()
    {
        if (!_isTrapped)
        {
            if (!_detectingPlayer.PointToPlayer.isActiveAndEnabled)
            {
                Patrol();
            }
            else
            {
                LookAtTarget(_detectingPlayer.PointToPlayer.transform.position);
                GoToTarget();
            }
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
    }

    public void GettingStuck()
    {
        _isTrapped = true;
    }

    public void GetOutStuck()
    {
        _isTrapped = false;
    }

    private void Patrol()
    {
        if (_surfacesChecker.IsOnWall)
        {
            StartStayCoroutine();
        }

        if (IsMoving && !_isAttacking)
        {
            _rigidbody.velocity = new Vector2(_moveSpeed * transform.localScale.x, _rigidbody.velocity.y);
        }
        else
        {
            _rigidbody.velocity = Vector2.zero;
        }
    }

    private void GoToTarget()
    {
        StopStayCoroutine();
        IsMoving = true;

        if (IsMoving && !_isAttacking)
        {
            _rigidbody.velocity = new Vector2(_moveSpeed * transform.localScale.x, _rigidbody.velocity.y);
        }
    }

    private void LookAtTarget(Vector2 target)
    {
        if (!_isAttacking)
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
}