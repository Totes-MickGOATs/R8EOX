#if UNITY_EDITOR
using NUnit.Framework;

// InternalsVisibleTo("R8EOX.Tests.EditMode") is granted in Assets/Scripts/AssemblyInfo.cs.
// SuspensionMath is referenced by fully-qualified name throughout to satisfy the top-down
// linter rule that blocks cross-system `using R8EOX.*.Internal` directives.

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    public class SuspensionMathTests
    {
        // Realistic RC car values from BuggySpec
        private const float RestDistance = 0.05f;
        private const float WheelRadius = 0.0415f;
        private const float MinSpringLen = 0.008f;
        private const float MaxSpringForce = 50f;
        private const float SpringStrength = 700f;
        private const float SpringDamping = 13.0f;
        private const float Dt = 0.02f;
        private const float Tolerance = 0.0001f;

        // --- ComputeSpringLength ---

        [Test, Category("invariant")]
        public void ComputeSpringLength_NormalDistance_SubtractsRadius()
        {
            float result = R8EOX.Vehicle.Internal.SuspensionMath.ComputeSpringLength(
                anchorToContact: 0.09f, wheelRadius: WheelRadius, minSpringLen: MinSpringLen);

            Assert.That(result, Is.EqualTo(0.0485f).Within(Tolerance));
        }

        [Test, Category("invariant")]
        public void ComputeSpringLength_BelowMinimum_ClampsToBumpStop()
        {
            float result = R8EOX.Vehicle.Internal.SuspensionMath.ComputeSpringLength(
                anchorToContact: 0.045f, wheelRadius: WheelRadius, minSpringLen: MinSpringLen);

            Assert.That(result, Is.EqualTo(MinSpringLen).Within(Tolerance));
        }

        // --- ComputeSuspensionForce ---

        [Test, Category("invariant")]
        public void ComputeSuspensionForce_Compressed_ReturnsPositive()
        {
            // compression = 0.05 - 0.03 = 0.02; springForce = 700 * 0.02 = 14.0
            const float compressedLen = 0.03f;
            float result = R8EOX.Vehicle.Internal.SuspensionMath.ComputeSuspensionForce(
                springStrength: SpringStrength, restDistance: RestDistance,
                springLen: compressedLen, prevSpringLen: compressedLen, deltaTime: Dt);

            Assert.That(result, Is.EqualTo(14.0f).Within(Tolerance));
        }

        [Test, Category("invariant")]
        public void ComputeSuspensionForce_Extended_ReturnsZero()
        {
            // springLen > restDistance means tension — suspension cannot pull; clamps to 0
            const float extendedLen = 0.07f;
            float result = R8EOX.Vehicle.Internal.SuspensionMath.ComputeSuspensionForce(
                springStrength: SpringStrength, restDistance: RestDistance,
                springLen: extendedLen, prevSpringLen: extendedLen, deltaTime: Dt);

            Assert.That(result, Is.EqualTo(0f).Within(Tolerance));
        }

        // --- ComputeSuspensionForceWithDamping ---

        [Test, Category("invariant")]
        public void ComputeSuspensionForceWithDamping_CompressionVelocity_AddsDamping()
        {
            // Spring compressing: prevLen > curLen → positive damping → adds to spring force
            const float springLen = 0.03f;
            const float prevSpringLen = 0.04f; // was longer → spring got shorter → compressing

            float springOnlyForce = SpringStrength * (RestDistance - springLen); // 14.0

            float result = R8EOX.Vehicle.Internal.SuspensionMath.ComputeSuspensionForceWithDamping(
                springStrength: SpringStrength, springDamping: SpringDamping,
                restDistance: RestDistance, springLen: springLen,
                prevSpringLen: prevSpringLen, deltaTime: Dt);

            Assert.That(result, Is.GreaterThan(springOnlyForce));
        }

        [Test, Category("invariant")]
        public void ComputeSuspensionForceWithDamping_ExtensionVelocity_ReducesForce()
        {
            // Spring extending: prevLen < curLen → negative damping → subtracts from spring force
            const float springLen = 0.03f;
            const float prevSpringLen = 0.02f; // was shorter → spring got longer → extending

            float springOnlyForce = SpringStrength * (RestDistance - springLen); // 14.0

            float result = R8EOX.Vehicle.Internal.SuspensionMath.ComputeSuspensionForceWithDamping(
                springStrength: SpringStrength, springDamping: SpringDamping,
                restDistance: RestDistance, springLen: springLen,
                prevSpringLen: prevSpringLen, deltaTime: Dt);

            Assert.That(result, Is.LessThan(springOnlyForce));
        }

        // --- SanitizePrevSpringLenForLanding ---

        [Test, Category("invariant")]
        public void SanitizePrevSpringLen_FirstGroundFrame_ReturnsCurrentLen()
        {
            // wasOnGround=false means landing frame — return springLen to zero out damping spike
            const float springLen = 0.04f;
            const float staleAirborneLen = 0.10f;

            float result = R8EOX.Vehicle.Internal.SuspensionMath.SanitizePrevSpringLenForLanding(
                springLen: springLen, prevSpringLen: staleAirborneLen, wasOnGround: false);

            Assert.That(result, Is.EqualTo(springLen).Within(Tolerance));
        }

        [Test, Category("invariant")]
        public void SanitizePrevSpringLen_ContinuousGround_ReturnsPrevLen()
        {
            // wasOnGround=true means continuous contact — return prevSpringLen for normal damping
            const float springLen = 0.04f;
            const float prevSpringLen = 0.035f;

            float result = R8EOX.Vehicle.Internal.SuspensionMath.SanitizePrevSpringLenForLanding(
                springLen: springLen, prevSpringLen: prevSpringLen, wasOnGround: true);

            Assert.That(result, Is.EqualTo(prevSpringLen).Within(Tolerance));
        }

        // --- ComputeGripLoad / ComputeGripLoadFromSuspensionForce ---

        [Test, Category("value")]
        public void ComputeGripLoad_NormalCompression_ClampsToMax()
        {
            // springForce = 2000 * (0.05-0.008) = 84 which exceeds MaxSpringForce(50)
            const float highStrength = 2000f;
            const float bumpStopLen = MinSpringLen; // near bumpstop → exceeds cap

            float result = R8EOX.Vehicle.Internal.SuspensionMath.ComputeGripLoad(
                springStrength: highStrength, restDistance: RestDistance,
                springLen: bumpStopLen, maxSpringForce: MaxSpringForce);

            Assert.That(result, Is.EqualTo(MaxSpringForce).Within(Tolerance));
        }

        [Test, Category("value")]
        public void ComputeGripLoadFromSuspensionForce_NegativeForce_ClampsToZero()
        {
            const float negativeForce = -25f;

            float result = R8EOX.Vehicle.Internal.SuspensionMath.ComputeGripLoadFromSuspensionForce(
                suspensionForce: negativeForce, maxSpringForce: MaxSpringForce);

            Assert.That(result, Is.EqualTo(0f).Within(Tolerance));
        }
    }
}
#endif
