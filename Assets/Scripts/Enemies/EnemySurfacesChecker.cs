using UnityEngine;

public class EnemySurfacesChecker : SurfacesChecker
{
    [SerializeField] private bool _isOnWall;
    [SerializeField] private float _wallCheckDistance = 2f;

    private Vector2 _wallCheckDirection => transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    private RaycastHit2D[] _wallHits = new RaycastHit2D[5];

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

    private void FixedUpdate()
    {
        IsOnWall = capsuleCollider.Cast(_wallCheckDirection, castFilter, _wallHits, _wallCheckDistance) > 0;
    }
}
