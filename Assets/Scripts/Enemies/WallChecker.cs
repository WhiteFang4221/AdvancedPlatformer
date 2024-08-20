using UnityEngine;

public class WallChecker : MonoBehaviour
{
    [SerializeField] private ContactFilter2D _castFilter;
    [SerializeField] private float _wallCheckDistance = 2f;

    private CapsuleCollider2D _capsuleCollider;
    private RaycastHit2D[] _wallHits = new RaycastHit2D[5];
    private Vector2 WallCheckDirection => transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    [field: SerializeField] public bool IsOnWall { get; private set; }

    private void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        IsOnWall = _capsuleCollider.Cast(WallCheckDirection, _castFilter, _wallHits, _wallCheckDistance) > 0;
    }
}
