using UnityEngine;

[RequireComponent (typeof(CapsuleCollider2D), typeof(Animator), typeof(CollisionDetector))]
public class GroundChecker : MonoBehaviour
{
    [SerializeField] private ContactFilter2D _castFilter;    
    [SerializeField] private bool _isGrounded;

    private CollisionDetector _collisionDetector;
    private CapsuleCollider2D _collider;
    private Animator _animator;
    private RaycastHit2D[] _groundHits = new RaycastHit2D[5];
    private float _groundDistance = 0.05f;

    public bool IsGrounded
    {
        get
        {
            return _isGrounded;
        }
        private set
        {
            _isGrounded = value;
            _animator.SetBool(PlayerAnimator.Params.IsGrounded, _isGrounded);
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
    }
}
