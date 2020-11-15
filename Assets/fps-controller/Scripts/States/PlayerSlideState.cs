using FPSController.Input;
using UnityEngine;

namespace FPSController.State
{
    public class PlayerSlideState : SlideState
    {
        BurinkeruInputManager inputManager;

        protected new void Awake ()
        {
            base.Awake ();

            inputManager = BurinkeruInputManager.Instance;
        }

        public override void UpdateState ()
        {
            base.UpdateState ();

            if (inputManager.IsCommandDown (BurinkeruInputManager.InputCommand.JUMP))
            {
                jump ();
            }
        }

        protected override Vector3 getDeltaPosition ()
        {
            Vector3 forwardDirection = parent.transform.forward;
            Vector3 rightDirection = parent.transform.right;
            Vector3 deltaPosition = Vector3.zero;
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
            deltaPosition.Scale (new Vector3 (1f, 0f, 1f));

            return deltaPosition;
        }
    }
}