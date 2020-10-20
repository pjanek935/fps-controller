public class PlayerWallRunState : WallRunState
{
    public override void UpdateState ()
    {
        base.UpdateState ();

        if (BurinkeruInputManager.Instance.IsCommandUp (BurinkeruInputManager.InputCommand.JUMP))
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
    }
}
