using UnityEngine;

[RequireComponent (typeof (GroundState))]
public class RunState : MonoBehaviour
{
    [SerializeField] float runSpeed;

    public float RunSpeed
    {
        get { return runSpeed; }
    }
}
