using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : CharacterStateBase
{
    protected BurinkeruInputManager inputManager;

    public BurinkeruCharacterController BurinkeruCharacterController
    {
        get { return (BurinkeruCharacterController) parent; }
    }

    public void Enter (BurinkeruInputManager inputManager, BurinkeruCharacterController parent)
    {
        this.inputManager = inputManager;

       // base.Enter (parent);
    }

    protected virtual void switchCrouch()
    {
        //BurinkeruCharacterController burinkeruCharacterController = (BurinkeruCharacterController) Parent;

        //if (burinkeruCharacterController.IsCrouching)
        //{
        //    burinkeruCharacterController.ExitCrouch();
        //}
        //else
        //{
        //    burinkeruCharacterController.EnterCrouch();
        //}
    }
}
