using UnityEngine;

[RequireComponent (typeof (CharacterControllerBase))]
public class BasicMovementAnimationsController : MonoBehaviour
{
    [SerializeField] HeadAnimationController headAnimationController;
    [SerializeField] SpineAnimationController spineAnimationController;
    [SerializeField] CameraFOVAnimator cameraFOVAnimator;

    [SerializeField] float minYVelocityToAnimateLanding = 1f;

    CharacterControllerBase characterController;
    GroundState groundState;

    private void Awake ()
    {
        characterController = GetComponent<CharacterControllerBase> ();
        groundState = GetComponent<GroundState> ();
    }
    private void OnEnable ()
    {
        characterController.OnStateChanged += onStateChanged;

        if (groundState != null)
        {
            groundState.OnGroundInternalStateChanged += onGroundStateInternalStateChaged;
        }
    }

    private void OnDisable ()
    {
        characterController.OnStateChanged -= onStateChanged;

        if (groundState != null)
        {
            groundState.OnGroundInternalStateChanged -= onGroundStateInternalStateChaged;
        }
    }

    void onStateChanged (CharacterStateBase newState, CharacterStateBase prevState)
    {
        if (typeof (InAirState).IsAssignableFrom (prevState.GetType ()) && typeof (GroundState).IsAssignableFrom (newState.GetType ()))
        {
            InAirState inAirState = (InAirState) prevState;

            if (Mathf.Abs (inAirState.LastInAirVelocity.y) > minYVelocityToAnimateLanding)
            {
                headAnimationController.AnimateLand ();
            }
        }

        if (typeof (WallRunState).IsAssignableFrom (newState.GetType ()))
        {
            WallRunState wallRunState = (WallRunState) newState;

            switch (wallRunState.WallRunSide)
            {
                case WallRunState.WallRunType.LEFT:

                    spineAnimationController.AnimateWallRunLeft ();

                    break;

                case WallRunState.WallRunType.RIGH:

                    spineAnimationController.AnimateWallRunRight ();

                    break;
            }
        }

        if (typeof (WallRunState).IsAssignableFrom (prevState.GetType ()))
        {
            spineAnimationController.AnimateToDefaultPosition ();
        }
        else if (typeof (GroundState).IsAssignableFrom (newState.GetType ()))
        {
            GroundState groundState = (GroundState) newState;
            onGroundStateInternalStateChaged (groundState.InternalState);
        }
        else if (typeof (SlideState).IsAssignableFrom (newState.GetType ()))
        {
            headAnimationController.SetCrouch (true);
        }
    }

    void onGroundStateInternalStateChaged (GroundState.GroundStateInternalState groundStateInternalState)
    {
        bool isCrouchOn = groundStateInternalState == GroundState.GroundStateInternalState.CROUCH;
        headAnimationController.SetCrouch (isCrouchOn);
        bool fovIncreased = groundStateInternalState == GroundState.GroundStateInternalState.RUN;
        
        if (fovIncreased)
        {
            cameraFOVAnimator.IncreaseFOV ();
        }
        else 
        {
            cameraFOVAnimator.SetDefaultFOV ();
        }
    }
}
