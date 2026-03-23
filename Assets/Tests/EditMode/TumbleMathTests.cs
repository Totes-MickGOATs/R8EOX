#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using R8EOX.Vehicle.Internal;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    internal sealed class TumbleMathTests
    {
        private const float EngageDeg = 50f;
        private const float FullDeg = 70f;
        private const float HysteresisDeg = 5f;

        // -------------------------------------------------------------------------
        // ComputeTumbleFactor
        // -------------------------------------------------------------------------

        [Test]
        [Category("invariant")]
        public void ComputeTumbleFactor_Airborne_ReturnsZero()
        {
            float result = TumbleMath.ComputeTumbleFactor(
                tiltAngle: 80f,
                isAirborne: true,
                wasTumbling: false,
                engageDeg: EngageDeg,
                fullDeg: FullDeg,
                hysteresisDeg: HysteresisDeg);

            Assert.That(result, Is.EqualTo(0f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeTumbleFactor_BelowEngage_ReturnsZero()
        {
            float result = TumbleMath.ComputeTumbleFactor(
                tiltAngle: 30f,
                isAirborne: false,
                wasTumbling: false,
                engageDeg: EngageDeg,
                fullDeg: FullDeg,
                hysteresisDeg: HysteresisDeg);

            Assert.That(result, Is.EqualTo(0f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeTumbleFactor_AboveFull_ReturnsOne()
        {
            float result = TumbleMath.ComputeTumbleFactor(
                tiltAngle: 80f,
                isAirborne: false,
                wasTumbling: false,
                engageDeg: EngageDeg,
                fullDeg: FullDeg,
                hysteresisDeg: HysteresisDeg);

            Assert.That(result, Is.EqualTo(1f));
        }

        [Test]
        [Category("value")]
        public void ComputeTumbleFactor_MidRange_ReturnsSmoothstep()
        {
            // tilt=60, engage=50, full=70 → t=0.5, smoothstep(0.5)=0.5
            const float t = 0.5f;
            const float expected = t * t * (3f - 2f * t);

            float result = TumbleMath.ComputeTumbleFactor(
                tiltAngle: 60f,
                isAirborne: false,
                wasTumbling: false,
                engageDeg: EngageDeg,
                fullDeg: FullDeg,
                hysteresisDeg: HysteresisDeg);

            Assert.That(result, Is.EqualTo(expected).Within(0.0001f));
        }

        [Test]
        [Category("invariant")]
        public void ComputeTumbleFactor_Hysteresis_LowersEngageThreshold()
        {
            // wasTumbling=true → effectiveEngage = 50 - 5 = 45
            // tiltAngle=47 is above 45 so factor > 0, but would be 0 without hysteresis
            float resultWithHysteresis = TumbleMath.ComputeTumbleFactor(
                tiltAngle: 47f,
                isAirborne: false,
                wasTumbling: true,
                engageDeg: EngageDeg,
                fullDeg: FullDeg,
                hysteresisDeg: HysteresisDeg);

            float resultWithoutHysteresis = TumbleMath.ComputeTumbleFactor(
                tiltAngle: 47f,
                isAirborne: false,
                wasTumbling: false,
                engageDeg: EngageDeg,
                fullDeg: FullDeg,
                hysteresisDeg: HysteresisDeg);

            Assert.That(resultWithHysteresis, Is.GreaterThan(0f),
                "Should engage at 45 (50-5) when wasTumbling=true");
            Assert.That(resultWithoutHysteresis, Is.EqualTo(0f),
                "Should not engage at 47 when wasTumbling=false (engage threshold is 50)");
        }

        // -------------------------------------------------------------------------
        // ComputeTiltAngle
        // -------------------------------------------------------------------------

        [Test]
        [Category("invariant")]
        public void ComputeTiltAngle_Upright_ReturnsZero()
        {
            float result = TumbleMath.ComputeTiltAngle(Vector3.up);

            Assert.That(result, Is.EqualTo(0f).Within(0.01f));
        }

        [Test]
        [Category("value")]
        public void ComputeTiltAngle_FortyFiveDegrees_ReturnsFortyFive()
        {
            Vector3 carUp = new Vector3(0f, 1f, 1f).normalized;

            float result = TumbleMath.ComputeTiltAngle(carUp);

            Assert.That(result, Is.EqualTo(45f).Within(0.01f));
        }
    }
}
#endif
