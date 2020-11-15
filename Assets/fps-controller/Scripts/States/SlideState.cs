using UnityEngine;

namespace FPSController.State
{
    [RequireComponent (typeof (GroundState))]
    [RequireComponent (typeof (CrouchState))]
    public class SlideState : CharacterStateBase
    {
        [SerializeField] protected float initialSlideSpeed = 20f;
        [SerializeField] protected float movementDrag = 0.01f;
        [SerializeField] protected float movementSpeed = 5f;
        [SerializeField] protected float minMovementSpeed = 0.5f;

        [SerializeField]
        [Tooltip ("Below this slide speed" +
            " slide state will exit to grund state.")]
        protected float exitToGroundStateSqrSpeed = 1f;

        [Tooltip ("Above this speed instead of transioning " +
            "from in air state to ground state, player will transition to slide state.")]
        [SerializeField] protected float fromInAirMinSqrSpeed = 5f;

        GroundState groundState;

        protected void Awake ()
        {
            groundState = GetComponent<GroundState> ();
        }

        public Vector3 DeltaPosition
        {
            get;
            protected set;
        }

        public float InitialSlideSpeed
        {
            get { return initialSlideSpeed; }
        }

        public float Movementdrag
        {
            get { return movementDrag; }
        }

        public float FromInAirMinSqrSpeed
        {
            get { return fromInAirMinSqrSpeed; }
        }

        protected override void onEnter ()
        {
            base.onEnter ();

            Vector3 direction = parent.Velocity;
            direction.Normalize ();
            direction.Scale (new Vector3 (1f, 0f, 1f));
            direction *= initialSlideSpeed;
            setVelocity (direction);
        }

        public override void UpdateState ()
        {
            updateDeltaPosition ();

            if (!parent.IsGrounded)
            {
                setVelocity (DeltaPosition);
                requestNewState<InAirState> ();
            }
            else if (parent.Velocity.sqrMagnitude < exitToGroundStateSqrSpeed)
            {
                requestNewState<GroundState> ();
            }
            else
            {
                move ();
            }
        }

        void move ()
        {
            Vector3 deltaPosition = DeltaPosition * movementSpeed;

            if (deltaPosition.sqrMagnitude > minMovementSpeed)
            {
                setVelocity (Vector3.Lerp (parent.Velocity, deltaPosition, Time.deltaTime));
            }
        }

        void updateDeltaPosition ()
        {
            DeltaPosition = getDeltaPosition ();
        }

        protected virtual Vector3 getDeltaPosition ()
        {
            return Vector3.zero;
        }

        public override float GetMovementDrag ()
        {
            return movementDrag;
        }

        protected void jump ()
        {
            Vector3 newVelocity = DeltaPosition + parent.Velocity;
            newVelocity.y = Mathf.Sqrt (groundState.JumpHeight * 2f * groundState.Gravity);
            setVelocity (newVelocity);
            requestNewState<PlayerInAirState> ();
        }
    }
}
