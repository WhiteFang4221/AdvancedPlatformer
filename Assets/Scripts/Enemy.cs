using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public static class DarkWizardAnimator
{
    public static class Params
    {
        public const string IsRunning = "IsRunning";
        public const string IsAttacking = "IsAttacking";
    }

    public static class States
    {
        public const string Idle = nameof(Idle);
        public const string WizardRun = nameof(WizardRun);
        public const string WizardAttack = nameof(WizardAttack);
    }
}
public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform _path;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private Transform _obstacleRayObject;
    [SerializeField] private Transform _playerPosition;
    [SerializeField] private LayerMask _playerDetectingMask;
    [SerializeField] private float _obstacleRayDistance;
    [SerializeField] private float _obstacleRayDistanceBehind;
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;

    private Transform _lastPlayerPositionPoint;
    private Animator _animator;
    private Transform[] _points;

    private int _currentPoint;

    private float _attackRadius;
    private float _minDistance = 1f;
    private float _minDistanceToPlayer = 2f;
    private float _timeWaiting = 2f;
    private float _timeWaitingPlayer = 5f;
    private float _waitTimer = 0;
    private float _waitTimerPlayer = 0;

    private bool _isStalkingPlayer = false;
    private bool _isSeePlayer = false;
    private bool _isCanMove = true;

    private void Start()
    {
        _attackRadius = _attackPoint.GetComponent<CircleCollider2D>().radius;
        _animator = GetComponent<Animator>();
        _points = new Transform[_path.childCount];

        for (int i = 0; i < _points.Length; i++)
        {
            _points[i] = _path.GetChild(i);
        }
    }

    private void Update()
    {
        DetectPlayer();
        DetectPlayerBehind();

        if (!_isStalkingPlayer)
        {
            Patrol();
        }
        else
        {
            GoPlayerPosition();
        }
    }

    private void Reflect(Transform target)
    {
        if (transform.position.x < target.position.x)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }

    private void GoToPoint(Transform target)
    {
        Reflect(target);
        transform.position = Vector2.MoveTowards(transform.position, target.position, _speed * Time.deltaTime);
        _animator.SetBool(DarkWizardAnimator.Params.IsRunning, true);
    }

    private void DetectPlayer()
    {
        RaycastHit2D obstacleHit = Physics2D.Raycast(_obstacleRayObject.transform.position, Vector2.right * transform.localScale, _obstacleRayDistance, _playerDetectingMask);

        if (obstacleHit.collider != null)
        {
            _isStalkingPlayer = true;
            _isSeePlayer = true;
            _lastPlayerPositionPoint.position = new Vector2(obstacleHit.transform.position.x, transform.position.y);
            Debug.DrawRay(_obstacleRayObject.transform.position, Vector2.right * obstacleHit.distance * transform.localScale, Color.red);
        }
        else
        {
            Debug.DrawRay(_obstacleRayObject.transform.position, Vector2.right * _obstacleRayDistance * transform.localScale, Color.green);
            _isSeePlayer = false;
        }
    }

    private void DetectPlayerBehind()
    {
        RaycastHit2D obstacleHit = Physics2D.Raycast(_obstacleRayObject.transform.position, Vector2.left * transform.localScale, _obstacleRayDistanceBehind, _playerDetectingMask);

        if (obstacleHit.collider != null)
        {
            _isStalkingPlayer = true;
            _lastPlayerPositionPoint.position = new Vector2(obstacleHit.transform.position.x, transform.position.y);
            Debug.DrawRay(_obstacleRayObject.transform.position, Vector2.left * obstacleHit.distance * transform.localScale, Color.red);
        }
        else
        {
            Debug.DrawRay(_obstacleRayObject.transform.position, Vector2.left * _obstacleRayDistanceBehind * transform.localScale, Color.green);
        }
    }

    private void Patrol()
    {
        Transform target = _points[_currentPoint];

        if (Vector2.Distance(transform.position, target.position) < _minDistance)
        {
            _animator.SetBool(DarkWizardAnimator.Params.IsRunning, false);

            if (_waitTimer >= _timeWaiting)
            {
                _currentPoint++;
                _waitTimer = 0;
            }
            else
            {
                _waitTimer += Time.deltaTime;
            }

            if (_currentPoint >= _points.Length)
            {
                _currentPoint = 0;
            }
        }
        else
        {
            GoToPoint(target);
        }
    }

    private void GoPlayerPosition()
    {
        if (Vector2.Distance(transform.position, _lastPlayerPositionPoint.transform.position) < _minDistanceToPlayer)
        {
            _animator.SetBool(DarkWizardAnimator.Params.IsRunning, false);

            if (_isSeePlayer)
            {
                _animator.SetBool(DarkWizardAnimator.Params.IsAttacking, true);
                _waitTimerPlayer = 0;
            }
            else
            {
                _animator.SetBool(DarkWizardAnimator.Params.IsAttacking, false);

                if (_waitTimerPlayer >= _timeWaitingPlayer)
                {
                    _isStalkingPlayer = false;
                    _waitTimerPlayer = 0;
                }
                else
                {
                    _waitTimerPlayer += Time.deltaTime;
                }
            }
        }
        else
        {
            _animator.SetBool(DarkWizardAnimator.Params.IsAttacking, false);

            if (_isCanMove)
            {
                GoToPoint(_lastPlayerPositionPoint);
            }
        }
    }

    private void AttackStarting()
    {
        _isCanMove = false;
    }

    private void AttackEnding()
    {
        _isCanMove = true;
    }

    private void Attack()
    {
        Collider2D player = Physics2D.OverlapCircle(_attackPoint.transform.position, _attackRadius, _playerDetectingMask);
        if (player != null)
        {
            player.GetComponent<PlayerController>().TakeHit(_damage);
        }
        
    }
}

