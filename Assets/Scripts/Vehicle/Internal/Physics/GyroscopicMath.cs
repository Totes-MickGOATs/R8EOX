using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    /// <summary>
    /// Pure math functions for gyroscopic precession and reaction torque.
    /// Two equations produce emergent aerial behavior:
    ///   1. Gyroscopic torque: tau = omega_body x (I * omega_spin * axis)
    ///   2. Reaction torque: tau = -axis * I * d(omega)/dt
    /// </summary>
    internal static class GyroscopicMath
    {
        /// <summary>
        /// Compute gyroscopic precession torque from body rotation and wheel spin.
        /// tau_gyro = omega_body x (I_wheel * omega_spin * spin_axis)
        /// This couples yaw to pitch and pitch to yaw -- NOT damping.
        /// </summary>
        /// <param name="bodyAngularVelocity">Body angular velocity in world space (rad/s)</param>
        /// <param name="wheelSpinAxis">Wheel spin axis in world space (unit vector)</param>
        /// <param name="wheelMoI">Wheel moment of inertia (kg*m^2)</param>
        /// <param name="wheelAngularVelocity">Wheel spin rate (rad/s)</param>
        /// <returns>Gyroscopic torque vector in world space (N*m)</returns>
        public static Vector3 ComputeGyroscopicTorque(
            Vector3 bodyAngularVelocity, Vector3 wheelSpinAxis,
            float wheelMoI, float wheelAngularVelocity)
        {
            Debug.Assert(wheelMoI >= 0f, "Wheel MoI must be non-negative");

            Vector3 angularMomentum = wheelSpinAxis * (wheelMoI * wheelAngularVelocity);
            return Vector3.Cross(bodyAngularVelocity, angularMomentum);
        }

        /// <summary>
        /// Compute reaction torque from wheel spin rate change (Newton's 3rd law).
        /// tau_reaction = -spin_axis * I_wheel * delta_omega / delta_time
        /// Throttle accelerates wheels -> nose pitches up.
        /// Brake decelerates wheels -> nose pitches down.
        /// </summary>
        /// <param name="wheelSpinAxis">Wheel spin axis in world space (unit vector)</param>
        /// <param name="wheelMoI">Wheel moment of inertia (kg*m^2)</param>
        /// <param name="currentSpinRate">Current wheel spin rate (rad/s)</param>
        /// <param name="previousSpinRate">Previous frame wheel spin rate (rad/s)</param>
        /// <param name="deltaTime">Time step (seconds)</param>
        /// <returns>Reaction torque vector in world space (N*m)</returns>
        public static Vector3 ComputeReactionTorque(
            Vector3 wheelSpinAxis, float wheelMoI,
            float currentSpinRate, float previousSpinRate, float deltaTime)
        {
            Debug.Assert(wheelMoI >= 0f, "Wheel MoI must be non-negative");
            Debug.Assert(deltaTime > 0f, "Delta time must be positive");

            float deltaOmega = currentSpinRate - previousSpinRate;
            float angularAcceleration = deltaOmega / deltaTime;
            return -wheelSpinAxis * (wheelMoI * angularAcceleration);
        }

        /// <summary>
        /// Convert forward speed to wheel angular velocity.
        /// omega = v / r (no-slip condition)
        /// </summary>
        /// <param name="forwardSpeed">Forward speed in m/s</param>
        /// <param name="wheelRadius">Wheel radius in metres</param>
        /// <returns>Angular velocity in rad/s</returns>
        public static float ComputeWheelAngularVelocity(float forwardSpeed, float wheelRadius)
        {
            Debug.Assert(wheelRadius > 0f, "Wheel radius must be positive");

            return forwardSpeed / wheelRadius;
        }

        /// <summary>
        /// Compute torque from steering-induced axis change on a spinning wheel.
        /// When the driver steers mid-air, the spin axis rotates, producing a
        /// real gyroscopic torque: tau = I_wheel * omega_spin * d(axis)/dt.
        /// This is how RC drivers yaw-correct in the air.
        /// </summary>
        /// <param name="currentSpinAxis">Current wheel spin axis in world space (unit vector)</param>
        /// <param name="prevSpinAxis">Previous frame wheel spin axis in world space</param>
        /// <param name="wheelMoI">Wheel moment of inertia (kg*m^2)</param>
        /// <param name="wheelAngularVelocity">Wheel spin rate (rad/s)</param>
        /// <param name="deltaTime">Time step (seconds)</param>
        /// <returns>Steering precession torque in world space (N*m)</returns>
        public static Vector3 ComputeSteeringPrecessionTorque(
            Vector3 currentSpinAxis, Vector3 prevSpinAxis,
            float wheelMoI, float wheelAngularVelocity, float deltaTime)
        {
            if (deltaTime <= 0f)
                return Vector3.zero;

            Vector3 axisRate = (currentSpinAxis - prevSpinAxis) / deltaTime;
            return wheelMoI * wheelAngularVelocity * axisRate;
        }
    }
}
