using UnityEngine;

[RequireComponent(typeof(PlayerController), typeof(Rigidbody2D), typeof(Animator))]
[RequireComponent (typeof(SpriteRenderer))]

public class LadderController : MonoBehaviour
{
    [SerializeField] private Transform _ladderChecker;
    [SerializeField] private Transform _bottomLadderChecker;
    [SerializeField] private LayerMask _ladderMask;

    private Animator _animator;
    private PlayerController _playerController;
    private Rigidbody2D _rigidbody;

    private bool _isLadderCheck;
    private bool _isBottomLadderCheck;
    private bool _isLadderPositionCorrected = true;
    private float _ladderCenter;

    public bool IsOnLadder => _animator.GetBool(PlayerAnimator.IsOnLadder);

    public Vector2 MoveInput => _playerController.MoveInput;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckLadder();
        FindPositionOnLadder();
    }

    private void CheckLadder()
    {
        _isLadderCheck = Physics2D.OverlapPoint(_ladderChecker.position, _ladderMask);
        _isBottomLadderCheck = Physics2D.OverlapPoint(_bottomLadderChecker.position, _ladderMask);
        CatchLadder();
    }

    private void CatchLadder()
    {
        if (_playerController.IsRolling == false)
        {
            if (_isLadderCheck || _isBottomLadderCheck)
            {
                if (!_isLadderCheck && _isBottomLadderCheck) // Сверху
                {
                    if (MoveInput.y > 0)
                    {
                        _animator.SetBool(PlayerAnimator.IsOnLadder, false);
                    }
                    else if (MoveInput.y < 0)
                    {
                        _animator.SetBool(PlayerAnimator.IsOnLadder, true);
                    }
                }
                else if (_isLadderCheck && _isBottomLadderCheck) // на лестнице
                {
                    if (MoveInput.y > 0)
                    {
                        _animator.SetBool(PlayerAnimator.IsOnLadder, true) ;
                    }
                    else if (MoveInput.y < 0)
                    {
                        _animator.SetBool(PlayerAnimator.IsOnLadder, true);
                    }
                }
                else if (_isLadderCheck && !_isBottomLadderCheck) // внизу
                {
                    if (MoveInput.y > 0)
                    {
                        _animator.SetBool(PlayerAnimator.IsOnLadder, true);
                    }
                    else if (MoveInput.y < 0)
                    {
                        _animator.SetBool(PlayerAnimator.IsOnLadder, false);
                    }
                }
            }
            else
            {
                _animator.SetBool(PlayerAnimator.IsOnLadder, false);
            }

            ChangeBodyTypeOnLadder();
            _animator.SetBool(PlayerAnimator.IsOnLadder, IsOnLadder);
        }
    }

    private void ChangeBodyTypeOnLadder()
    {
        if (IsOnLadder)
        {
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;

        }
    }

    private void FindPositionOnLadder()
    {
        if (IsOnLadder && _isLadderPositionCorrected)
        {
            _isLadderPositionCorrected = false;
            _rigidbody.velocity = Vector2.zero;
            ChangePositionOnLadder();
        }
        else if (!IsOnLadder && !_isLadderPositionCorrected)
        {
            _isLadderPositionCorrected = true;
        }
    }

    private void ChangePositionOnLadder()
    {
        if (_isLadderCheck)
        {
            _ladderCenter = Physics2D.OverlapPoint(_ladderChecker.position, _ladderMask).GetComponent<BoxCollider2D>().bounds.center.x;
        }
        else if (_bottomLadderChecker)
        {
            _ladderCenter = Physics2D.OverlapPoint(_bottomLadderChecker.position, _ladderMask).GetComponent<BoxCollider2D>().bounds.center.x;
        }

        transform.position = new Vector2(_ladderCenter, transform.position.y);
    }
}
