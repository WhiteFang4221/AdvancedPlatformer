using UnityEngine;

[RequireComponent (typeof(CapsuleCollider2D), typeof(SpriteRenderer))]

public class CollisionDetector : MonoBehaviour
{
    private CapsuleCollider2D _capsuleCollider;
    private SpriteRenderer _spriteRenderer;
    private int _minOrder = 3;
    private int _defaultOrder = 7;

    public bool IsIgnorePlatformCollider { get; private set; } = false;

    private void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Platform platform))
        {
            Physics2D.IgnoreCollision(collision.GetComponent<EdgeCollider2D>(), _capsuleCollider, true);
            IsIgnorePlatformCollider = true;
            _spriteRenderer.sortingOrder = _minOrder;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Platform platform))
        {
            Physics2D.IgnoreCollision(collision.GetComponent<EdgeCollider2D>(), _capsuleCollider, false);
            IsIgnorePlatformCollider = false;
            _spriteRenderer.sortingOrder = _defaultOrder;
        }
    }
}
