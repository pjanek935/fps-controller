using UnityEngine;

namespace FPSController.Animation
{
    public class SpineAnimationController : MonoBehaviour
    {
        [SerializeField] Animator animator = null;

        public void AnimateWallRunLeft ()
        {
            animator.SetTrigger ("Wall Run Left");
        }

        public void AnimateWallRunRight ()
        {
            animator.SetTrigger ("Wall Run Right");
        }

        public void AnimateToDefaultPosition ()
        {
            animator.SetTrigger ("Default");
        }
    }
}