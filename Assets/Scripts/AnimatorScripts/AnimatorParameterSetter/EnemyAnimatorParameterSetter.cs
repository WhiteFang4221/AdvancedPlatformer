using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimatorParameters
{
    public class EnemyAnimatorParameterSetter : AnimatorParameterSetter
    {
        private protected override void SetHitTrigger()
        {
            _animator.SetTrigger(EnemyAnimationStrings.HitTrigger);
        }

        private protected override void SetIsAlive(bool isAlive)
        {
            _animator.SetBool(EnemyAnimationStrings.IsAlive, isAlive);
        }
    }
}
