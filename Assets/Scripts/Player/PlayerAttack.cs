using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerMoving))]
public class PlayerAttack : MonoBehaviour
{
    private Animator _animator;
    private PlayerMoving _playerMoving;

    [field: SerializeField] public int Damage { get; private set; } = 10;
    [field: SerializeField] public Vector2 Knockback { get; private set; } = Vector2.zero;
    public bool IsAttacking => _animator.GetBool(PlayerAnimationStrings.IsAttacking);

    private void OnEnable()
    {
        _playerMoving.Attacked += Attack;
    }

    private void OnDisable()
    {
        _playerMoving.Attacked -= Attack;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMoving = GetComponent<PlayerMoving>();
    }        

    private void Attack()
    {
        _animator.SetTrigger(PlayerAnimationStrings.AttackTrigger);
    }
}

