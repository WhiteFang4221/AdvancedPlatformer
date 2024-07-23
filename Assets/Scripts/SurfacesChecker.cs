using UnityEngine;

[RequireComponent (typeof(CapsuleCollider2D), typeof(Animator))]

abstract public class SurfacesChecker : MonoBehaviour
{
    [SerializeField] protected ContactFilter2D ÑastFilter;

    protected CapsuleCollider2D ÑapsuleCollider;
    protected Animator Animator;
     
    private void Start()
    {
        ÑapsuleCollider = GetComponent<CapsuleCollider2D>();
        Animator = GetComponent<Animator>();
    }
}
