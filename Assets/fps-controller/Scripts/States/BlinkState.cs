using FPSController.Input;
using UnityEngine;

namespace FPSController.State
{
    public class BlinkState : CharacterStateBase
    {
        [SerializeField] float speed = 100f;
        [SerializeField] float duration = 0.5f;
        [SerializeField] int maxBlinks = 3;

        [Tooltip ("How fast one blink charge will refill.")]
        [SerializeField] float refillRate = 0.05f;

        float durationTimer = 0;
        int blinkCounter = 0;

        Vector3 deltaPosition;

        public float FillValue;
        private void FixedUpdate ()
        {
            if (blinkCounter > 0)
            {
                FillValue += refillRate;

                if (FillValue >= 1)
                {
                    FillValue = 0f;
                    blinkCounter--;
                }
            }
        }

        protected override void onEnter ()
        {
            base.onEnter ();

            Vector3 velocity = parent.Velocity;
            velocity.y = 0;
            setVelocity (velocity);
            durationTimer = 0;
            blinkCounter++;
        }

        public bool CanBlinkNow ()
        {
            bool result = false;
            deltaPosition = getDeltaPosition ();

            if (deltaPosition.sqrMagnitude > 0.5 && blinkCounter < maxBlinks)
            {
                result = true;
            }

            return result;
        }

        public override void UpdateState ()
        {
            durationTimer += Time.deltaTime;

            if (durationTimer > duration)
            {
                if (parent.IsGrounded)
                {
                    requestNewState<GroundState> ();
                }
                else
                {
                    requestNewState<InAirState> ();
                }
            }
            else
            {
                move (deltaPosition * Time.deltaTime);
            }
        }

        protected Vector3 getDeltaPosition ()
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
            deltaPosition *= speed;
            deltaPosition.Scale (new Vector3 (1f, 0f, 1f));

            return deltaPosition;
        }
    }
}

