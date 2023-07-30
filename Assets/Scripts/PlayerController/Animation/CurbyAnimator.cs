using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class CurbyAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerController _controller;
    
    
    private static readonly int MSpeed = Animator.StringToHash("mSpeed");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsDashing = Animator.StringToHash("isDashing");
    private static readonly int IsPlanting = Animator.StringToHash("IsPlanting");

    void Awake()
    {
        if (!_animator)
        {
            Debug.LogError("No Animator was set up noob!", gameObject);
        }

        _controller = GetComponent<PlayerController>();

        // _controller.jumpAction.started += context => _animator.SetBool(IsJumping, true);
        // _controller.jumpAction.canceled += context => _animator.SetBool(IsJumping, false);

        // _controller.dashAction.started += context => _animator.SetBool(IsDashing, true);
        // _controller.dashAction.canceled += context => _animator.SetBool(IsDashing, false);
        
        _controller.interactAction.started += context => _animator.SetTrigger(IsPlanting);
        
        
    }

    void Update()
    {
        if (!_animator)
            return;
        
        _animator.SetFloat(MSpeed, _controller.CurrentMovementSpeed);
        _animator.SetBool(IsJumping, _controller.IsJumping);
        _animator.SetBool(IsDashing, _controller.IsDashing);
    }
}
