using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    internal static class WheelSolveStage
    {
        public static void Execute(
            ref VehicleFrame frame,
            Rigidbody rb,
            WheelManager wheels)
        {
            foreach (var w in wheels.All)
            {
                w.IsBraking = frame.BrakeForce > 0f && w.IsMotor;
                w.ApplyWheelPhysics(rb, frame.Dt);

                if (w.IsSteer)
                {
                    w.transform.localRotation = Quaternion.Euler(
                        0f, frame.CurrentSteering * Mathf.Rad2Deg, 0f);
                }
            }
        }
    }
}
