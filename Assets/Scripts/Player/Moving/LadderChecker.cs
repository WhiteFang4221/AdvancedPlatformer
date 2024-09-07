using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]

public class LadderChecker : MonoBehaviour
{
    [SerializeField] private Transform _ladderChecker;
    [SerializeField] private Transform _bottomLadderChecker;
    [SerializeField] private LayerMask _ladderMask;

    private Animator _animator;
    private PlayerMoving _playerMoving;
    private Rigidbody2D _rigidbody;

    private bool _isLadderCheck;
    private bool _isBottomLadderCheck;
    private bool _isLadderPositionCorrected = true;
    private float _ladderCenter;

    public bool IsOnLadder => _animator.GetBool(PlayerAnimationStrings.IsOnLadder);

    public Vector2 MoveInput => _playerMoving.MoveInput;

    private void Awake()
    {
        _playerMoving = GetComponent<PlayerMoving>();
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
        if (_playerMoving.IsRolling == false)
        {
            if (_isLadderCheck || _isBottomLadderCheck)
            {
                if (!_isLadderCheck && _isBottomLadderCheck) // Сверху
                {
                    if (MoveInput.y > 0)
                    {
                        _animator.SetBool(PlayerAnimationStrings.IsOnLadder, false);
                        _rigidbody.velocity = Vector2.zero;
                    }
                    else if (MoveInput.y < 0)
                    {
                        _animator.SetBool(PlayerAnimationStrings.IsOnLadder, true);
                    }
                }
                else if (_isLadderCheck && _isBottomLadderCheck) // на лестнице
                {
                    if (MoveInput.y > 0)
                    {
                        _animator.SetBool(PlayerAnimationStrings.IsOnLadder, true) ;
                    }
                    else if (MoveInput.y < 0)
                    {
                        _animator.SetBool(PlayerAnimationStrings.IsOnLadder, true);
                    }
                }
                else if (_isLadderCheck && !_isBottomLadderCheck) // внизу
                {
                    if (MoveInput.y > 0)
                    {
                        _animator.SetBool(PlayerAnimationStrings.IsOnLadder, true);
                    }
                    else if (MoveInput.y < 0)
                    {
                        _animator.SetBool(PlayerAnimationStrings.IsOnLadder, false);
                    }
                }
            }
            else
            {
                _animator.SetBool(PlayerAnimationStrings.IsOnLadder, false);
            }

            ChangeBodyTypeOnLadder();
            _animator.SetBool(PlayerAnimationStrings.IsOnLadder, IsOnLadder);
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
