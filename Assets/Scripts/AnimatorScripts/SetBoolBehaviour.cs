using UnityEngine;

public class SetBoolBehaviour : StateMachineBehaviour
{
    [SerializeField] private string _boolName;
    [SerializeField] private bool _updateOnStateMachine;
    [SerializeField] private bool _updateOnState;
    [SerializeField] private bool _valueOnEnter;
    [SerializeField] private bool _valueOnExit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_updateOnState)
        {
            animator.SetBool(_boolName, _valueOnEnter);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_updateOnState)
        {
            animator.SetBool(_boolName, _valueOnExit);
        }
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (_updateOnStateMachine)
        {
            animator.SetBool(_boolName, _valueOnEnter);
        }
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (_updateOnStateMachine)
        {
            animator.SetBool(_boolName, _valueOnExit);
        }
    }
}
