#if UNITY_EDITOR
using UnityEngine;

namespace R8EOX.Editor.Builders
{
    internal readonly struct BuggySpec
    {
        // ---- Identity ----

        public readonly string Name;
        public readonly BuggyMotorKind Motor;


        // ---- Drivetrain ----

        public readonly BuggyDriveLayout Layout;
        public readonly BuggyDiffType RearDiff;
        public readonly float RearPreload;
        public readonly BuggyDiffType FrontDiff;
        public readonly float FrontPreload;
        public readonly BuggyDiffType CenterDiff;
        public readonly float CenterPreload;
        public readonly float CenterFrontBias;


        // ---- Body (chassis plate dimensions used for mesh sizing) ----

        public readonly float ChassisWidth;
        public readonly float ChassisHeight;
        public readonly float ChassisLength;


        // ---- Wheel Positions ----

        /// <summary>Z offset from center to axle.</summary>
        public readonly float WheelbaseHalf;
        /// <summary>X offset from center to wheel.</summary>
        public readonly float TrackHalf;


        // ---- Wheel Sizes ----

        public readonly float TireRadius;
        public readonly float FrontTireWidth;
        public readonly float RearTireWidth;
        public readonly float HubRadius;
        public readonly float FrontHubWidth;
        public readonly float RearHubWidth;


        // ---- Suspension ----

        public readonly float RestDistance;
        public readonly float OverExtend;
        public readonly float MinSpringLen;
        public readonly float MaxSpringForce;


        // ---- Rigidbody ----

        public readonly float Mass;
        public readonly Vector3 CenterOfMass;


        // ---- Spring Tuning ----

        public readonly float FrontSpringStrength;
        public readonly float FrontSpringDamping;
        public readonly float RearSpringStrength;
        public readonly float RearSpringDamping;


        // ---- Traction ----

        public readonly float GripCoeff;


        // ---- Steering ----

        public readonly float SteeringMax;


        // ---- Drivetrain Ratio ----

        public readonly float GearRatio;


        // ---- Appearance ----

        public readonly Color BodyColor;


        // ---- Metadata ----

        public readonly bool IsReference;


        // ---- Constructor ----

        public BuggySpec(
            string name, BuggyMotorKind motor,
            BuggyDriveLayout layout,
            BuggyDiffType rearDiff, float rearPreload,
            BuggyDiffType frontDiff, float frontPreload,
            BuggyDiffType centerDiff, float centerPreload, float centerFrontBias,
            float chassisWidth, float chassisHeight, float chassisLength,
            float wheelbaseHalf, float trackHalf,
            float tireRadius, float frontTireWidth, float rearTireWidth,
            float hubRadius, float frontHubWidth, float rearHubWidth,
            float restDistance, float overExtend, float minSpringLen, float maxSpringForce,
            float mass, Vector3 centerOfMass,
            float frontSpringStrength, float frontSpringDamping,
            float rearSpringStrength, float rearSpringDamping,
            float gripCoeff, float steeringMax,
            float gearRatio,
            Color bodyColor,
            bool isReference = false)
        {
            Name = name; Motor = motor;
            Layout = layout;
            RearDiff = rearDiff; RearPreload = rearPreload;
            FrontDiff = frontDiff; FrontPreload = frontPreload;
            CenterDiff = centerDiff; CenterPreload = centerPreload; CenterFrontBias = centerFrontBias;
            ChassisWidth = chassisWidth; ChassisHeight = chassisHeight; ChassisLength = chassisLength;
            WheelbaseHalf = wheelbaseHalf; TrackHalf = trackHalf;
            TireRadius = tireRadius; FrontTireWidth = frontTireWidth; RearTireWidth = rearTireWidth;
            HubRadius = hubRadius; FrontHubWidth = frontHubWidth; RearHubWidth = rearHubWidth;
            RestDistance = restDistance; OverExtend = overExtend;
            MinSpringLen = minSpringLen; MaxSpringForce = maxSpringForce;
            Mass = mass; CenterOfMass = centerOfMass;
            FrontSpringStrength = frontSpringStrength; FrontSpringDamping = frontSpringDamping;
            RearSpringStrength = rearSpringStrength; RearSpringDamping = rearSpringDamping;
            GripCoeff = gripCoeff; SteeringMax = steeringMax;
            GearRatio = gearRatio;
            BodyColor = bodyColor;
            IsReference = isReference;
        }
    }
}
#endif
