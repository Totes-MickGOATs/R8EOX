#if UNITY_EDITOR
using NUnit.Framework;
using R8EOX.Vehicle.Internal;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    public class DrivetrainMathTests
    {
        // Realistic values shared across tests
        private const float AxleForce  = 18f;
        private const float Preload    = 5f;
        private const float FrontBias  = 0.35f;
        private const float Tolerance  = 0.001f;

        // diffType int constants: 0=Open, 1=BallDiff, 2=Spool
        private const int Open     = 0;
        private const int BallDiff = 1;
        private const int Spool    = 2;

        // ── ComputeAxleSplit ──────────────────────────────────────────────────

        [Test, Category("invariant")]
        public void ComputeAxleSplit_OpenDiff_FiftyFifty()
        {
            // diffType=0, both grounded, equal RPM → each wheel gets exactly half
            DrivetrainMath.AxleSplit result = DrivetrainMath.ComputeAxleSplit(
                axleForce: AxleForce,
                leftOnGround: true, rightOnGround: true,
                leftRpm: 100f, rightRpm: 100f,
                diffType: Open, couplingPreload: Preload);

            Assert.That(result.LeftShare,  Is.EqualTo(9f).Within(Tolerance));
            Assert.That(result.RightShare, Is.EqualTo(9f).Within(Tolerance));
        }

        [Test, Category("invariant")]
        public void ComputeAxleSplit_LeftAirborne_AllToRight()
        {
            // Left wheel off ground → early-out returns all force to right
            DrivetrainMath.AxleSplit result = DrivetrainMath.ComputeAxleSplit(
                axleForce: AxleForce,
                leftOnGround: false, rightOnGround: true,
                leftRpm: 0f, rightRpm: 100f,
                diffType: BallDiff, couplingPreload: Preload);

            Assert.That(result.LeftShare,  Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.RightShare, Is.EqualTo(AxleForce).Within(Tolerance));
        }

        [Test, Category("invariant")]
        public void ComputeAxleSplit_RightAirborne_AllToLeft()
        {
            // Right wheel off ground → early-out returns all force to left
            DrivetrainMath.AxleSplit result = DrivetrainMath.ComputeAxleSplit(
                axleForce: AxleForce,
                leftOnGround: true, rightOnGround: false,
                leftRpm: 100f, rightRpm: 0f,
                diffType: BallDiff, couplingPreload: Preload);

            Assert.That(result.LeftShare,  Is.EqualTo(AxleForce).Within(Tolerance));
            Assert.That(result.RightShare, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test, Category("invariant")]
        public void ComputeAxleSplit_ForceConservation()
        {
            // LeftShare + RightShare must always equal axleForce.
            // Use BallDiff with large RPM diff so coupling is clamped to preload.
            DrivetrainMath.AxleSplit result = DrivetrainMath.ComputeAxleSplit(
                axleForce: AxleForce,
                leftOnGround: true, rightOnGround: true,
                leftRpm: 200f, rightRpm: 100f,
                diffType: BallDiff, couplingPreload: Preload);

            float total = result.LeftShare + result.RightShare;
            Assert.That(total, Is.EqualTo(AxleForce).Within(Tolerance));
        }

        [Test, Category("value")]
        public void ComputeAxleSplit_BallDiff_CouplingTransfersForce()
        {
            // rpmDiff = 0.05 − 0 = 0.05; raw coupling = 0.05 * 500 = 25
            // clamped to preload (5): left = 9 − 5 = 4, right = 9 + 5 = 14
            DrivetrainMath.AxleSplit result = DrivetrainMath.ComputeAxleSplit(
                axleForce: AxleForce,
                leftOnGround: true, rightOnGround: true,
                leftRpm: 0.05f, rightRpm: 0f,
                diffType: BallDiff, couplingPreload: Preload);

            Assert.That(result.LeftShare,  Is.EqualTo(4f).Within(Tolerance));
            Assert.That(result.RightShare, Is.EqualTo(14f).Within(Tolerance));
        }

        [Test, Category("value")]
        public void ComputeAxleSplit_Spool_MaxCoupling()
        {
            // diffType=2: maxCoupling = Abs(18) * 0.5 = 9
            // rpmDiff = 0.05 − 0 = 0.05; raw coupling = 25, clamped to 9
            // left = 9 − 9 = 0, right = 9 + 9 = 18
            DrivetrainMath.AxleSplit result = DrivetrainMath.ComputeAxleSplit(
                axleForce: AxleForce,
                leftOnGround: true, rightOnGround: true,
                leftRpm: 0.05f, rightRpm: 0f,
                diffType: Spool, couplingPreload: Preload);

            Assert.That(result.LeftShare,  Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.RightShare, Is.EqualTo(18f).Within(Tolerance));
        }

        // ── ComputeCenterDiffSplit ────────────────────────────────────────────

        [Test, Category("invariant")]
        public void ComputeCenterDiffSplit_OpenDiff_BiasOnly()
        {
            // diffType=0 → coupling block skipped; split is purely by frontBias
            // front = 18 * 0.35 = 6.3, rear = 18 * 0.65 = 11.7
            (float frontForce, float rearForce) = DrivetrainMath.ComputeCenterDiffSplit(
                engineForce: AxleForce,
                frontBias: FrontBias,
                frontAvgRpm: 100f, rearAvgRpm: 100f,
                diffType: Open, preload: Preload);

            Assert.That(frontForce, Is.EqualTo(6.3f).Within(Tolerance));
            Assert.That(rearForce,  Is.EqualTo(11.7f).Within(Tolerance));
        }

        [Test, Category("value")]
        public void ComputeCenterDiffSplit_BallDiff_CouplingShiftsForce()
        {
            // rpmDiff = 0.01 − 0 = 0.01; raw coupling = 0.01 * 500 = 5
            // clamped to preload (5): front = 6.3 − 5 = 1.3, rear = 11.7 + 5 = 16.7
            (float frontForce, float rearForce) = DrivetrainMath.ComputeCenterDiffSplit(
                engineForce: AxleForce,
                frontBias: FrontBias,
                frontAvgRpm: 0.01f, rearAvgRpm: 0f,
                diffType: BallDiff, preload: Preload);

            Assert.That(frontForce, Is.EqualTo(1.3f).Within(Tolerance));
            Assert.That(rearForce,  Is.EqualTo(16.7f).Within(Tolerance));
        }
    }
}
#endif
