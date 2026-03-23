#if UNITY_EDITOR
using NUnit.Framework;
using R8EOX.Vehicle.Internal;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    internal sealed class GripMathTests
    {
        // RC-realistic constants
        private const float GripCoeff = 0.7f;
        private const float GripLoad = 30f;
        private const float GripFactor = 0.8f;
        private const float ZTraction = 0.10f;
        private const float ZBrakeTraction = 0.5f;
        private const float StaticFrictionSpeed = 0.5f;
        private const float StaticFrictionTraction = 5.0f;
        private const float WheelRadius = 0.0415f;
        private const float Tolerance = 0.001f;
        private const float ExpectedRpm = 1150.518f;
        private const float RpmTolerance = 0.1f;

        // ── SlipRatio ─────────────────────────────────────────────────────────

        [Test]
        [Category("invariant")]
        public void ComputeSlipRatio_ZeroSpeed_ReturnsZero()
        {
            float result = GripMath.ComputeSlipRatio(lateralVelocity: 3f, speed: 0f);

            Assert.That(result, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test]
        [Category("invariant")]
        public void ComputeSlipRatio_FullSidewaysSlide_ReturnsOne()
        {
            float result = GripMath.ComputeSlipRatio(lateralVelocity: 5f, speed: 5f);

            Assert.That(result, Is.EqualTo(1f).Within(Tolerance));
        }

        [Test]
        [Category("value")]
        public void ComputeSlipRatio_PartialSlide_ReturnsRatio()
        {
            float result = GripMath.ComputeSlipRatio(lateralVelocity: 2f, speed: 5f);

            Assert.That(result, Is.EqualTo(0.4f).Within(Tolerance));
        }

        // ── LateralForce ──────────────────────────────────────────────────────

        [Test]
        [Category("invariant")]
        public void ComputeLateralForceMagnitude_PositiveLateral_ReturnsNegative()
        {
            float result = GripMath.ComputeLateralForceMagnitude(
                lateralVelocity: 1f,
                gripFactor: GripFactor,
                gripCoeff: GripCoeff,
                gripLoad: GripLoad);

            Assert.That(result, Is.LessThan(0f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeLateralForceMagnitude_NegativeLateral_ReturnsPositive()
        {
            float result = GripMath.ComputeLateralForceMagnitude(
                lateralVelocity: -1f,
                gripFactor: GripFactor,
                gripCoeff: GripCoeff,
                gripLoad: GripLoad);

            Assert.That(result, Is.GreaterThan(0f));
        }

        // ── EffectiveTraction ─────────────────────────────────────────────────

        [Test]
        [Category("invariant")]
        public void ComputeEffectiveTraction_StoppedNoEngine_ReturnsStaticFriction()
        {
            float result = GripMath.ComputeEffectiveTraction(
                isBraking: false,
                forwardSpeed: 0.1f,
                engineForce: 0f,
                zTraction: ZTraction,
                zBrakeTraction: ZBrakeTraction,
                staticFrictionSpeed: StaticFrictionSpeed,
                staticFrictionTraction: StaticFrictionTraction);

            Assert.That(result, Is.EqualTo(StaticFrictionTraction).Within(Tolerance));
        }

        [Test]
        [Category("invariant")]
        public void ComputeEffectiveTraction_Braking_ReturnsBrakeTraction()
        {
            float result = GripMath.ComputeEffectiveTraction(
                isBraking: true,
                forwardSpeed: 3f,
                engineForce: 0f,
                zTraction: ZTraction,
                zBrakeTraction: ZBrakeTraction,
                staticFrictionSpeed: StaticFrictionSpeed,
                staticFrictionTraction: StaticFrictionTraction);

            Assert.That(result, Is.EqualTo(ZBrakeTraction).Within(Tolerance));
        }

        // ── LongitudinalForce ─────────────────────────────────────────────────

        [Test]
        [Category("invariant")]
        public void ComputeLongitudinalForceMagnitude_OpposesForwardMotion()
        {
            float result = GripMath.ComputeLongitudinalForceMagnitude(
                forwardSpeed: 1f,
                effectiveTraction: ZTraction,
                gripCoeff: GripCoeff,
                gripLoad: GripLoad);

            Assert.That(result, Is.LessThan(0f));
        }

        // ── WheelRpm ──────────────────────────────────────────────────────────

        [Test]
        [Category("value")]
        public void ComputeWheelRpm_AtSpeed_ReturnsExpectedRpm()
        {
            // 5 m/s at r=0.0415 m → (5/0.0415)*60/(2π) ≈ 1150.5 RPM
            float result = GripMath.ComputeWheelRpm(forwardSpeed: 5f, wheelRadius: WheelRadius);

            Assert.That(result, Is.EqualTo(ExpectedRpm).Within(RpmTolerance));
        }
    }
}
#endif
