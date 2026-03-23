#if UNITY_EDITOR
using NUnit.Framework;
using R8EOX.Vehicle.Internal;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    internal sealed class ESCMathTests
    {
        // ── Helper ──────────────────────────────────────────────────────────

        private static ESCMath.DriveResult Drive(
            float throttle = 0f, float brake = 0f, float fwdSpeed = 0f,
            bool reverseEngaged = false)
        {
            return ESCMath.ComputeGroundDrive(
                throttle, brake, fwdSpeed, reverseEngaged,
                engineForceMax: 18f, brakeForce: 15.3f, reverseForce: 9.9f,
                coastDrag: 2.5f, maxSpeed: 20f, velocityMagnitude: 0f,
                reverseSpeedThreshold: 0.25f, forwardSpeedClearThreshold: 0.50f,
                reverseBrakeMinThreshold: 0.1f);
        }

        // ── Invariant tests ─────────────────────────────────────────────────

        [Test]
        [Category("invariant")]
        public void ComputeGroundDrive_FullThrottle_EngineForcePositive()
        {
            var result = Drive(throttle: 1.0f);
            Assert.That(result.EngineForce, Is.EqualTo(18f).Within(0.001f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeGroundDrive_FullThrottle_NoBrakeOrCoast()
        {
            var result = Drive(throttle: 1.0f);
            Assert.That(result.BrakeForce, Is.EqualTo(0f).Within(0.001f));
            Assert.That(result.CoastDragForce, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeGroundDrive_BrakeOnly_BrakeForcePositive()
        {
            var result = Drive(brake: 1.0f);
            Assert.That(result.BrakeForce, Is.EqualTo(15.3f).Within(0.001f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeGroundDrive_NoInput_CoastDrag()
        {
            var result = Drive(throttle: 0f, brake: 0f);
            Assert.That(result.CoastDragForce, Is.EqualTo(2.5f).Within(0.001f));
            Assert.That(result.EngineForce, Is.EqualTo(0f).Within(0.001f));
            Assert.That(result.BrakeForce, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeGroundDrive_Coast_ClearsReverse()
        {
            var result = Drive(throttle: 0f, brake: 0f, reverseEngaged: true);
            Assert.That(result.ReverseEngaged, Is.False);
        }

        // ── Value tests ─────────────────────────────────────────────────────

        [Test]
        [Category("value")]
        public void ComputeGroundDrive_ReverseEngaged_NegativeEngineForce()
        {
            var result = Drive(brake: 1.0f, reverseEngaged: true, fwdSpeed: 0f);
            Assert.That(result.EngineForce, Is.EqualTo(-9.9f).Within(0.001f));
        }

        [Test]
        [Category("value")]
        public void ComputeGroundDrive_ReverseStateMachine_EngagesOnBrake()
        {
            // brake=0.5 > threshold 0.1, fwdSpeed=0 < threshold 0.25
            var result = Drive(brake: 0.5f, fwdSpeed: 0f);
            Assert.That(result.ReverseEngaged, Is.True);
        }

        [Test]
        [Category("value")]
        public void ComputeGroundDrive_ReverseStateMachine_ThrottleClearsReverse()
        {
            var result = Drive(throttle: 1.0f, reverseEngaged: true);
            Assert.That(result.ReverseEngaged, Is.False);
        }

        [Test]
        [Category("value")]
        public void ComputeGroundDrive_AtMaxSpeed_ZeroEngineForce()
        {
            // fwdSpeed == maxSpeed (20) → engine force must be 0
            var result = Drive(throttle: 1.0f, fwdSpeed: 20f);
            Assert.That(result.EngineForce, Is.EqualTo(0f).Within(0.001f));
        }
    }
}
#endif
