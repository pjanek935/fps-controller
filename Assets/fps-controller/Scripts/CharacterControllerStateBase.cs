using UnityEngine;

[RequireComponent (typeof (CharacterControllerBase))]
public abstract class CharacterStateBase : MonoBehaviour
{
    public System.Type PreviousStateType
    {
        get;
        protected set;
    }

    protected CharacterControllerBase parent
    {
        get;
        private set;
    }

    public System.Type NewRequestedState
    {
        get;
        protected set;
    }

    protected void OnEnable ()
    {
        parent = GetComponent<CharacterControllerBase> ();
    }

    public void Enter (System.Type previousStateType)
    {
        this.PreviousStateType = previousStateType;
        NewRequestedState = null;
        onEnter ();
    }

    public void Exit ()
    {
        NewRequestedState = null;
        onExit ();
    }

    protected virtual void requestNewState <T> () where T : CharacterStateBase
    {
        NewRequestedState = typeof (T);
    }

    protected void addVelocity (Vector3 velocityDelta)
    {
        parent.AddVelocity (velocityDelta);
    }

    protected void setVelocity (Vector3 newVelocity)
    {
        parent.SetVelocity (newVelocity);
    }

    protected void move (Vector3 deltaPosition)
    {
        parent.Move (deltaPosition);
    }

    protected virtual void onEnter () { }
    protected virtual void onExit () { }
    public virtual float GetMovementDrag () { return 1f; }
    public virtual float GetMovementSpeedFactor () { return 1f; }
    public abstract void UpdateState ();
}
