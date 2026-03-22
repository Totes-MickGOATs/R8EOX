using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    internal class Chassis
    {
        private float mass;
        private Vector3 centerOfMass;
        private float dragCoefficient;

        internal void Initialize(float vehicleMass, Vector3 com)
        {
            mass = vehicleMass;
            centerOfMass = com;
        }

        internal void ApplyAerodynamics(float speed)
        {
            // TODO: Calculate and apply downforce and drag
        }

        internal float GetDownforce(float speed)
        {
            // TODO: Return downforce at current speed
            return 0f;
        }
    }
}
