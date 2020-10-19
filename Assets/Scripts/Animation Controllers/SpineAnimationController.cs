using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void AnimateWallRunLeft ()
    {
        animator.SetTrigger ("Wall Run Left");
    }

    public void AnimateWallRunRight()
    {
        animator.SetTrigger ("Wall Run Right");
    }

    public void AnimateToDefaultPosition()
    {
        animator.SetTrigger ("Default");
    }
}
