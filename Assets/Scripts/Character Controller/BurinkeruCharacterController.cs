using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CapsuleCollider))]
public class BurinkeruCharacterController : CharacterControllerBase
{
    BurinkeruInputManager inputManager;
    PlayerCrouchState crouchState;

    public static Vector3 MovementAxes
    {
        get { return new Vector3 (1f, 0f, 1f); }
    }

    public bool IsCrouching
    {
        get { return crouchState != null; }
    }

    public PlayerCrouchState CrouchState
    {
        get { return crouchState; }
    }

    void setListenersToWeapon (WeaponBase weapon)
    {
        if (weapon != null)
        {
            weapon.OnAddVelocityRequested += AddVelocity;
            weapon.OnSetVelocityRequested += SetVelocity;
        }
    }

    private void Awake ()
    {
        layerMaskToCheckForPushback = LayerMask.GetMask ("Default");
    }

    protected override void enterState (CharacterStateBase state, System.Type prevStateType)
    {
        if (state is PlayerState)
        {
            PlayerState playerCharacterControllerStateBase = (PlayerState) state;
            playerCharacterControllerStateBase.Enter (inputManager, this);

        }
    }

    // Start is called before the first frame update
    protected void Start ()
    {
        inputManager = BurinkeruInputManager.Instance;
        //SetNewState<PlayerInAirState> ();
    }

    // Update is called once per frame
    protected new void Update ()
    {
        base.Update ();
    }

    override protected void updateState ()
    {
        base.updateState ();

        if (crouchState != null)
        {
            //crouchState.UpdateState ();
        }
    }

    void onBlink ()
    {
        Velocity = Vector3.zero;
    }

    public void EnterCrouch ()
    {
        crouchState = new PlayerCrouchState ();
        //crouchState.Enter (inputManager, this);
    }

    public void ExitCrouch ()
    {
        if (crouchState != null)
        {
           // crouchState.Exit ();
            crouchState = null;
        }
    }

    public Vector3 GetLookDirection ()
    {
        return Vector3.zero;
    }

    public Vector3 GetUpwardDirection ()
    {
        return Vector3.zero;
    }

    public override float GetMovementSpeed ()
    {
        float result = CharacterControllerParameters.Instance.DefaultMoveSpeed;

        if (mainMovementState != null)
        {
            result *= mainMovementState.GetMovementSpeedFactor ();
        }

        if (crouchState != null && IsGrounded)
        {
            //result *= crouchState.GetMovementSpeedFactor ();
        }

        return result;
    }

    override protected void applyForces ()
    {
        float friction = 1f - GetMovementDrag ();
        Vector3 velocitySum = Velocity;
        Move (velocitySum * Time.deltaTime);
        Vector3 v = Velocity;
        v.Scale (new Vector3(friction, 1f, friction));
        Velocity = v;
    }
}
