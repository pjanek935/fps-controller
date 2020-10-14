using System;
using UnityEngine;

[RequireComponent (typeof (CapsuleCollider))]
[RequireComponent (typeof (PlayerGroundState))]
[RequireComponent (typeof (PlayerInAirState))]
public class CharacterControllerBase : MonoBehaviour
{
    [SerializeField] Camera fpsCamera;

    protected CharacterStateBase mainMovementState;
    protected int layerMaskToCheckForPushback = 0;
    protected Vector3 deltaPositionToMoveInCurrentFrame = Vector3.zero;
    protected bool preciseCollisionCalucations = true;
    public const int CollisionCalicationsPrecision = 20; //the bigger the number more precise the caluclation

    public CapsuleCollider CapsuleCollider
    {
        get { return GetComponent <CapsuleCollider> (); }
    }

    public bool IsGrounded
    {
        get;
        protected set;
    }

    public Vector3 PrevDeltaPosition
    {
        get;
        private set;
    }

    public Vector3 Velocity
    {
        get;
        protected set;
    }

    public Vector3 GetLookDirection ()
    {
        return fpsCamera.transform.forward;
    }

    private void Start ()
    {
        layerMaskToCheckForPushback = LayerMask.GetMask ("Default");
        setNewState<PlayerInAirState> ();
    }

    protected void Update ()
    {
        Vector3 startPos = transform.position;

        updateState ();
        applyForces ();
        updatePosition ();
        updateIsGrounded ();
        checkForPushback ();
        setNewStateIfNeeded ();

        PrevDeltaPosition = transform.position - startPos;
    }

    public virtual float GetMovementSpeed ()
    {
        return 1f;
    }

    public void Move (Vector3 deltaPosition)
    {
        deltaPositionToMoveInCurrentFrame += deltaPosition;
    }

    public void AddVelocity (Vector3 velocityDelta)
    {
        Velocity += velocityDelta;
    }

    public void SetVelocity (Vector3 newVelocty)
    {
        Velocity = newVelocty;
    }

    public virtual float GetMovementDrag ()
    {
        float result = 1f;

        if (mainMovementState != null)
        {
            result = mainMovementState.GetMovementDrag ();
        }

        return result;
    }

    protected virtual void updateState ()
    {
        if (mainMovementState != null)
        {
            mainMovementState.UpdateState ();
        }
    }

    void setNewStateIfNeeded ()
    {
        if (mainMovementState != null && mainMovementState.NewRequestedState != null)
        {
            setNewState (mainMovementState.NewRequestedState);
        }
    }

    void updatePosition ()
    {
        if (preciseCollisionCalucations)
        {
            Vector3 d = deltaPositionToMoveInCurrentFrame / (float) CollisionCalicationsPrecision;

            for (int i = 0; i < CollisionCalicationsPrecision; i++)
            {
                transform.position += d;
                checkForPushback ();
            }

            deltaPositionToMoveInCurrentFrame = Vector3.zero;
        }
        else
        {
            transform.position += deltaPositionToMoveInCurrentFrame;
            deltaPositionToMoveInCurrentFrame = Vector3.zero;
        }
    }

    void updateIsGrounded ()
    {
        if (CapsuleCollider == null)
        {
            return;
        }

        Vector3 sphereCastPos = transform.position - Vector3.up * CapsuleCollider.height / 4f + Vector3.up * CapsuleCollider.center.y;
        float sphereCastRadius = CapsuleCollider.radius;
        Collider [] colliders = Physics.OverlapSphere (sphereCastPos, sphereCastRadius, layerMaskToCheckForPushback);
        IsGrounded = false;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders [i] != CapsuleCollider)
            {
                Vector3 contactPoint = colliders [i].GetClosestPoint (sphereCastPos);
                Vector3 contactDirectionVector = contactPoint - sphereCastPos;
                Vector3 pushVector = sphereCastPos - contactPoint;
                transform.position += Vector3.ClampMagnitude (pushVector, Mathf.Clamp (sphereCastRadius - pushVector.magnitude, 0, sphereCastRadius));

                if (!(Mathf.Abs (contactDirectionVector.y) < 0.1f
                    || Mathf.Abs (contactDirectionVector.x) > 0.4f
                    || Mathf.Abs (contactDirectionVector.z) > 0.4)) //TODO magic numbers
                {
                    IsGrounded = true;
                }
            }
        }
    }

    bool checkForPushback ()
    {
        bool result = false;
        
        if (CapsuleCollider == null)
        {
            return false;
        }

        Collider [] colliders = Physics.OverlapSphere (transform.position, CapsuleCollider.radius, layerMaskToCheckForPushback);
        Vector3 contactPoint = Vector3.zero;

        for (int i = 0; i < colliders.Length; i++)
        {
            result = true;
            contactPoint = colliders [i].GetClosestPoint (transform.position);
            makePushback (contactPoint);
        }

        return result;
    }

    void makePushback (Vector3 contactPoint)
    {
        if (CapsuleCollider == null)
        {
            return;
        }

        Vector3 pushVector = transform.position - contactPoint;
        transform.position += Vector3.ClampMagnitude (pushVector,
            Mathf.Clamp (CapsuleCollider.radius - pushVector.magnitude, 0, CapsuleCollider.radius));
    }

    protected void setNewState<T> () where T : CharacterStateBase
    {
        if (mainMovementState == null || typeof (T) != mainMovementState.GetType ())
        {
            System.Type previousStateType = null;

            if (mainMovementState != null)
            {
                previousStateType = mainMovementState.GetType ();
            }

            T newState = GetComponent<T> ();

            if (newState != null)
            {
                if (mainMovementState != null)
                {
                    exitState (mainMovementState);
                }

                mainMovementState = newState;
                enterState (newState, previousStateType);

                Debug.Log ("new state: " + newState.GetType ().ToString ());
            }
        }
    }

    protected void setNewState (System.Type stateType)
    {
        if (mainMovementState == null || stateType != mainMovementState.GetType ())
        {
            System.Type prevStateType = null;

            if (mainMovementState != null)
            {
                prevStateType = mainMovementState.GetType ();
            }

            Component newState = GetComponent (stateType); 

            if (newState != null)
            {
                if (mainMovementState != null)
                {
                    exitState (mainMovementState);
                }

                mainMovementState = (CharacterStateBase) newState;
                enterState (mainMovementState, prevStateType);

                Debug.Log ("new state: " + newState.GetType ().ToString ());
            }
        }
    }

    protected virtual void enterState (CharacterStateBase state, System.Type previousStateType)
    {
        state.Enter (previousStateType);
    }

    protected void exitState (CharacterStateBase state)
    {
        state.Exit ();
    }

    protected virtual void applyForces ()
    {
        float friction = 1f - GetMovementDrag ();
        Vector3 velocitySum = Velocity;
        Move (velocitySum * Time.deltaTime);
        Vector3 v = Velocity;
        v.Scale (new Vector3 (friction, 1f, friction));
        Velocity = v;

        //Debug.Log ("Apply forces: " + Velocity);
    }
}
