using UnityEngine;

public class HeadAnimationController : MonoBehaviour
{
    const string LandTrigger = "Land";
    const string HardLandTrigger = "HardLand";
    const string CrouchBool = "Crouch";

    [SerializeField] Animator animator = null;

    public void AnimateJump ()
    {
        
    }

    public void AnimateLand()
    {
        animator.SetTrigger(LandTrigger);
    }

    public void AnimateHardLand()
    {
        animator.SetTrigger(HardLandTrigger);
    }

    public void SetCrouch (bool isCrouchOn)
    {
        animator.SetBool (CrouchBool, isCrouchOn);
    }
}
