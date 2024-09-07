using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(WallChecker))]
[RequireComponent(typeof(Enemy), typeof(DetectPlayer))]
public class EnemyMoving : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Enemy _enemy;
    private DetectPlayer _detectPlayer;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private WallChecker _surfacesChecker;

    private Coroutine _stayCoroutine;
    private WaitForSeconds _stayTime = new WaitForSeconds(3);

    [SerializeField] private bool _isMoving = true;
    private bool _isFaceRight = true;
    private bool _isTrapped = false;
    private bool _isAttacking => _animator.GetBool(EnemyAnimationStrings.IsAttacking);


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _surfacesChecker = GetComponent<WallChecker>();
        _detectPlayer = GetComponent<DetectPlayer>();
        _enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        _enemy.Died += StopMoving; 
    }

    private void OnDisable()
    {
        _enemy.Died -= StopMoving;
    }

    private void Update()
    {
        if (!_isTrapped)
        {
            if (_detectPlayer.PointToPlayer.isActiveAndEnabled)
            {
                LookAtTarget(_detectPlayer.PointToPlayer.transform.position);
                GoToTarget();
            }
            else
            {
                Move();
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

        if (collision.TryGetComponent(out PointToPlayer playerPoint) && _detectPlayer.IsSeePlayer == false)
        {
            playerPoint.gameObject.SetActive(false);
            StartStayCoroutine();
        }
    }

    public void GettingStuck()
    {
        _isTrapped = true;
        _animator.SetBool(EnemyAnimationStrings.IsTrapped, _isTrapped);
    }

    public void GetOutStuck()
    {
        _isTrapped = false;
        _animator.SetBool(EnemyAnimationStrings.IsTrapped, _isTrapped);
    }

    private void Move()
    {
        if (_isMoving)
        {
            _rigidbody.velocity = new Vector2(_speed * transform.localScale.x, _rigidbody.velocity.y);
        }
        else
        {
            _rigidbody.velocity = Vector2.zero;
        }

        _animator.SetBool(EnemyAnimationStrings.IsMoving, _isMoving);
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

    private void GoToTarget()
    {
        StopStayCoroutine();
        _isMoving = !_detectPlayer.IsHasTarget;
        _animator.SetBool(EnemyAnimationStrings.IsMoving, _isMoving);  

        if (_isMoving && !_isAttacking)
        {
            _rigidbody.velocity = new Vector2(_speed * transform.localScale.x, _rigidbody.velocity.y);
        }
    }

    private void TurnAround()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        _isFaceRight = !_isFaceRight;
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

    private IEnumerator Stay()
    {
        _isMoving = false;
        _animator.SetBool(EnemyAnimationStrings.IsMoving, _isMoving);
        yield return _stayTime;
        _isMoving = true;
        _animator.SetBool(EnemyAnimationStrings.IsMoving, _isMoving);
    }

    private void StopMoving()
    {
        enabled = false;
    }
}
