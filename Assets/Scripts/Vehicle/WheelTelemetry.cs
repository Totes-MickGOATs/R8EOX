using UnityEngine;

namespace R8EOX.Vehicle
{
    public readonly struct WheelTelemetry
    {
        public readonly Vector3 ContactPoint;     // World position where tire touches ground
        public readonly Vector3 ContactNormal;    // Surface normal at contact
        public readonly float SlipRatio;          // 0-1 lateral slip magnitude
        public readonly float GripFactor;         // 0-1 from grip curve evaluation
        public readonly float WheelRpm;           // Angular velocity in RPM
        public readonly float SuspensionForce;    // Newtons of spring force
        public readonly bool IsOnGround;          // Grounded this frame
        public readonly int SurfaceLayer;         // Physics layer index of contacted surface

        public WheelTelemetry(
            Vector3 contactPoint, Vector3 contactNormal,
            float slipRatio, float gripFactor, float wheelRpm,
            float suspensionForce, bool isOnGround, int surfaceLayer)
        {
            ContactPoint = contactPoint; ContactNormal = contactNormal;
            SlipRatio = slipRatio; GripFactor = gripFactor; WheelRpm = wheelRpm;
            SuspensionForce = suspensionForce; IsOnGround = isOnGround; SurfaceLayer = surfaceLayer;
        }
    }
}
