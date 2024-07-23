using UnityEngine;

[RequireComponent (typeof(CapsuleCollider2D), typeof(Animator))]

abstract public class SurfacesChecker : MonoBehaviour
{
    [SerializeField] protected ContactFilter2D �astFilter;

    protected CapsuleCollider2D �apsuleCollider;
    protected Animator Animator;
     
    private void Start()
    {
        �apsuleCollider = GetComponent<CapsuleCollider2D>();
        Animator = GetComponent<Animator>();
    }
}
