using UnityEngine;

[RequireComponent (typeof(CapsuleCollider2D), typeof(Animator), typeof(CollisionDetector))]

public class SurfacesChecker : MonoBehaviour
{
    [SerializeField] private ContactFilter2D _castFilter;
    [SerializeField] private float _groundDistance = 0.05f;
    [SerializeField] private float _wallCheckDistance = 2f;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isOnWall;

    private CapsuleCollider2D _collider;
    private Animator _animator;
    private CollisionDetector _collisionDetector;

    private RaycastHit2D[] _groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] _wallHits = new RaycastHit2D[5];
    private Vector2 _wallCheckDirection => transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    public bool IsGrounded
    {
        get
        {
            return _isGrounded;
        }
        private set
        {
            _isGrounded = value;
            _animator.SetBool(PlayerAnimator.IsGrounded, _isGrounded);
        }
    }

    public bool IsOnWall
    {
        get
        {
            return _isOnWall;
        }
        private set
        {
            _isOnWall = value;
        }
    }

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();
        _collisionDetector = GetComponent<CollisionDetector>();
    }

    private void FixedUpdate()
    {
        if (_collisionDetector.IsIgnorePlatformCollider == true)
        {
            IsGrounded = false;
        }
        else
        {
            IsGrounded = _collider.Cast(Vector2.down, _castFilter, _groundHits, _groundDistance) > 0;
        }

        IsOnWall = _collider.Cast(_wallCheckDirection, _castFilter, _wallHits, _wallCheckDistance) > 0;
    }
}
