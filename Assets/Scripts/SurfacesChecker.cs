using UnityEngine;

[RequireComponent (typeof(CapsuleCollider2D), typeof(Animator))]

abstract public class SurfacesChecker : MonoBehaviour
{
    [SerializeField] protected ContactFilter2D castFilter;

    protected CapsuleCollider2D capsuleCollider;
    protected Animator animator;
     
    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }
}
