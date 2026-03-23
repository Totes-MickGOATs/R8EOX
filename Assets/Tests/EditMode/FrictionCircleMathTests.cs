#if UNITY_EDITOR
using NUnit.Framework;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    internal sealed class FrictionCircleMathTests
    {
        // ------------------------------------------------------------------ //
        // invariant: physics laws that must always hold
        // ------------------------------------------------------------------ //

        [Test]
        [Category("invariant")]
        public void ComputeCombinedSlipScale_PureLateralWithinCircle_BothScalesOne()
        {
            // latNorm = 0.5, lonNorm = 0 → combinedSq = 0.25 ≤ 1 → no clamping
            var (lat, lon) = R8EOX.Vehicle.Internal.FrictionCircleMath
                .ComputeCombinedSlipScale(latForce: 5f, lonForce: 0f, maxGripForce: 10f);

            Assert.That(lat, Is.EqualTo(1f).Within(0.001f));
            Assert.That(lon, Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeCombinedSlipScale_PureLongitudinalWithinCircle_BothScalesOne()
        {
            // latNorm = 0, lonNorm = 0.5 → combinedSq = 0.25 ≤ 1 → no clamping
            var (lat, lon) = R8EOX.Vehicle.Internal.FrictionCircleMath
                .ComputeCombinedSlipScale(latForce: 0f, lonForce: 5f, maxGripForce: 10f);

            Assert.That(lat, Is.EqualTo(1f).Within(0.001f));
            Assert.That(lon, Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeCombinedSlipScale_CombinedWithinCircle_BothScalesOne()
        {
            // latNorm = lonNorm = 0.5 → combinedSq = 0.5 ≤ 1 → no clamping
            var (lat, lon) = R8EOX.Vehicle.Internal.FrictionCircleMath
                .ComputeCombinedSlipScale(latForce: 5f, lonForce: 5f, maxGripForce: 10f);

            Assert.That(lat, Is.EqualTo(1f).Within(0.001f));
            Assert.That(lon, Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeCombinedSlipScale_CombinedExceedsCircle_ScalesDown()
        {
            // latNorm = lonNorm = 1 → combinedSq = 2 → scale = 1/sqrt(2) ≈ 0.7071
            const float expected = 0.7071f;

            var (lat, lon) = R8EOX.Vehicle.Internal.FrictionCircleMath
                .ComputeCombinedSlipScale(latForce: 10f, lonForce: 10f, maxGripForce: 10f);

            Assert.That(lat, Is.EqualTo(expected).Within(0.001f));
            Assert.That(lon, Is.EqualTo(expected).Within(0.001f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeCombinedSlipScale_ZeroForces_BothScalesOne()
        {
            // combinedSq = 0 ≤ 1 → no clamping → (1, 1)
            var (lat, lon) = R8EOX.Vehicle.Internal.FrictionCircleMath
                .ComputeCombinedSlipScale(latForce: 0f, lonForce: 0f, maxGripForce: 10f);

            Assert.That(lat, Is.EqualTo(1f).Within(0.001f));
            Assert.That(lon, Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeCombinedSlipScale_ZeroMaxGrip_BothScalesZero()
        {
            // Guard branch: maxGripForce <= 0 → early return (0, 0)
            var (lat, lon) = R8EOX.Vehicle.Internal.FrictionCircleMath
                .ComputeCombinedSlipScale(latForce: 5f, lonForce: 5f, maxGripForce: 0f);

            Assert.That(lat, Is.EqualTo(0f).Within(0.001f));
            Assert.That(lon, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeCombinedSlipScale_RatioPreserved()
        {
            // Proportional scaling: lat and lon scales must always be equal
            var (lat, lon) = R8EOX.Vehicle.Internal.FrictionCircleMath
                .ComputeCombinedSlipScale(latForce: 8f, lonForce: 6f, maxGripForce: 10f);

            Assert.That(lat, Is.EqualTo(lon).Within(0.001f));
        }

        // ------------------------------------------------------------------ //
        // value: concrete expected outputs
        // ------------------------------------------------------------------ //

        [Test]
        [Category("value")]
        public void ComputeCombinedSlipScale_PureLateralExceeding_ScalesToOne()
        {
            // latNorm = 1.5, lonNorm = 0 → combinedSq = 2.25 → scale = 1/1.5 ≈ 0.6667
            const float expected = 0.6667f;

            var (lat, lon) = R8EOX.Vehicle.Internal.FrictionCircleMath
                .ComputeCombinedSlipScale(latForce: 15f, lonForce: 0f, maxGripForce: 10f);

            Assert.That(lat, Is.EqualTo(expected).Within(0.001f));
            Assert.That(lon, Is.EqualTo(expected).Within(0.001f));
        }
    }
}
#endif
