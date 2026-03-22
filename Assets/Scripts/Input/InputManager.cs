using UnityEngine;
using UnityEngine.InputSystem;

namespace R8EOX.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;

        private InputAction throttleAction;
        private InputAction brakeAction;
        private InputAction steeringAction;

        private void Awake()
        {
            // TODO: Resolve input actions from PlayerInput
        }

        public float GetThrottle()
        {
            // TODO: Read throttle input value
            return 0f;
        }

        public float GetBrake()
        {
            // TODO: Read brake input value
            return 0f;
        }

        public float GetSteering()
        {
            // TODO: Read steering input value
            return 0f;
        }
    }
}
