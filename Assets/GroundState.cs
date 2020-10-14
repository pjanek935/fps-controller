using UnityEngine;

[RequireComponent (typeof (CharacterControllerBase))]
public class GroundState : CharacterStateBase
{
    public enum GroundStateInternalState
    {
        NONE, RUN, CROUCH,
    }

    [SerializeField] protected float movementDrag = 1f;
    [SerializeField] protected float jumpHeight = 5f;
    [SerializeField] protected float maxHorizontalVelocity = 5f;
    [SerializeField] protected float gravity = 10f;
    [SerializeField] protected float movementSpeed = 4f;

    protected RunState runState;
    protected CrouchState crouchState;
    protected SlideState slideState;
    protected BlinkState blinkState;

    public GroundStateInternalState InternalState
    {
        get;
        protected set;
    }

    protected new void OnEnable ()
    {
        base.OnEnable ();

        runState = GetComponent<RunState> ();
        crouchState = GetComponent <CrouchState> ();
        slideState = GetComponent<SlideState> ();
        blinkState = GetComponent<BlinkState> ();
    }

    public bool IsRunStateAvailable
    {
        get { return runState != null; }
    }

    public bool IsCrounchStateAvailable
    {
        get { return crouchState != null; }
    }

    public bool IsSlideStateAvailable
    {
        get { return slideState != null; }
    }

    public bool IsBlinkStateAvailable
    {
        get { return blinkState != null; }
    }

    public Vector3 DeltaPosition
    {
        get;
        private set;
    }

    public override float GetMovementSpeedFactor ()
    {
        return movementSpeed;
    }

    public override float GetMovementDrag ()
    {
        return movementDrag;
    }

    protected void updateDeltaPosition ()
    {
        DeltaPosition = getDeltaPosition ();
    }

    public override void UpdateState ()
    {
        if (! parent.IsGrounded)
        {
            setVelocity (DeltaPosition);
            requestNewState<PlayerInAirState> ();
        }
        else
        {
            updateDeltaPosition ();
            move (DeltaPosition * Time.deltaTime);

            if (InternalState == GroundStateInternalState.RUN)
            {
                if (DeltaPosition.sqrMagnitude < 0.5)
                {
                    InternalState = GroundStateInternalState.NONE;
                }
            }
        }
    }

    protected override void onEnter ()
    {
        base.onEnter ();

        InternalState = GroundStateInternalState.NONE;
        Vector3 currentVelocity = parent.Velocity;
        currentVelocity.y = 0;
        setVelocity (currentVelocity);
    }

    protected override void onExit ()
    {
        base.onExit ();

        InternalState = GroundStateInternalState.NONE;
    }

    protected void jump ()
    {
        Vector3 newVelocity = DeltaPosition;
        newVelocity.y = Mathf.Sqrt (jumpHeight * 2f * gravity);
        setVelocity (newVelocity);
        requestNewState<PlayerInAirState> ();
    }

    protected virtual Vector3 getDeltaPosition ()
    {
        return Vector3.one;
    }

    protected float getMovementSpeed ()
    {
        float result = movementSpeed;

        if (InternalState == GroundStateInternalState.RUN && IsRunStateAvailable)
        {
            result = runState.RunSpeed;
        }
        else if (InternalState == GroundStateInternalState.CROUCH && IsCrounchStateAvailable)
        {
            result = crouchState.CrouchSpeed;
        }

        return result;
    }
    
    protected void switchRunIfNeeded ()
    {
        if (IsRunStateAvailable)
        {
            switch (InternalState)
            {
                case GroundStateInternalState.RUN:

                    InternalState = GroundStateInternalState.NONE;

                    break;

                case GroundStateInternalState.NONE:
                case GroundStateInternalState.CROUCH:

                    InternalState = GroundStateInternalState.RUN;

                    break;
            }
        }
    }

    protected void switchCrouchOrSlideIfNeeded ()
    {
        if (IsCrounchStateAvailable)
        {
            switch (InternalState)
            {
                case GroundStateInternalState.CROUCH:

                    InternalState = GroundStateInternalState.NONE;

                    break;

                case GroundStateInternalState.NONE:

                    InternalState = GroundStateInternalState.CROUCH;

                    break;

                case GroundStateInternalState.RUN:

                    if (IsSlideStateAvailable)
                    {
                        InternalState = GroundStateInternalState.NONE;
                        setVelocity (DeltaPosition);
                        requestNewState<SlideState> ();
                    }
                    else
                    {
                        InternalState = GroundStateInternalState.CROUCH;
                    }

                    break;
            }
        }
    }
}
