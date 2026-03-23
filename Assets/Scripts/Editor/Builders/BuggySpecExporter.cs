#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Exports buggy spec data to JSON for the interactive viewer playground.
    /// Run via R8EOX/Export Viewer Data or automatically when building all buggies.
    /// </summary>
    internal static class BuggySpecExporter
    {
        const string k_OutputPath = "buggy-specs.json";

        [MenuItem("R8EOX/Export Viewer Data")]
        internal static void Export()
        {
            var twoWD = BuggySpecCatalog.Get2WD();
            var fourWD = BuggySpecCatalog.Get4WD();

            string json = "{\n" +
                BuildShared() + ",\n" +
                BuildMotorPresets() + ",\n" +
                BuildColliders() + ",\n" +
                "  \"variants\": {\n" +
                BuildVariant("2wd", twoWD) + ",\n" +
                BuildVariant("4wd", fourWD) + "\n" +
                "  }\n" +
                "}";

            string fullPath = Path.Combine(Application.dataPath, "..", k_OutputPath);
            File.WriteAllText(fullPath, json);
            Debug.Log($"[BuggySpecExporter] Wrote {k_OutputPath}");
        }

        static string BuildShared()
        {
            return "  \"shared\": {\n" +
                $"    \"diffStiffness\": 125,\n" +
                $"    \"flipHeight\": 0.35,\n" +
                $"    \"sphereCastRadius\": {R8EOX.Vehicle.Internal.RaycastWheel.SphereCastRadius:F4},\n" +
                $"    \"angularDrag\": 0.05,\n" +
                $"    \"bounciness\": 0.05,\n" +
                $"    \"steeringSpeed\": 7.0,\n" +
                $"    \"steeringSpeedLimit\": 8.0,\n" +
                $"    \"steeringHighSpeedFactor\": 0.4,\n" +
                $"    \"throttleRampDown\": 10.0,\n" +
                $"    \"tumbleEngage\": 50,\n" +
                $"    \"tumbleFull\": 70,\n" +
                $"    \"tumbleBounce\": 0.35,\n" +
                $"    \"tumbleFriction\": 0.3,\n" +
                $"    \"tumbleHysteresis\": 5\n" +
                "  }";
        }

        static string BuildMotorPresets()
        {
            string[] names = { "21.5T", "17.5T", "13.5T", "9.5T", "5.5T", "1.5T" };
            var presets = R8EOX.Vehicle.Internal.MotorPresetRegistry.Presets;
            string result = "  \"motorPresets\": [\n";
            for (int i = 0; i < presets.Length && i < names.Length; i++)
            {
                var p = presets[i];
                result += $"    {{ \"name\": \"{names[i]}\", \"engineForce\": {p.EngineForceMax:F1}, " +
                    $"\"brakeForce\": {p.BrakeForce:F1}, \"reverseForce\": {p.ReverseForce:F1}, " +
                    $"\"coastDrag\": {p.CoastDrag:F1}, \"maxSpeed\": {p.MaxSpeed:F1}, " +
                    $"\"throttleRampUp\": {p.ThrottleRampUp:F1} }}";
                if (i < presets.Length - 1) result += ",";
                result += "\n";
            }
            result += "  ]";
            return result;
        }

        static string BuildColliders()
        {
            // These match the hardcoded values in RCBuggyBuilder.AddColliders
            return "  \"colliders\": [\n" +
                "    { \"name\": \"ChassisSlab\",    \"size\": [0.14, 0.03, 0.39],   \"center\": [0, -0.0425, 0] },\n" +
                "    { \"name\": \"BodyShellCol\",   \"size\": [0.12, 0.07, 0.25],   \"center\": [0,  0.0075, 0] },\n" +
                "    { \"name\": \"FrontBumperCol\", \"size\": [0.11, 0.035, 0.04],  \"center\": [0, -0.025,  0.215] },\n" +
                "    { \"name\": \"RearBumperCol\",  \"size\": [0.09, 0.045, 0.045], \"center\": [0, -0.02,  -0.215] }\n" +
                "  ]";
        }

        static string BuildVariant(string key, BuggySpec spec)
        {
            int motorIdx = (int)spec.Motor;
            string[] motorNames = { "21.5T", "17.5T", "13.5T", "9.5T", "5.5T", "1.5T" };
            string motorName = motorIdx < motorNames.Length ? motorNames[motorIdx] : "Custom";

            string[] diffNames = { "Open", "BallDiff", "Spool" };
            string rearDiff = diffNames[(int)spec.RearDiff];
            string frontDiff = diffNames[(int)spec.FrontDiff];
            string centerDiff = diffNames[(int)spec.CenterDiff];

            string layout = spec.Layout == BuggyDriveLayout.AWD ? "AWD" : "RWD";

            return $"    \"{key}\": {{\n" +
                $"      \"name\": \"{spec.Name}\",\n" +
                $"      \"motor\": \"{motorName}\", \"motorIndex\": {motorIdx},\n" +
                $"      \"layout\": \"{layout}\",\n" +
                $"      \"rearDiff\": \"{rearDiff}\", \"rearPreload\": {spec.RearPreload:F1},\n" +
                $"      \"frontDiff\": \"{frontDiff}\", \"frontPreload\": {spec.FrontPreload:F1},\n" +
                $"      \"centerDiff\": \"{centerDiff}\", \"centerPreload\": {spec.CenterPreload:F1},\n" +
                $"      \"centerFrontBias\": {spec.CenterFrontBias:F2},\n" +
                $"      \"chassisWidth\": {spec.ChassisWidth:F4}, \"chassisHeight\": {spec.ChassisHeight:F4},\n" +
                $"      \"chassisLength\": {spec.ChassisLength:F4},\n" +
                $"      \"wheelbaseHalf\": {spec.WheelbaseHalf:F4}, \"trackHalf\": {spec.TrackHalf:F4},\n" +
                $"      \"tireRadius\": {spec.TireRadius:F4},\n" +
                $"      \"frontTireWidth\": {spec.FrontTireWidth:F4}, \"rearTireWidth\": {spec.RearTireWidth:F4},\n" +
                $"      \"hubRadius\": {spec.HubRadius:F4},\n" +
                $"      \"frontHubWidth\": {spec.FrontHubWidth:F4}, \"rearHubWidth\": {spec.RearHubWidth:F4},\n" +
                $"      \"restDistance\": {spec.RestDistance:F4}, \"overExtend\": {spec.OverExtend:F4},\n" +
                $"      \"minSpringLen\": {spec.MinSpringLen:F4}, \"maxSpringForce\": {spec.MaxSpringForce:F1},\n" +
                $"      \"mass\": {spec.Mass:F1},\n" +
                $"      \"comY\": {spec.CenterOfMass.y:F4},\n" +
                $"      \"frontSpringK\": {spec.FrontSpringStrength:F1},\n" +
                $"      \"frontSpringD\": {spec.FrontSpringDamping:F1},\n" +
                $"      \"rearSpringK\": {spec.RearSpringStrength:F1},\n" +
                $"      \"rearSpringD\": {spec.RearSpringDamping:F1},\n" +
                $"      \"gripCoeff\": {spec.GripCoeff:F1},\n" +
                $"      \"steeringMax\": {spec.SteeringMax:F2},\n" +
                $"      \"steeringMaxDeg\": {spec.SteeringMax * Mathf.Rad2Deg:F1},\n" +
                $"      \"gearRatio\": {spec.GearRatio:F1},\n" +
                $"      \"bodyColor\": [{spec.BodyColor.r:F2}, {spec.BodyColor.g:F2}, {spec.BodyColor.b:F2}]\n" +
                "    }";
        }
    }
}
#endif
