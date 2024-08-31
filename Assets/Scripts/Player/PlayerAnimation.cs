using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationState { idle, right, left, jump, fall };
public class PlayerAnimation : MonoBehaviour
{
    Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void UpdateAnimationState(AnimationState state)
    {
        switch (state)
        {
            case AnimationState.right:
                _animator.SetBool("Right", true);
                break;
            case AnimationState.left:
                _animator.SetBool("Left", true);
                break;
            case AnimationState.jump:
                _animator.SetBool("Jump", true);
                break;
            case AnimationState.fall:
                _animator.SetBool("Fall", true);
                break;
            default:
                ReturnToIdle();
                break;
        }
    }

    void ReturnToIdle()
    {
        _animator.SetBool("Fall", false);
        _animator.SetBool("Right", false);
        _animator.SetBool("Left", false);
        _animator.SetBool("Jump", false);
    }
}
