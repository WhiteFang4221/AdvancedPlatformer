using UnityEngine;

public class Attacker : MonoBehaviour
{
    [SerializeField] private AttackZone _attackZone;
    [SerializeField] private int _damage = 10;
    [SerializeField] private Vector2 _knockback;

    private Animator _animator;

    private void OnEnable()
    {
        _attackZone.TargetAttacked += DoDamage;
        _attackZone.TargetPushed += PushTarget;
    }

    private void OnDisable()
    {
        _attackZone.TargetAttacked -= DoDamage;
        _attackZone.TargetPushed -= PushTarget;
    }

    private void DoDamage(IDamageable target)
    {
        target.TakeDamage(_damage);
    }

    private void PushTarget(IPushable target)
    {
        Vector2 deliveredKnockback = transform.localScale.x > 0 ? _knockback : new Vector2(-_knockback.x, _knockback.y);
        target.PushOffOnHit(deliveredKnockback);
    }
}


