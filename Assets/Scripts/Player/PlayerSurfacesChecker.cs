using UnityEngine;

[RequireComponent (typeof(CollisionDetector))]
public class PlayerSurfacesChecker : SurfacesChecker
{
    private CollisionDetector _collisionDetector;
    private RaycastHit2D[] _groundHits = new RaycastHit2D[5];
    [SerializeField] private float _groundDistance = 0.05f;
    [SerializeField] private bool _isGrounded;

    public bool IsGrounded
    {
        get
        {
            return _isGrounded;
        }
        protected set
        {
            _isGrounded = value;
            animator.SetBool(PlayerAnimator.IsGrounded, _isGrounded);
        }
    }

    private void Awake()
    {
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
            IsGrounded = capsuleCollider.Cast(Vector2.down, castFilter, _groundHits, _groundDistance) > 0;
        }
    }
}
