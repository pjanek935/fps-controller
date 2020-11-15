using FPSController.Input;
using UnityEngine;

namespace FPSController.State
{
    public class PlayerGroundState : GroundState
    {
        public override void UpdateState ()
        {
            BurinkeruInputManager inputManager = BurinkeruInputManager.Instance;

            if (inputManager.IsCommandDown (BurinkeruInputManager.InputCommand.JUMP))
            {
                jump ();
            }
            else if (inputManager.IsCommandDown (BurinkeruInputManager.InputCommand.RUN) && IsRunStateAvailable)
            {
                switchRunIfNeeded ();
            }
            else if (inputManager.IsCommandDown (BurinkeruInputManager.InputCommand.CROUCH) && IsCrounchStateAvailable)
            {
                switchCrouchOrSlideIfNeeded ();
            }
            else if (inputManager.IsCommandDown (BurinkeruInputManager.InputCommand.BLINK) &&
                IsBlinkStateAvailable &&
                blinkState.CanBlinkNow ())
            {
                requestNewState<BlinkState> ();
            }

            base.UpdateState ();
        }

        protected override Vector3 getDeltaPosition ()
        {
            Vector3 deltaPosition = Vector3.zero;
            Vector3 forwardDirection = parent.transform.forward;
            Vector3 rightDirection = parent.transform.right;
            BurinkeruInputManager inputManager = BurinkeruInputManager.Instance;

            if (inputManager.IsCommandPressed (BurinkeruInputManager.InputCommand.FORWARD))
            {
                deltaPosition += (forwardDirection);
            }
            else if (inputManager.IsCommandPressed (BurinkeruInputManager.InputCommand.BACKWARD))
            {
                deltaPosition -= (forwardDirection);
            }

            if (inputManager.IsCommandPressed (BurinkeruInputManager.InputCommand.RIGHT))
            {
                deltaPosition += (rightDirection);
            }
            else if (inputManager.IsCommandPressed (BurinkeruInputManager.InputCommand.LEFT))
            {
                deltaPosition -= (rightDirection);
            }

            deltaPosition.Normalize ();
            deltaPosition *= getMovementSpeed ();
            deltaPosition.Scale (new Vector3 (1f, 0f, 1f));

            return deltaPosition;
        }
    }

}