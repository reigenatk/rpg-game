using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimation : MonoBehaviour
{
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // this is basically saying, call SetAnimationParameters whenever the delegate is called (which will happen from the input class)
        EventHandler.MovementEvent += SetAnimationParameters;
    }

    private void OnDisable()
    {
        EventHandler.MovementEvent -= SetAnimationParameters;
    }

    // if movement occurs, then change the animation
    private void SetAnimationParameters(float inputX, float inputY, bool isWalking, bool isRunning,  bool isIdle, bool idleLeft, bool idleRight, bool idleUp, bool idleDown)
    {
        animator.SetFloat(Settings.xInput, inputX);
        animator.SetFloat(Settings.yInput, inputY);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);
        if (idleUp)
            animator.SetTrigger(Settings.idleUp);
        if (idleDown)
            animator.SetTrigger(Settings.idleDown);
        if (idleLeft)
            animator.SetTrigger(Settings.idleLeft);
        if (idleRight)
            animator.SetTrigger(Settings.idleRight);
    }


}
