using UnityEngine;
public class AttackZone : MonoBehaviour
{
    [SerializeField] private int _damage = 10;
    [SerializeField] private Vector2 _knockback = Vector2.zero;
    private Collider2D _attackCollider;

    private void Awake()
    {
        _attackCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthManager healthManager = collision.GetComponent<HealthManager>();

        if (healthManager != null)
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? _knockback : new Vector2(-_knockback.x, _knockback.y);
            healthManager.TakeDamage(_damage, deliveredKnockback);
            Debug.Log(collision.name + "Получил Урон " +  _damage);
        }
    }
}
