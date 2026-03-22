using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    internal class Wheel
    {
        private float grip;
        private float suspensionCompression;
        private float angularVelocity;

        internal void Tick(float deltaTime)
        {
            // TODO: Update wheel physics, suspension, grip
        }

        internal float GetGrip()
        {
            return grip;
        }

        internal void ApplySteering(float steerAngle)
        {
            // TODO: Apply steering angle to this wheel
        }

        internal void ApplyBrake(float brakeForce)
        {
            // TODO: Apply braking force
        }

        internal void ApplyDriveTorque(float torque)
        {
            // TODO: Apply drive torque from motor
        }
    }
}
