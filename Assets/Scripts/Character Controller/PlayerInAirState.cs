using UnityEngine;

public class PlayerInAirState : InAirState
{
    [SerializeField] float minMovementSpeedMagnitude = 0.5f;
    bool jumpPressed = false;

    protected override void onEnter ()
    {
        jumpPressed = false;

        base.onEnter ();
    }

    public override void UpdateState()
    {
        changeStateIfNeeded ();
        applyGravity ();
        move ();

        BurinkeruInputManager inputManager = BurinkeruInputManager.Instance;

        if (inputManager.IsCommandDown (BurinkeruInputManager.InputCommand.JUMP))
        {
            if (IsWallRunStateAvailable)
            {
                wallRunState.StartInitializationTimer ();
            }

            jumpPressed = true;
        }
        else if (inputManager.IsCommandUp (BurinkeruInputManager.InputCommand.JUMP))
        {
            wallRunState.StopInitializationTimer ();

            if (jumpPressed)
            {
                jumpPressed = false;
                tryToJump ();
            }
        }

        if (IsWallRunStateAvailable && wallRunState.ShouldStartWallRunState (parent))
        {
            wallRunState.StopInitializationTimer ();
            requestNewState<WallRunState> ();
        }
        else if (inputManager.IsCommandDown (BurinkeruInputManager.InputCommand.BLINK) &&
           IsBlinkStateAvailable &&
           blinkState.CanBlinkNow ())
        {
            requestNewState<BlinkState> ();
        }

        clampHorizontalVelocity ();
    }

    void clampHorizontalVelocity ()
    {
        Vector3 velocity = parent.Velocity;

        if (velocity.magnitude > maxHorizontalVelocity)
        {
            velocity = maxHorizontalVelocity * velocity.normalized;
            velocity.y = parent.Velocity.y;
            setVelocity (velocity);
        }
    }

    void move ()
    {
        Vector3 deltaPosition = getMoveDirection ();
        deltaPosition *= movementSpeed;

        if (deltaPosition.sqrMagnitude > minMovementSpeedMagnitude)
        {
            setVelocity (Vector3.Lerp (parent.Velocity, deltaPosition, Time.deltaTime));
        }
    }

    Vector3 getMoveDirection ()
    {
        Vector3 forwardDirection = parent.transform.forward;
        Vector3 rightDirection = parent.transform.right;
        Vector3 deltaPosition = Vector3.zero;
        BurinkeruInputManager inputManager = BurinkeruInputManager.Instance;

        if (inputManager.IsCommandPressed(BurinkeruInputManager.InputCommand.FORWARD))
        {
            deltaPosition += (forwardDirection);
        }
        else if (inputManager.IsCommandPressed(BurinkeruInputManager.InputCommand.BACKWARD))
        {
            deltaPosition -= (forwardDirection);
        }

        if (inputManager.IsCommandPressed(BurinkeruInputManager.InputCommand.RIGHT))
        {
            deltaPosition += (rightDirection);
        }
        else if (inputManager.IsCommandPressed(BurinkeruInputManager.InputCommand.LEFT))
        {
            deltaPosition -= (rightDirection);
        }

        deltaPosition.Normalize();
        deltaPosition.Scale(BurinkeruCharacterController.MovementAxes);

        return deltaPosition;
    }
}
