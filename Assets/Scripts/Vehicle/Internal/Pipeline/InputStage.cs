using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    internal static class InputStage
    {
        public static void Execute(
            ref VehicleFrame frame,
            R8EOX.Input.IVehicleInput input,
            Rigidbody rb, Transform vehicleTransform,
            float prevSmoothThrottle,
            float throttleRampUp, float throttleRampDown)
        {
            frame.ThrottleRaw = input != null ? input.Throttle : 0f;
            frame.BrakeInput   = input != null ? input.Brake   : 0f;
            frame.SteerInput   = input != null ? input.Steer   : 0f;

            float rampRate = frame.ThrottleRaw > prevSmoothThrottle
                ? throttleRampUp : throttleRampDown;
            frame.SmoothThrottle = Mathf.MoveTowards(
                prevSmoothThrottle, frame.ThrottleRaw, rampRate * frame.Dt);

            frame.ForwardSpeed = Vector3.Dot(rb.linearVelocity, vehicleTransform.forward);
        }
    }
}
