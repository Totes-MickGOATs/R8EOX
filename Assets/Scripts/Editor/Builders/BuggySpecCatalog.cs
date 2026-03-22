#if UNITY_EDITOR
using UnityEngine;

namespace R8EOX.Editor.Builders
{
    internal static class BuggySpecCatalog
    {
        internal static BuggySpec Get2WD() => new BuggySpec(
            name: "2WDBuggy",
            motor: BuggyMotorKind.Motor13_5T,
            layout: BuggyDriveLayout.RWD,
            rearDiff: BuggyDiffType.BallDiff, rearPreload: 50f,
            frontDiff: BuggyDiffType.Open, frontPreload: 0f,
            centerDiff: BuggyDiffType.Open, centerPreload: 0f, centerFrontBias: 0f,
            chassisWidth: 0.52f, chassisHeight: 0.032f, chassisLength: 1.36f,
            wheelbaseHalf: 0.68f, trackHalf: 0.50f,
            tireRadius: 0.166f, frontTireWidth: 0.112f, rearTireWidth: 0.168f,
            hubRadius: 0.11f, frontHubWidth: 0.128f, rearHubWidth: 0.184f,
            restDistance: 0.20f, overExtend: 0.08f, minSpringLen: 0.032f, maxSpringForce: 50f,
            mass: 15f, centerOfMass: new Vector3(0f, -0.12f, 0f),
            frontSpringStrength: 700f, frontSpringDamping: 41f,
            rearSpringStrength: 350f, rearSpringDamping: 29f,
            gripCoeff: 0.7f, steeringMax: 0.50f,
            bodyColor: new Color(0.18f, 0.45f, 0.9f, 0.85f)
        );

        internal static BuggySpec Get4WD() => new BuggySpec(
            name: "4WDBuggy",
            motor: BuggyMotorKind.Motor13_5T,
            layout: BuggyDriveLayout.AWD,
            rearDiff: BuggyDiffType.BallDiff, rearPreload: 50f,
            frontDiff: BuggyDiffType.Open, frontPreload: 0f,
            centerDiff: BuggyDiffType.Open, centerPreload: 20f, centerFrontBias: 0.35f,
            chassisWidth: 0.52f, chassisHeight: 0.032f, chassisLength: 1.36f,
            wheelbaseHalf: 0.68f, trackHalf: 0.52f,
            tireRadius: 0.166f, frontTireWidth: 0.140f, rearTireWidth: 0.168f,
            hubRadius: 0.11f, frontHubWidth: 0.156f, rearHubWidth: 0.184f,
            restDistance: 0.20f, overExtend: 0.08f, minSpringLen: 0.032f, maxSpringForce: 50f,
            mass: 17f, centerOfMass: new Vector3(0f, -0.10f, 0f),
            frontSpringStrength: 650f, frontSpringDamping: 38f,
            rearSpringStrength: 400f, rearSpringDamping: 32f,
            gripCoeff: 0.7f, steeringMax: 0.45f,
            bodyColor: new Color(0.18f, 0.75f, 0.35f, 0.85f)
        );
    }
}
#endif
