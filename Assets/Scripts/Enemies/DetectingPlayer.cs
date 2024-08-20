using UnityEngine;

[RequireComponent(typeof(Animator))]

public class DetectingPlayer : MonoBehaviour
{
    [SerializeField] private LayerMask _playerDetectingMask;
    [SerializeField] private PointToPlayer _pointToPlayer;
    [SerializeField] private Transform _obstacleRayObject;
    [SerializeField] private DetectionZone _detectionPlayerZone;
    [SerializeField] private float _obstacleRayDistance = 10f;
    [SerializeField] private float _obstacleRayDistanceBehind = 5f;
    [SerializeField] private bool _isHasTarget = false;

    private Animator _animator;
    private bool _isSeePlayer = false;

    public PointToPlayer PointToPlayer => _pointToPlayer;
    public bool IsSeePlayer => _isSeePlayer;
    public bool IsHasTarget
    {
        get
        {
            return _isHasTarget;
        }
        private set
        {
            _isHasTarget = value;
            _animator.SetBool(EnemyAnimationStrings.IsHasTarget, value);
        }
    }
    public bool IsAlive => _animator.GetBool(EnemyAnimationStrings.IsAlive);

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _pointToPlayer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (IsAlive)
        {
            DetectPlayer();

            if (_isSeePlayer == false)
            {
                DetectPlayerBehind();
            }

            IsHasTarget = _detectionPlayerZone.DetectedColliders.Count > 0;
        }
    }

    private void DetectPlayer()
    {
        RaycastHit2D obstacleHit = Physics2D.Raycast(_obstacleRayObject.transform.position, Vector2.right * transform.localScale, _obstacleRayDistance, _playerDetectingMask);

        if (obstacleHit.collider != null)
        {
            _pointToPlayer.transform.position = new Vector2(obstacleHit.transform.position.x, transform.position.y);
            _pointToPlayer.gameObject.SetActive(true);
            _isSeePlayer = true;
        }
        else
        {
            _isSeePlayer = false;
        }
    }

    private void DetectPlayerBehind()
    {
        RaycastHit2D obstacleHit = Physics2D.Raycast(_obstacleRayObject.transform.position, Vector2.left * transform.localScale, _obstacleRayDistanceBehind, _playerDetectingMask);

        if (obstacleHit.collider != null)
        {
            _pointToPlayer.transform.position = new Vector2(obstacleHit.transform.position.x, transform.position.y);
            _pointToPlayer.gameObject.SetActive(true);
        }
    }
}
