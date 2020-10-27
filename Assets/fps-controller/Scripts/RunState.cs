using UnityEngine;

[RequireComponent (typeof (GroundState))]
public class RunState : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;

    public float RunSpeed
    {
        get { return runSpeed; }
    }
}
