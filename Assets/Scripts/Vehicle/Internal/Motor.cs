using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    internal class Motor
    {
        private float currentRPM;
        private float maxRPM;
        private float idleRPM;
        private AnimationCurve torqueCurve;

        internal void Initialize(float maxRpm, float idleRpm, AnimationCurve curve)
        {
            maxRPM = maxRpm;
            idleRPM = idleRpm;
            torqueCurve = curve;
        }

        internal void Tick(float throttle, float deltaTime)
        {
            // TODO: Update RPM based on throttle and load
        }

        internal float GetTorqueAtCurrentRPM()
        {
            // TODO: Sample torque curve at current RPM
            return 0f;
        }

        internal float GetCurrentRPM()
        {
            return currentRPM;
        }
    }
}
