using UnityEngine;

public class AttackZone : MonoBehaviour
{
    [SerializeField] private int _damage = 10;
    [SerializeField] private Vector2 _knockback = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthManager healthManager = collision.GetComponent<HealthManager>();

        if (healthManager != null)
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? _knockback : new Vector2(-_knockback.x, _knockback.y);
            healthManager.TakeDamage(_damage, deliveredKnockback);
        }
    }
}
