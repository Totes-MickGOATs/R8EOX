using UnityEngine.InputSystem;

namespace R8EOX.Input.Internal
{
    internal class InputMapper
    {
        internal float MapThrottle(InputAction action)
        {
            // TODO: Read and normalize throttle value (0 to 1)
            return action?.ReadValue<float>() ?? 0f;
        }

        internal float MapBrake(InputAction action)
        {
            // TODO: Read and normalize brake value (0 to 1)
            return action?.ReadValue<float>() ?? 0f;
        }

        internal float MapSteering(InputAction action)
        {
            // TODO: Read and normalize steering value (-1 to 1)
            return action?.ReadValue<float>() ?? 0f;
        }
    }
}
