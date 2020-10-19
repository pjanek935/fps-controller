﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CharacterControllerBase))]
public class BasicMovementAnimationsController : MonoBehaviour
{
    [SerializeField] HeadAnimationController headAnimationController;
    [SerializeField] SpineAnimationController spineAnimationController;

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
            Debug.Log ("y vel:" + inAirState.LastInAirVelocity.y);
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
    }

    void onGroundStateInternalStateChaged (GroundState.GroundStateInternalState groundStateInternalState)
    {

    }
}
