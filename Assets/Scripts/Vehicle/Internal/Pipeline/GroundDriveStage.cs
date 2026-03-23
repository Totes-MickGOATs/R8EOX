using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    internal static class GroundDriveStage
    {
        const float k_ReverseSpeedThreshold = 0.25f;
        const float k_ForwardSpeedClearThreshold = 0.50f;
        const float k_ReverseBrakeMinThreshold = 0.1f;

        public static void Execute(
            ref VehicleFrame frame,
            Rigidbody rb, Transform vehicleTransform,
            WheelManager wheels,
            float engineForceMax, float brakeForce, float reverseForce,
            float coastDrag, float maxSpeed)
        {
            if (frame.IsAirborne)
            {
                frame.EngineForce = 0f;
                frame.BrakeForce = 0f;
                frame.CoastDragForce = 0f;
                foreach (var w in wheels.All)
                    w.MotorForceShare = 0f;
                return;
            }

            var r = ESCMath.ComputeGroundDrive(
                frame.SmoothThrottle, frame.BrakeInput, frame.ForwardSpeed,
                frame.ReverseEngaged,
                engineForceMax, brakeForce, reverseForce,
                coastDrag, maxSpeed, rb.linearVelocity.magnitude,
                k_ReverseSpeedThreshold, k_ForwardSpeedClearThreshold,
                k_ReverseBrakeMinThreshold);

            frame.EngineForce = r.EngineForce;
            frame.BrakeForce = r.BrakeForce;
            frame.CoastDragForce = r.CoastDragForce;
            frame.ReverseEngaged = r.ReverseEngaged;

            if (r.CoastDragForce > 0f)
                rb.AddForce(-vehicleTransform.forward * r.CoastDragForce, ForceMode.Force);
        }
    }
}
