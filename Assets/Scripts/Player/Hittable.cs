using System;
using UnityEngine;

public class Hittable : MonoBehaviour
{
    public event Action<int> HitTaken;

    public void TakeDamage(int damage, Vector2 knockback)
    {
        HitTaken?.Invoke(damage);
    }

    private void OnHit(Vector2 knockback)
    {

    }
}
