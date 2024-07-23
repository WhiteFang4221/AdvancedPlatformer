using UnityEngine;

public class EnemySurfacesChecker : SurfacesChecker
{
    [SerializeField] private bool _isOnWall;
    [SerializeField] private float _wallCheckDistance = 2f;

    private RaycastHit2D[] _wallHits = new RaycastHit2D[5];
    private Vector2 WallCheckDirection => transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    public bool IsOnWall => _isOnWall;

    private void FixedUpdate()
    {
        _isOnWall = ÑapsuleCollider.Cast(WallCheckDirection, ÑastFilter, _wallHits, _wallCheckDistance) > 0;
    }
}
