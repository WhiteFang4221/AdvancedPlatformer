using System.Collections;
using UnityEngine;

[RequireComponent (typeof(Animator))]
abstract public class Enemy : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] private  int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private LayerMask _deathMask;
    private bool isDead = false;


    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger(EnemyAnimationStrings.Params.HitTrigger);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        gameObject.layer = _deathMask;
        StartCoroutine(Despawn());
    }


    private IEnumerator Despawn()
    {
        animator.SetBool(EnemyAnimationStrings.Params.IsDead, isDead);
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
