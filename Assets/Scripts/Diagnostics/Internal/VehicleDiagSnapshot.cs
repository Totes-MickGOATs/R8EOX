#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

namespace R8EOX.Diagnostics.Internal
{
    internal struct VehicleDiagSnapshot
    {
        public float SpeedKmh;
        public float SteerAngle;
        public float ThrottleInput;
        public float BrakeInput;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
        public bool IsGrounded;
        public int GroundedWheelCount;
        public float SuspensionTravel;
        public float SlipAngle;
        public float SlipRatio;

        public static VehicleDiagSnapshot Empty => new VehicleDiagSnapshot();
    }
}
#endif
