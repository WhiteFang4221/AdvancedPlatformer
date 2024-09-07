using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DetectPlayer : MonoBehaviour
{
    [SerializeField] private LayerMask _playerDetectingMask;
    [SerializeField] private RayObstacle _obstacleRayObject;
    [SerializeField] private DetectionZone _detectionZone;
    [SerializeField] private float _obstacleRayDistance = 10f;
    [SerializeField] private float _obstacleRayDistanceBehind = 5f;

    private Animator _animator;

    [field: SerializeField] public PointToPlayer PointToPlayer { get; private set; }
    public bool IsSeePlayer { get; private set; } = false;
    public bool IsHasTarget { get; private set; } = false;

    private void Awake()
    {
        PointToPlayer.gameObject.SetActive(false);
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Detect();

        if (IsSeePlayer == false)
        {
            DetectBehind();
        }

        IsHasTarget = _detectionZone.DetectedColliders.Count > 0;
        _animator.SetBool(EnemyAnimationStrings.IsHasTarget, IsHasTarget);
    }

    private void Detect()
    {
        RaycastHit2D obstacleHit = Physics2D.Raycast(_obstacleRayObject.transform.position, Vector2.right * transform.localScale, _obstacleRayDistance, _playerDetectingMask);

        if (obstacleHit.collider != null)
        {
            PointToPlayer.transform.position = new Vector2(obstacleHit.transform.position.x, transform.position.y);
            PointToPlayer.gameObject.SetActive(true);
            IsSeePlayer = true;
        }
        else
        {
            IsSeePlayer = false;
        }
    }

    private void DetectBehind()
    {
        RaycastHit2D obstacleHit = Physics2D.Raycast(_obstacleRayObject.transform.position, Vector2.left * transform.localScale, _obstacleRayDistanceBehind, _playerDetectingMask);

        if (obstacleHit.collider != null)
        {
            PointToPlayer.transform.position = new Vector2(obstacleHit.transform.position.x, transform.position.y);
            PointToPlayer.gameObject.SetActive(true);
        }
    }
}
