#if UNITY_EDITOR
using System.Globalization;
using UnityEngine;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Serializes a built buggy prefab + BuggySpec to JSON for the viewer.
    /// Inspects actual components — no hardcoded value duplication.
    /// </summary>
    internal static class BuggySpecSerializer
    {
        internal static string BuildVariantFromPrefab(string key, BuggySpec spec, GameObject root)
        {
            int motorIdx = (int)spec.Motor;
            string[] motorNames = { "21.5T", "17.5T", "13.5T", "9.5T", "5.5T", "1.5T" };
            string motorName = motorIdx < motorNames.Length ? motorNames[motorIdx] : "Custom";

            string[] diffNames = { "Open", "BallDiff", "Spool" };
            string layout = spec.Layout == BuggyDriveLayout.AWD ? "AWD" : "RWD";

            string wheels = BuildWheels(root);
            string meshes = BuildBodyMeshes(root);
            float wheelVisY = FindWheelVisualY(root);
            FindArmDimensions(root, out float armY, out float armTh, out float armDp);

            return $"    \"{key}\": {{\n" +
                $"      \"name\": \"{spec.Name}\",\n" +
                (spec.IsReference ? "      \"reference\": true,\n" : "") +
                $"      \"motor\": \"{motorName}\", \"motorIndex\": {motorIdx},\n" +
                $"      \"layout\": \"{layout}\",\n" +
                $"      \"rearDiff\": \"{diffNames[(int)spec.RearDiff]}\", " + F("\"rearPreload\": {0}", spec.RearPreload) + ",\n" +
                $"      \"frontDiff\": \"{diffNames[(int)spec.FrontDiff]}\", " + F("\"frontPreload\": {0}", spec.FrontPreload) + ",\n" +
                $"      \"centerDiff\": \"{diffNames[(int)spec.CenterDiff]}\", " + F("\"centerPreload\": {0}", spec.CenterPreload) + ",\n" +
                F("      \"centerFrontBias\": {0}", spec.CenterFrontBias) + ",\n" +
                F("      \"chassisWidth\": {0}", spec.ChassisWidth) + ", " + F("\"chassisHeight\": {0}", spec.ChassisHeight) + ",\n" +
                F("      \"chassisLength\": {0}", spec.ChassisLength) + ",\n" +
                F("      \"wheelbaseHalf\": {0}", spec.WheelbaseHalf) + ", " + F("\"trackHalf\": {0}", spec.TrackHalf) + ",\n" +
                F("      \"tireRadius\": {0}", spec.TireRadius) + ",\n" +
                F("      \"frontTireWidth\": {0}", spec.FrontTireWidth) + ", " + F("\"rearTireWidth\": {0}", spec.RearTireWidth) + ",\n" +
                F("      \"hubRadius\": {0}", spec.HubRadius) + ",\n" +
                F("      \"frontHubWidth\": {0}", spec.FrontHubWidth) + ", " + F("\"rearHubWidth\": {0}", spec.RearHubWidth) + ",\n" +
                F("      \"restDistance\": {0}", spec.RestDistance) + ", " + F("\"overExtend\": {0}", spec.OverExtend) + ",\n" +
                F("      \"minSpringLen\": {0}", spec.MinSpringLen) + ", " + F("\"maxSpringForce\": {0}", spec.MaxSpringForce) + ",\n" +
                F("      \"mass\": {0}", spec.Mass) + ",\n" +
                F("      \"comY\": {0}", spec.CenterOfMass.y) + ",\n" +
                F("      \"frontSpringK\": {0}", spec.FrontSpringStrength) + ",\n" +
                F("      \"frontSpringD\": {0}", spec.FrontSpringDamping) + ",\n" +
                F("      \"rearSpringK\": {0}", spec.RearSpringStrength) + ",\n" +
                F("      \"rearSpringD\": {0}", spec.RearSpringDamping) + ",\n" +
                F("      \"gripCoeff\": {0}", spec.GripCoeff) + ",\n" +
                F("      \"steeringMax\": {0}", spec.SteeringMax) + ",\n" +
                F("      \"steeringMaxDeg\": {0}", spec.SteeringMax * Mathf.Rad2Deg) + ",\n" +
                F("      \"gearRatio\": {0}", spec.GearRatio) + ",\n" +
                $"      \"bodyColor\": [{F(spec.BodyColor.r)}, {F(spec.BodyColor.g)}, {F(spec.BodyColor.b)}],\n" +
                wheels + meshes +
                F("      \"armY\": {0}", armY) + ", " + F("\"armThickness\": {0}", armTh) + ", " + F("\"armDepth\": {0}", armDp) + ",\n" +
                F("      \"wheelVisualY\": {0}", wheelVisY) + "\n" +
                "    }";
        }

        static string BuildWheels(GameObject root)
        {
            string result = "      \"wheels\": [\n";
            bool first = true;
            foreach (Transform child in root.transform)
            {
                var rw = child.GetComponent<R8EOX.Vehicle.Internal.RaycastWheel>();
                if (rw == null) continue;
                if (!first) result += ",\n";
                first = false;
                string wName = child.name.Replace("Wheel", "");
                result += $"        {{ \"name\": \"{wName}\", " +
                    $"\"steer\": {(rw.IsSteer ? "true" : "false")}, " +
                    $"\"motor\": {(rw.IsMotor ? "true" : "false")} }}";
            }
            return result + "\n      ],\n";
        }

        static string BuildBodyMeshes(GameObject root)
        {
            string result = "      \"bodyMeshes\": [\n";
            bool first = true;
            foreach (Transform child in root.transform)
            {
                var mr = child.GetComponent<MeshRenderer>();
                if (mr == null) continue;
                if (child.GetComponent<R8EOX.Vehicle.Internal.RaycastWheel>() != null) continue;
                if (child.parent != root.transform) continue;
                if (child.name.Contains("Arm")) continue;

                if (!first) result += ",\n";
                first = false;
                var s = child.localScale; var p = child.localPosition; var r = child.localEulerAngles;

                string matName = mr.sharedMaterial != null ? mr.sharedMaterial.name : "";
                string matType = "darkGrey";
                if (matName.Contains("BodySemi")) matType = "bodySemi";
                else if (matName.Contains("BodyWing")) matType = "bodySolid";
                else if (matName.Contains("Medium")) matType = "medGrey";

                result += $"        {{ \"name\": \"{child.name}\", " +
                    $"\"size\": [{F(s.x)}, {F(s.y)}, {F(s.z)}], " +
                    $"\"pos\": [{F(p.x)}, {F(p.y)}, {F(p.z)}], " +
                    $"\"mat\": \"{matType}\"";
                if (Mathf.Abs(r.x) > 0.1f) result += $", \"rotX\": {F(r.x)}";
                result += " }";
            }
            return result + "\n      ],\n";
        }

        static float FindWheelVisualY(GameObject root)
        {
            foreach (Transform child in root.transform)
            {
                if (child.GetComponent<R8EOX.Vehicle.Internal.RaycastWheel>() == null) continue;
                foreach (Transform wChild in child) return wChild.localPosition.y;
            }
            return -0.05f;
        }

        static void FindArmDimensions(GameObject root, out float armY, out float armTh, out float armDp)
        {
            armY = -0.053f; armTh = 0.005f; armDp = 0.02f;
            foreach (Transform child in root.transform)
            {
                if (!child.name.Contains("Arm")) continue;
                armY = child.localPosition.y; armTh = child.localScale.y; armDp = child.localScale.z;
                return;
            }
        }

        internal static string F(float v) => v.ToString("G6", CultureInfo.InvariantCulture);
        internal static string F(string fmt, float v) =>
            string.Format(CultureInfo.InvariantCulture, fmt, v.ToString("G6", CultureInfo.InvariantCulture));
    }
}
#endif
