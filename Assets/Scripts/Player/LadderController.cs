using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController), typeof(Rigidbody2D), typeof(Animator))]
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

    public bool IsOnLadder
    {
        get
        {
            return _animator.GetBool(PlayerAnimator.Params.IsOnLadder);
        }
    }

    public Vector2 MoveInput
    {
        get
        {
            return _playerController.MoveInput;
        }
    }

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
                if (_isLadderCheck == false && _isBottomLadderCheck == true) // Сверху
                {
                    if (MoveInput.y > 0)
                    {
                        _animator.SetBool(PlayerAnimator.Params.IsOnLadder, false);
                    }
                    else if (MoveInput.y < 0)
                    {
                        _animator.SetBool(PlayerAnimator.Params.IsOnLadder, true);
                    }
                }
                else if (_isLadderCheck == true && _isBottomLadderCheck == true) // на лестнице
                {
                    if (MoveInput.y > 0)
                    {
                        _animator.SetBool(PlayerAnimator.Params.IsOnLadder, true) ;
                    }
                    else if (MoveInput.y < 0)
                    {
                        _animator.SetBool(PlayerAnimator.Params.IsOnLadder, true);
                    }
                }
                else if (_isLadderCheck == true && _isBottomLadderCheck == false) // внизу
                {
                    if (MoveInput.y > 0)
                    {
                        _animator.SetBool(PlayerAnimator.Params.IsOnLadder, true);
                    }
                    else if (MoveInput.y < 0)
                    {
                        _animator.SetBool(PlayerAnimator.Params.IsOnLadder, false);
                    }
                }
            }
            else
            {
                _animator.SetBool(PlayerAnimator.Params.IsOnLadder, false);
            }

            ChangeBodyTypeOnLadder();
            _animator.SetBool(PlayerAnimator.Params.IsOnLadder, IsOnLadder);
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
        if (IsOnLadder && _isLadderPositionCorrected == true)
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
