using UnityEngine;

public class CameraFOVAnimator : MonoBehaviour
{
    [SerializeField] Animator animator = null;

    public void IncreaseFOV ()
    {
        animator.SetBool ("FOV Increased", true);
    }

    public void SetDefaultFOV ()
    {
        animator.SetBool ("FOV Increased", false);
    }
}
