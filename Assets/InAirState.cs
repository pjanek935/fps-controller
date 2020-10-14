using UnityEngine;

[RequireComponent (typeof (CharacterControllerBase))]
public class InAirState : CharacterStateBase
{
    [SerializeField] protected int maxJumpsInAir = 1;
    [SerializeField] protected float maxHorizontalVelocity = 5f;
    [SerializeField] protected float gravity = 10f;
    [SerializeField] protected float movementDrag = 2f;
    [SerializeField] protected float jumpHeight = 10f;
    [SerializeField] protected float movementSpeed = 5f;

    int jumpCounter = 0;
    protected SlideState slideState;
    protected WallRunState wallRunState;
    protected BlinkState blinkState;

    public float Gravity
    {
        get { return gravity; }
    }

    public float JumpHeight
    {
        get { return jumpHeight; }
    }

    public bool IsSlideStateAvailable
    {
        get { return slideState != null; }
    }

    public bool IsWallRunStateAvailable
    {
        get { return wallRunState != null; }
    }

    public bool IsBlinkStateAvailable
    {
        get { return blinkState != null; }
    }

    protected new void OnEnable ()
    {
        base.OnEnable ();

        slideState = GetComponent<SlideState> ();
        wallRunState = GetComponent<WallRunState> ();
        blinkState = GetComponent<BlinkState> ();
    }

    protected override void onEnter ()
    {
        //if (PreviousStateType != null && 
        //    typeof (WallRunState).IsAssignableFrom (PreviousStateType) && 
        //    jumpCounter == 0)
        //{
        //    jumpCounter = -1;
        //}
        //else
        //{
        //    jumpCounter = 0;
        //}

        jumpCounter = 0;
    }

    protected override void onExit () { }

    public override float GetMovementSpeedFactor ()
    {
        return movementSpeed;
    }

    public override float GetMovementDrag ()
    {
        return movementDrag;
    }

    public override void UpdateState ()
    {
        applyGravity ();
        changeStateIfNeeded ();
    }

    protected void jump ()
    {
        jumpCounter++;
        float velocityY = Mathf.Sqrt (jumpHeight * 2f * gravity);
        Vector3 currentVelocity = parent.Velocity;
        currentVelocity.y = velocityY;
        setVelocity (currentVelocity);
    }

    protected bool tryToJump ()
    {
        bool result = false;

        if (jumpCounter < maxJumpsInAir)
        {
            result = true;
            jump ();
        }

        return result;
    }

    protected virtual void changeStateIfNeeded ()
    {
        if (parent.IsGrounded)
        {
            Vector3 horizontalVelicity = parent.Velocity;
            horizontalVelicity.Scale (new Vector3 (1f, 0f, 1f));

            if (IsSlideStateAvailable)
            {
                if (horizontalVelicity.sqrMagnitude > slideState.FromInAirMinSqrSpeed)
                {
                    requestNewState<SlideState> ();
                }
                else
                {
                    requestNewState<GroundState> ();
                }
            }
            else
            {
                requestNewState<GroundState> ();
            }
        }
    }

    protected void applyGravity ()
    {
        float gravity = -this.gravity * Time.deltaTime;
        addVelocity (new Vector3 (0f, gravity, 0));
    }
}
