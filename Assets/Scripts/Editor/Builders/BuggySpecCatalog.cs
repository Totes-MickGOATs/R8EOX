#if UNITY_EDITOR
using UnityEngine;

namespace R8EOX.Editor.Builders
{
    internal static class BuggySpecCatalog
    {
        /// <summary>All registered specs. Builder and exporter discover from here.</summary>
        internal static BuggySpec[] GetAll() => new[] { GetRCBuggy(), Get2WD(), Get4WD() };

        internal static BuggySpec GetRCBuggy() => new BuggySpec(
            name: "RCBuggy",
            motor: BuggyMotorKind.Motor21_5T,
            layout: BuggyDriveLayout.RWD,
            rearDiff: BuggyDiffType.Open, rearPreload: 0f,
            frontDiff: BuggyDiffType.Open, frontPreload: 0f,
            centerDiff: BuggyDiffType.Open, centerPreload: 0f, centerFrontBias: 0f,
            chassisWidth: 0.13f, chassisHeight: 0.008f, chassisLength: 0.34f,
            wheelbaseHalf: 0.17f, trackHalf: 0.125f,
            tireRadius: 0.0415f, frontTireWidth: 0.028f, rearTireWidth: 0.042f,
            hubRadius: 0.0275f, frontHubWidth: 0.032f, rearHubWidth: 0.046f,
            restDistance: 0.05f, overExtend: 0.02f, minSpringLen: 0.008f, maxSpringForce: 50f,
            mass: 1.3f, centerOfMass: new Vector3(0f, -0.05f, 0f),
            frontSpringStrength: 600f, frontSpringDamping: 11.0f,
            rearSpringStrength: 300f, rearSpringDamping: 8.0f,
            gripCoeff: 0.75f, steeringMax: 0.55f,
            gearRatio: 7.5f,
            bodyColor: new Color(0.85f, 0.2f, 0.15f, 0.85f),
            isReference: true
        );

        internal static BuggySpec Get2WD() => new BuggySpec(
            name: "2WDBuggy",
            motor: BuggyMotorKind.Motor17_5T,
            layout: BuggyDriveLayout.RWD,
            rearDiff: BuggyDiffType.Open, rearPreload: 0f,
            frontDiff: BuggyDiffType.Open, frontPreload: 0f,
            centerDiff: BuggyDiffType.Open, centerPreload: 0f, centerFrontBias: 0f,
            chassisWidth: 0.13f, chassisHeight: 0.008f, chassisLength: 0.34f,
            wheelbaseHalf: 0.17f, trackHalf: 0.125f,
            tireRadius: 0.0415f, frontTireWidth: 0.028f, rearTireWidth: 0.042f,
            hubRadius: 0.0275f, frontHubWidth: 0.032f, rearHubWidth: 0.046f,
            restDistance: 0.05f, overExtend: 0.02f, minSpringLen: 0.008f, maxSpringForce: 50f,
            mass: 1.5f, centerOfMass: new Vector3(0f, -0.05f, 0f),
            frontSpringStrength: 700f, frontSpringDamping: 13.0f,
            rearSpringStrength: 350f, rearSpringDamping: 9.2f,
            gripCoeff: 0.7f, steeringMax: 0.50f,
            gearRatio: 7.5f,
            bodyColor: new Color(0.18f, 0.45f, 0.9f, 0.85f)
        );

        internal static BuggySpec Get4WD() => new BuggySpec(
            name: "4WDBuggy",
            motor: BuggyMotorKind.Motor13_5T,
            layout: BuggyDriveLayout.AWD,
            rearDiff: BuggyDiffType.BallDiff, rearPreload: 5f,
            frontDiff: BuggyDiffType.Open, frontPreload: 0f,
            centerDiff: BuggyDiffType.Open, centerPreload: 2f, centerFrontBias: 0.35f,
            chassisWidth: 0.13f, chassisHeight: 0.008f, chassisLength: 0.34f,
            wheelbaseHalf: 0.17f, trackHalf: 0.13f,
            tireRadius: 0.0415f, frontTireWidth: 0.035f, rearTireWidth: 0.042f,
            hubRadius: 0.0275f, frontHubWidth: 0.039f, rearHubWidth: 0.046f,
            restDistance: 0.05f, overExtend: 0.02f, minSpringLen: 0.008f, maxSpringForce: 50f,
            mass: 1.7f, centerOfMass: new Vector3(0f, -0.025f, 0f),
            frontSpringStrength: 650f, frontSpringDamping: 12.0f,
            rearSpringStrength: 400f, rearSpringDamping: 8.4f,
            gripCoeff: 0.7f, steeringMax: 0.45f,
            gearRatio: 7.5f,
            bodyColor: new Color(0.18f, 0.75f, 0.35f, 0.85f)
        );
    }
}
#endif
