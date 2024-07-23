using UnityEngine;

[RequireComponent (typeof(CollisionDetector))]

public class PlayerSurfacesChecker : SurfacesChecker
{
    [SerializeField] private float _groundDistance = 0.05f;
    private RaycastHit2D[] _groundHits = new RaycastHit2D[5];
    private CollisionDetector _collisionDetector;
    private bool _isGrounded;

    public bool IsGrounded
    {
        get
        {
            return _isGrounded;
        }
        protected set
        {
            _isGrounded = value;
            Animator.SetBool(PlayerAnimator.IsGrounded, _isGrounded);
        }
    }

    private void Awake()
    {
        _collisionDetector = GetComponent<CollisionDetector>();
    }

    private void FixedUpdate()
    {
        if (_collisionDetector.IsIgnorePlatformCollider)
        {
            IsGrounded = false;
        }
        else
        {
            IsGrounded = ÑapsuleCollider.Cast(Vector2.down, ÑastFilter, _groundHits, _groundDistance) > 0;
        }
    }
}
