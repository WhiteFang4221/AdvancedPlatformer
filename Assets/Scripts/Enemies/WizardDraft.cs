//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class NewBehaviourScript : MonoBehaviour
//{
//    [SerializeField] private Transform _path;
//    [SerializeField] private Transform _attackPoint;
//    [SerializeField] private Transform _obstacleRayObject;
//    [SerializeField] private LayerMask _playerDetectingMask;
//    [SerializeField] private float _obstacleRayDistance;
//    [SerializeField] private float _obstacleRayDistanceBehind;

//    private Vector2 _lastPlayerPositionPoint;
//    private Transform[] _patrolPoints;
//    private int _patrolCurrentPoint;

//    private float _attackRadius;
//    private float _minDistanceToPoint = 1f;
//    private float _minDistanceToPlayer = 2f;
//    private float _patrolWaitTime = 2f;
//    private float _patrolTimer = 0;
//    private float _timeWaitingPlayer = 5f;
//    private float _waitTimerPlayer = 0;

//    private bool _isStalkingPlayer = false;
//    private bool _isSeePlayer = false;
//    private bool _isCanMove = true;

//    private void Start()
//    {
//        _attackRadius = _attackPoint.GetComponent<CircleCollider2D>().radius;
//        _animator = GetComponent<Animator>();
//        _patrolPoints = new Transform[_path.childCount];

//        for (int i = 0; i < _patrolPoints.Length; i++)
//        {
//            _patrolPoints[i] = _path.GetChild(i);
//        }
//    }

//    private void Update()
//    {
//        DetectPlayer();

//        if (_isSeePlayer == false)
//        {
//            DetectPlayerBehind();
//        }

//        if (_isStalkingPlayer == false)
//        {
//            Patrol();
//        }
//        else
//        {
//            GoPlayerPosition();
//        }
//    }

//    private void Reflect(Vector2 target)
//    {
//        if (transform.position.x < target.x)
//        {
//            transform.localScale = new Vector2(1, 1);
//        }
//        else
//        {
//            transform.localScale = new Vector2(-1, 1);
//        }
//    }

//    private void GoToPoint(Vector2 target)
//    {
//        transform.position = Vector2.MoveTowards(transform.position, target, _speed * Time.deltaTime);
//        _animator.SetBool(DarkWizardAnimator.Params.IsRunning, true);
//    }

//    private void DetectPlayer()
//    {
//        RaycastHit2D obstacleHit = Physics2D.Raycast(_obstacleRayObject.transform.position, Vector2.right * transform.localScale, _obstacleRayDistance, _playerDetectingMask);

//        if (obstacleHit.collider != null)
//        {
//            _isStalkingPlayer = true;
//            _isSeePlayer = true;
//            _lastPlayerPositionPoint = new Vector2(obstacleHit.transform.position.x, transform.position.y);
//            Debug.DrawRay(_obstacleRayObject.transform.position, Vector2.right * obstacleHit.distance * transform.localScale, Color.red);
//        }
//        else
//        {
//            Debug.DrawRay(_obstacleRayObject.transform.position, Vector2.right * _obstacleRayDistance * transform.localScale, Color.green);
//            _isSeePlayer = false;
//        }
//    }

//    private void DetectPlayerBehind()
//    {
//        RaycastHit2D obstacleHit = Physics2D.Raycast(_obstacleRayObject.transform.position, Vector2.left * transform.localScale, _obstacleRayDistanceBehind, _playerDetectingMask);

//        if (obstacleHit.collider != null)
//        {
//            _isStalkingPlayer = true;
//            _lastPlayerPositionPoint = new Vector2(obstacleHit.transform.position.x, transform.position.y);
//            Reflect(_lastPlayerPositionPoint);
//            Debug.DrawRay(_obstacleRayObject.transform.position, Vector2.left * obstacleHit.distance * transform.localScale, Color.red);
//        }
//        else
//        {
//            Debug.DrawRay(_obstacleRayObject.transform.position, Vector2.left * _obstacleRayDistanceBehind * transform.localScale, Color.green);
//        }
//    }

//    private void Patrol()
//    {
//        Vector2 target = _patrolPoints[_patrolCurrentPoint].position;
//        Reflect(target);

//        if (Vector2.Distance(transform.position, target) < _minDistanceToPoint)
//        {
//            _animator.SetBool(DarkWizardAnimator.Params.IsRunning, false);

//            if (_patrolTimer >= _patrolWaitTime)
//            {
//                _patrolCurrentPoint++;
//                _patrolTimer = 0;
//            }
//            else
//            {
//                _patrolTimer += Time.deltaTime;
//            }

//            if (_patrolCurrentPoint >= _patrolPoints.Length)
//            {
//                _patrolCurrentPoint = 0;
//            }
//        }
//        else
//        {
//            GoToPoint(target);
//        }
//    }

//    private void GoPlayerPosition()
//    {
//        if (Vector2.Distance(transform.position, _lastPlayerPositionPoint) < _minDistanceToPlayer)
//        {
//            _animator.SetBool(DarkWizardAnimator.Params.IsRunning, false);

//            if (_isSeePlayer)
//            {
//                _animator.SetBool(DarkWizardAnimator.Params.IsAttacking, true);
//                _waitTimerPlayer = 0;
//            }
//            else
//            {
//                _animator.SetBool(DarkWizardAnimator.Params.IsAttacking, false);
//                WaitOnLastPlayerPosition();
//            }
//        }
//        else
//        {
//            _animator.SetBool(DarkWizardAnimator.Params.IsAttacking, false);

//            if (_isCanMove)
//            {
//                GoToPoint(_lastPlayerPositionPoint);
//            }
//        }
//    }

//    private void WaitOnLastPlayerPosition()
//    {
//        _waitTimerPlayer += Time.deltaTime;

//        if (_waitTimerPlayer >= _timeWaitingPlayer)
//        {
//            _isStalkingPlayer = false;
//            _waitTimerPlayer = 0;
//        }
//    }

//    private void AttackStarting()
//    {
//        _isCanMove = false;
//    }

//    private void AttackEnding()
//    {
//        _isCanMove = true;
//    }

//    private void Attack()
//    {
//        Collider2D player = Physics2D.OverlapCircle(_attackPoint.transform.position, _attackRadius, _playerDetectingMask);

//        if (player != null)
//        {
//            player.GetComponent<PlayerHealth>().TakeDamage(_damage);
//        }
//    }
//}
