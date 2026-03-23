#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using R8EOX.Vehicle.Internal;

namespace R8EOX.Tests.EditMode
{
    /// <summary>
    /// EditMode tests for WheelForceSolver.Solve.
    /// Category "invariant" — physics laws that must always hold.
    /// Category "value"     — expected directional/magnitude outputs.
    /// </summary>
    [TestFixture]
    public class WheelForceSolverTests
    {
        // ------------------------------------------------------------------ //
        //  Helper                                                              //
        // ------------------------------------------------------------------ //

        static WheelForceInput MakeInput(
            float springStrength = 700f, float springDamping = 13f,
            float restDistance = 0.05f, float minSpringLen = 0.008f,
            float maxSpringForce = 50f,
            float wheelRadius = 0.0415f, float gripCoeff = 0.7f,
            float zTraction = 0.10f, float zBrakeTraction = 0.5f,
            AnimationCurve gripCurve = null,
            bool isMotor = true, bool isBraking = false,
            float motorForceShare = 0f,
            float anchorToContact = 0.085f,
            Vector3? contactNormal = null, Vector3? contactPoint = null,
            Vector3? tireVelocity = null, Vector3? wheelForward = null,
            Vector3? wheelRight = null,
            float prevSpringLen = 0.05f, bool wasGroundedLastFrame = true,
            float dt = 0.02f, float currentEngineForce = 0f)
        {
            if (gripCurve == null)
                gripCurve = new AnimationCurve(
                    new Keyframe(0f, 0f), new Keyframe(0.15f, 0.8f),
                    new Keyframe(0.4f, 1.0f), new Keyframe(1.0f, 0.7f));

            return new WheelForceInput(
                springStrength, springDamping, restDistance, minSpringLen, maxSpringForce,
                wheelRadius, gripCoeff, zTraction, zBrakeTraction, gripCurve,
                isMotor, isBraking, motorForceShare,
                anchorToContact,
                contactNormal ?? Vector3.up,
                contactPoint  ?? Vector3.zero,
                tireVelocity  ?? Vector3.zero,
                wheelForward  ?? Vector3.forward,
                wheelRight    ?? Vector3.right,
                prevSpringLen, wasGroundedLastFrame, dt, currentEngineForce);
        }

        // ------------------------------------------------------------------ //
        //  Invariant tests                                                     //
        // ------------------------------------------------------------------ //

        [Test, Category("invariant")]
        public void Solve_TotalForceIsSumOfComponents()
        {
            // Moving wheel so all force channels activate.
            var input = MakeInput(
                tireVelocity: new Vector3(0.5f, 0f, 1.0f),
                motorForceShare: 3f,
                currentEngineForce: 2f);

            WheelForceResult r = WheelForceSolver.Solve(in input);

            Vector3 expected = r.SuspensionForce + r.LateralForce
                             + r.LongitudinalForce + r.MotorForce;

            Assert.That(r.TotalForce.x, Is.EqualTo(expected.x).Within(1e-4f),
                "TotalForce.x must equal component sum");
            Assert.That(r.TotalForce.y, Is.EqualTo(expected.y).Within(1e-4f),
                "TotalForce.y must equal component sum");
            Assert.That(r.TotalForce.z, Is.EqualTo(expected.z).Within(1e-4f),
                "TotalForce.z must equal component sum");
        }

        [Test, Category("invariant")]
        public void Solve_StationaryWheel_HasSuspensionOnly()
        {
            // Zero velocity — grip and motor channels must be inert.
            // motorForceShare=0 so no motor force.
            var input = MakeInput(tireVelocity: Vector3.zero, motorForceShare: 0f);

            WheelForceResult r = WheelForceSolver.Solve(in input);

            Assert.That(r.SuspensionForce.magnitude, Is.GreaterThan(0f),
                "Compressed spring must produce non-zero suspension force");
            Assert.That(r.MotorForce, Is.EqualTo(Vector3.zero),
                "MotorForce must be zero when motorForceShare is 0");
        }

        [Test, Category("invariant")]
        public void Solve_BelowMinSpeed_NoLateralGrip()
        {
            // Speed < 0.1 m/s — lateral grip gate skips the calculation.
            var input = MakeInput(
                tireVelocity: new Vector3(0.05f, 0f, 0f)); // sideways but < 0.1 magnitude

            WheelForceResult r = WheelForceSolver.Solve(in input);

            // LateralForce is only the ramp-slide compensation term (springHoriz),
            // which acts along WheelRight (X-axis). For a flat ContactNormal (Vector3.up)
            // the suspension has no horizontal component, so the result is zero.
            Assert.That(r.LateralForce.x, Is.EqualTo(0f).Within(1e-4f),
                "Below minimum speed, lateral grip must not apply");
            Assert.That(r.LateralForce.z, Is.EqualTo(0f).Within(1e-4f),
                "Below minimum speed, lateral grip must not apply");
        }

