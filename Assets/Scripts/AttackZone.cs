using HealthSystem;
using System;
using UnityEngine;

public class AttackZone : MonoBehaviour
{
    public event Action<Health> TargetAttacked;
    public event Action<IPushable> TargetPushed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Health target))
        {
            TargetAttacked?.Invoke(target);
        }

        if (collision.TryGetComponent(out IPushable pushableObject))
        {
            TargetPushed?.Invoke(pushableObject);
        }
    }
}