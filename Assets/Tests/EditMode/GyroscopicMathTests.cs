#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using R8EOX.Vehicle.Internal;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    public class GyroscopicMathTests
    {
        // Realistic RC car values from BuggySpec
        private const float WheelMoI = 0.12f;
        private const float WheelRadius = 0.0415f;
        private const float Dt = 0.02f;
        private const float Tolerance = 0.001f;

        // --- ComputeGyroscopicTorque ---

        [Test, Category("invariant")]
        public void ComputeGyroscopicTorque_ZeroBodyOmega_ZeroTorque()
        {
            // No body rotation → angular momentum exists but cross product with zero is zero
            Vector3 result = GyroscopicMath.ComputeGyroscopicTorque(
                bodyAngularVelocity: Vector3.zero,
                wheelSpinAxis: Vector3.right,
                wheelMoI: WheelMoI,
                wheelAngularVelocity: 100f);

            Assert.That(result.x, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.y, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.z, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test, Category("invariant")]
        public void ComputeGyroscopicTorque_ZeroWheelSpin_ZeroTorque()
        {
            // Wheel not spinning → angular momentum is zero → no precession torque
            Vector3 result = GyroscopicMath.ComputeGyroscopicTorque(
                bodyAngularVelocity: new Vector3(0f, 1f, 0f),
                wheelSpinAxis: Vector3.right,
                wheelMoI: WheelMoI,
                wheelAngularVelocity: 0f);

            Assert.That(result.x, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.y, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.z, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test, Category("invariant")]
        public void ComputeGyroscopicTorque_YawCouplesPitch()
        {
            // bodyOmega=(0,1,0), spinAxis=(1,0,0), MoI=0.12, angVel=100
            // angularMomentum = (1,0,0) * (0.12 * 100) = (12,0,0)
            // cross((0,1,0), (12,0,0)) = (0*0 - 0*0, 0*12 - 0*0, 0*0 - 1*12) = (0, 0, -12)
            Vector3 result = GyroscopicMath.ComputeGyroscopicTorque(
                bodyAngularVelocity: new Vector3(0f, 1f, 0f),
                wheelSpinAxis: Vector3.right,
                wheelMoI: WheelMoI,
                wheelAngularVelocity: 100f);

            Assert.That(result.x, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.y, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.z, Is.EqualTo(-12f).Within(Tolerance));
        }

        // --- ComputeReactionTorque ---

        [Test, Category("invariant")]
        public void ComputeReactionTorque_ZeroAcceleration_ZeroTorque()
        {
            // currentSpinRate == previousSpinRate → deltaOmega = 0 → no reaction torque
            Vector3 result = GyroscopicMath.ComputeReactionTorque(
                wheelSpinAxis: Vector3.right,
                wheelMoI: WheelMoI,
                currentSpinRate: 100f,
                previousSpinRate: 100f,
                deltaTime: Dt);

            Assert.That(result.x, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.y, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.z, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test, Category("invariant")]
        public void ComputeReactionTorque_Acceleration_OpposesAxis()
        {
            // spinAxis=(1,0,0), current=110, prev=100, dt=0.02
            // deltaOmega = 10, angAccel = 10 / 0.02 = 500
            // torque = -(1,0,0) * 0.12 * 500 = (-60, 0, 0)
            Vector3 result = GyroscopicMath.ComputeReactionTorque(
                wheelSpinAxis: Vector3.right,
                wheelMoI: WheelMoI,
                currentSpinRate: 110f,
                previousSpinRate: 100f,
                deltaTime: Dt);

            Assert.That(result.x, Is.EqualTo(-60f).Within(Tolerance));
            Assert.That(result.y, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.z, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test, Category("invariant")]
        public void ComputeReactionTorque_BrakeDeceleration_TorqueAlongAxis()
        {
            // Deceleration: current < prev → deltaOmega negative → angAccel negative
            // torque = -spinAxis * MoI * negativeAccel → positive X component (along +axis)
            // spinAxis=(1,0,0), current=90, prev=100, dt=0.02
            // deltaOmega = -10, angAccel = -500, torque = -(1,0,0)*0.12*(-500) = (60, 0, 0)
            Vector3 result = GyroscopicMath.ComputeReactionTorque(
                wheelSpinAxis: Vector3.right,
                wheelMoI: WheelMoI,
                currentSpinRate: 90f,
                previousSpinRate: 100f,
                deltaTime: Dt);

            Assert.That(result.x, Is.GreaterThan(0f));
            Assert.That(result.x, Is.EqualTo(60f).Within(Tolerance));
            Assert.That(result.y, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.z, Is.EqualTo(0f).Within(Tolerance));
        }

        // --- ComputeWheelAngularVelocity ---

        [Test, Category("value")]
        public void ComputeWheelAngularVelocity_AtSpeed_ReturnsCorrectOmega()
        {
            // 10 m/s at wheelRadius=0.0415m → omega = 10 / 0.0415 ≈ 240.964 rad/s
            float result = GyroscopicMath.ComputeWheelAngularVelocity(
                forwardSpeed: 10f,
                wheelRadius: WheelRadius);

            Assert.That(result, Is.EqualTo(240.964f).Within(0.01f));
        }
    }
}
#endif