        [Test, Category("invariant")]
        public void Solve_MotorWheel_HasMotorForce()
        {
            // isMotor=true with non-zero motorForceShare must produce MotorForce.
            var input = MakeInput(isMotor: true, motorForceShare: 5.0f,
                wheelForward: Vector3.forward);

            WheelForceResult r = WheelForceSolver.Solve(in input);

            Assert.That(r.MotorForce.z, Is.GreaterThan(0f),
                "Motor wheel with positive motorForceShare must produce forward MotorForce");
        }

        [Test, Category("invariant")]
        public void Solve_NonMotorWheel_ZeroMotorForce()
        {
            var input = MakeInput(isMotor: false, motorForceShare: 5.0f);

            WheelForceResult r = WheelForceSolver.Solve(in input);

            Assert.That(r.MotorForce, Is.EqualTo(Vector3.zero),
                "Non-motor wheel must never produce MotorForce regardless of motorForceShare");
        }

        [Test, Category("invariant")]
        public void Solve_ForwardVelocity_LongitudinalForceOpposes()
        {
            // Wheel rolling forward — longitudinal friction must oppose (+Z → force ≤ 0).
            // Use speed well above static-friction threshold so the sign is deterministic.
            var input = MakeInput(
                tireVelocity: new Vector3(0f, 0f, 2.0f),
                wheelForward: Vector3.forward,
                motorForceShare: 0f,
                currentEngineForce: 0f);

            WheelForceResult r = WheelForceSolver.Solve(in input);

            Assert.That(r.LongitudinalForce.z, Is.LessThan(0f),
                "Longitudinal friction must oppose positive forward velocity");
        }

        // ------------------------------------------------------------------ //
        //  Value tests                                                         //
        // ------------------------------------------------------------------ //

        [Test, Category("value")]
        public void Solve_CompressedSpring_SuspensionForceUpward()
        {
            // anchorToContact=0.04 < restDistance(0.05)+wheelRadius(0.0415)=0.0915
            // → spring is compressed → positive suspension force along ContactNormal (up).
            var input = MakeInput(
                anchorToContact: 0.04f,
                restDistance: 0.05f,
                wheelRadius: 0.0415f,
                contactNormal: Vector3.up);

            WheelForceResult r = WheelForceSolver.Solve(in input);

            Assert.That(r.SuspensionForce.y, Is.GreaterThan(0f),
                "Compressed spring must produce upward suspension force");
        }

        [Test, Category("value")]
        public void Solve_SidewaysVelocity_LateralForceOpposes()
        {
            // Wheel sliding right (+X) — lateral grip must push back left (force.x < 0).
            // Speed must exceed k_MinSpeedForGrip (0.1).
            var input = MakeInput(
                tireVelocity: new Vector3(1.5f, 0f, 0f),
                wheelRight: Vector3.right,
                motorForceShare: 0f);

            WheelForceResult r = WheelForceSolver.Solve(in input);

            Assert.That(r.LateralForce.x, Is.LessThan(0f),
                "Lateral grip must oppose sideways (+X) sliding velocity");
        }

        // ------------------------------------------------------------------ //
        //  Friction circle integration tests                                  //
        // ------------------------------------------------------------------ //

        [Test, Category("invariant")]
        public void Solve_ThrottleReducesCornering_LateralForceLower()
        {
            // With motor force (longitudinal demand), lateral grip should be
            // reduced by the friction circle compared to no motor force.
            var noMotor = MakeInput(
                tireVelocity: new Vector3(1.5f, 0f, 2.0f),
                wheelRight: Vector3.right, wheelForward: Vector3.forward,
                motorForceShare: 0f, isMotor: true);
            var withMotor = MakeInput(
                tireVelocity: new Vector3(1.5f, 0f, 2.0f),
                wheelRight: Vector3.right, wheelForward: Vector3.forward,
                motorForceShare: 8.0f, isMotor: true);

            var rNoMotor = WheelForceSolver.Solve(in noMotor);
            var rWithMotor = WheelForceSolver.Solve(in withMotor);

            Assert.That(Mathf.Abs(rWithMotor.LateralForce.x),
                Is.LessThanOrEqualTo(Mathf.Abs(rNoMotor.LateralForce.x)),
                "Motor force should saturate friction circle, reducing lateral grip");
        }

        [Test, Category("invariant")]
        public void Solve_FrictionCircle_TotalForceStillSumsComponents()
        {
            // Even after friction circle scaling, TotalForce must equal the sum.
            var input = MakeInput(
                tireVelocity: new Vector3(2.0f, 0f, 3.0f),
                motorForceShare: 6.0f, isMotor: true,
                currentEngineForce: 5f);

            WheelForceResult r = WheelForceSolver.Solve(in input);

            Vector3 expected = r.SuspensionForce + r.LateralForce
                             + r.LongitudinalForce + r.MotorForce;
            Assert.That(r.TotalForce.x, Is.EqualTo(expected.x).Within(1e-4f));
            Assert.That(r.TotalForce.y, Is.EqualTo(expected.y).Within(1e-4f));
            Assert.That(r.TotalForce.z, Is.EqualTo(expected.z).Within(1e-4f));
        }
    }
}
#endif
