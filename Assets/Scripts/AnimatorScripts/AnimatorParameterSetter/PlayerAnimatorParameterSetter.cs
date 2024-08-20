using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimatorParameters
{
    public class PlayerAnimatorParameterSetter : AnimatorParameterSetter
    {
        private protected override void SetHitTrigger()
        {
            _animator.SetTrigger(PlayerAnimationStrings.HitTrigger);
        }

        private protected override void SetIsAlive(bool isAlive)
        {
            _animator.SetBool(PlayerAnimationStrings.IsAlive, isAlive);
        }
    }
}

