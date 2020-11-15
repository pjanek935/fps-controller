using UnityEngine;

namespace FPSController.State
{
    public class CrouchState : MonoBehaviour
    {
        [SerializeField] float crouchSpeed = 4f;

        public float CrouchSpeed
        {
            get { return crouchSpeed; }
        }
    }
}