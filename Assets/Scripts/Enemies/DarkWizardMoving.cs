using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SurfacesChecker))]
[RequireComponent(typeof(DetectingPlayer))]

public class DarkWizardMoving : MonoBehaviour
{
    [SerializeField] private DetectingPlayer _detectionPlayer;
    [SerializeField] private float _moveSpeed = 9f;

    public enum WalkableDirection
    {
        right, left
    }

    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private EnemySurfacesChecker _surfacesChecker;
    private EnemyHealthManager _enemyHealthManager;
    private WaitForSeconds _stayTime = new WaitForSeconds(3);
    
    private bool _isMoving = true;
    private Vector2 _walkDirectionVector = Vector2.right;
    private WalkableDirection _walkDirection;
    
    public WalkableDirection WalkDirection
    {
        get
        {
            return _walkDirection;
        }
        private set
        {
            if (_walkDirection != value)
            {
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);

                if (value == WalkableDirection.left)
                {
                    _walkDirectionVector = Vector2.left;
                }
                else if (value == WalkableDirection.right)
                {
                    _walkDirectionVector = Vector2.right;
                }
            }

            _walkDirection = value;
        }
    }

    public bool IsMoving
    {
        get
        {
            if (_detectionPlayer.IsHasTarget == true || IsAlive == false)
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

    public bool IsAlive
    {
        get
        {
           return _animator.GetBool(EnemyStringsAnimator.IsAlive);
        }
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _surfacesChecker = GetComponent<EnemySurfacesChecker>();
        _detectionPlayer = GetComponent<DetectingPlayer>();
        _enemyHealthManager = GetComponent<EnemyHealthManager>();
    }

    private void OnEnable()
    {
        _enemyHealthManager.HitTaken += OnHit;
    }

    private void OnDisable()
    {
        _enemyHealthManager.HitTaken -= OnHit;
    }

    private void FixedUpdate()
    {
        if (_surfacesChecker.IsOnWall)
        {
            StartCoroutine(Stay());
        }

        if (IsMoving && _detectionPlayer.IsAttacking == false)
        {
            _rigidbody.velocity = new Vector2(_moveSpeed * _walkDirectionVector.x, _rigidbody.velocity.y);
        }
        else
        {
            _rigidbody.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyStayPoint point))
        {
            StartCoroutine(Stay());
        }
    }

    private IEnumerator Stay()
    {
        FlipDirection();
        IsMoving = false;
        yield return _stayTime;
        IsMoving = true;
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.left)
        {
            Debug.Log("ѕовернулс€ направо");
            WalkDirection = WalkableDirection.right;
        }
        else if (WalkDirection == WalkableDirection.right)
        {
            Debug.Log("ѕовернулс€ налево");
            WalkDirection = WalkableDirection.left;
        }
        else
        {
            Debug.LogError("не установлено значение влево или вправо");
        }
    }

    private void OnHit(int damage, Vector2 knockback)
    {
        _rigidbody.velocity = new Vector2(knockback.x, _rigidbody.velocity.y + knockback.y);
    }
}