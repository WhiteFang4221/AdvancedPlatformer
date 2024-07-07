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

    public PointToPlayer PointToPlayer
    {
        get { return _pointToPlayer; }
    }
    public bool IsSeePlayer
    {
        get
        {
            return _isSeePlayer;
        }
    }

    public bool IsHasTarget
    {
        get
        {
            return _isHasTarget;
        }
        private set
        {
            _isHasTarget = value;
            _animator.SetBool(EnemyStringsAnimator.IsHasTarget, value);
        }
    }

    public bool IsAlive
    {
        get
        {
            return _animator.GetBool(EnemyStringsAnimator.IsAlive);
        }
    }

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
            Debug.DrawRay(_obstacleRayObject.transform.position, Vector2.right * obstacleHit.distance * transform.localScale, Color.red);
        }
        else
        {
            _isSeePlayer = false;
            Debug.DrawRay(_obstacleRayObject.transform.position, Vector2.right * _obstacleRayDistance * transform.localScale, Color.green);
        }
    }

    private void DetectPlayerBehind()
    {
        RaycastHit2D obstacleHit = Physics2D.Raycast(_obstacleRayObject.transform.position, Vector2.left * transform.localScale, _obstacleRayDistanceBehind, _playerDetectingMask);

        if (obstacleHit.collider != null)
        {
            _pointToPlayer.transform.position = new Vector2(obstacleHit.transform.position.x, transform.position.y);
            _pointToPlayer.gameObject.SetActive(true);
            Debug.DrawRay(_obstacleRayObject.transform.position, Vector2.left * obstacleHit.distance * transform.localScale, Color.red);
        }
        else
        {
            Debug.DrawRay(_obstacleRayObject.transform.position, Vector2.left * _obstacleRayDistanceBehind * transform.localScale, Color.green);
        }
    }
}
