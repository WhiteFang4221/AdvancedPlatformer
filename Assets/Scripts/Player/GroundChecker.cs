using System;
using UnityEngine;

[RequireComponent (typeof(CapsuleCollider2D), typeof(CollisionDetector), typeof(Animator))]

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private ContactFilter2D _castFilter;
    [SerializeField] private float _groundDistance = 0.05f;
    private Animator _animator;
    private CapsuleCollider2D _capsuleCollider;
    private CollisionDetector _collisionDetector;
    private RaycastHit2D[] _groundHits = new RaycastHit2D[5];
    private bool _isGrounded;

    public bool IsGrounded => _isGrounded;


    private void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _collisionDetector = GetComponent<CollisionDetector>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (_collisionDetector.IsIgnorePlatformCollider)
        {
            _isGrounded = false;
        }
        else
        {
            _isGrounded = _capsuleCollider.Cast(Vector2.down, _castFilter, _groundHits, _groundDistance) > 0;
            _animator.SetBool(PlayerAnimationStrings.IsGrounded, IsGrounded);
        }
    }
}
