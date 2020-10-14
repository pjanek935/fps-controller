using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlideState : SlideState
{
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
        deltaPosition.Scale (BurinkeruCharacterController.MovementAxes);

        return deltaPosition;
    }
}
