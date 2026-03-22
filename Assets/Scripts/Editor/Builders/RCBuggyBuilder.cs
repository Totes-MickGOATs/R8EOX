#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using R8EOX.Vehicle;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Constructs RCBuggy GameObject hierarchies from a BuggySpec.
    /// Supports 2WD (RWD) and 4WD (AWD) variants via menu items.
    /// </summary>
    internal static class RCBuggyBuilder
    {
        // ---- Constants ----

        const int k_DefaultCarLayer = 8;
        const string k_UrpLitShader = "Universal Render Pipeline/Lit";

        // ---- Menu Items ----

        [MenuItem("R8EOX/Build 2WD Buggy")]
        static void Build2WD() => BuildAndSave(BuggySpecCatalog.Get2WD());

        [MenuItem("R8EOX/Build 4WD Buggy")]
        static void Build4WD() => BuildAndSave(BuggySpecCatalog.Get4WD());

        [MenuItem("R8EOX/Build All Buggies")]
        static void BuildAll()
        {
            BuildAndSave(BuggySpecCatalog.Get2WD());
            BuildAndSave(BuggySpecCatalog.Get4WD());
        }

        // ---- Public API ----

        /// <summary>Backward-compatible wrapper — builds the default 2WD buggy.</summary>
        internal static GameObject BuildRCBuggy() => BuildFromSpec(BuggySpecCatalog.Get2WD());

        /// <summary>Builds the full RCBuggy hierarchy from a BuggySpec and returns the root.</summary>
        internal static GameObject BuildFromSpec(BuggySpec spec)
        {
            GameObject root = new GameObject(spec.Name);

            int carLayer = LayerMask.NameToLayer("Vehicle");
            if (carLayer < 0) carLayer = k_DefaultCarLayer;
            root.layer = carLayer;

            AddRigidbody(root, spec);
            root.AddComponent<R8EOX.Input.RCInput>();
            var vehicleManager = root.AddComponent<VehicleManager>();
            AddColliders(root);

            Color bodyTransp = new Color(spec.BodyColor.r, spec.BodyColor.g, spec.BodyColor.b, 0.85f);
            Color bodySolid  = new Color(spec.BodyColor.r, spec.BodyColor.g, spec.BodyColor.b, 1f);

            Material darkGrey = CreateMaterial("DarkGrey",   new Color(0.2f, 0.2f, 0.2f));
            Material medGrey  = CreateMaterial("MediumGrey", new Color(0.5f, 0.5f, 0.5f));
            Material bodySemi = CreateMaterial("BodySemi",   bodyTransp, transparent: true);
            Material bodyWing = CreateMaterial("BodyWing",   bodySolid);
            Material tireMat  = CreateMaterial("BlackTire",  new Color(0.05f, 0.05f, 0.05f));
            Material hubMat   = CreateMaterial("WhiteHub",   new Color(0.9f, 0.9f, 0.9f));

            AddBodyMeshes(root, darkGrey, medGrey, bodySemi, bodyWing);
            AddControlArms(root, darkGrey, spec);

            bool frontIsMotor = spec.Layout == BuggyDriveLayout.AWD;
            float th = spec.TrackHalf, wh = spec.WheelbaseHalf;

            BuildWheel(root, "WheelFL", new Vector3(-th, 0f,  wh),
                isSteer: true,  isMotor: frontIsMotor,
                spec.TireRadius, spec.FrontTireWidth, spec.HubRadius, spec.FrontHubWidth,
                spec, tireMat, hubMat, carLayer);
            BuildWheel(root, "WheelFR", new Vector3( th, 0f,  wh),
                isSteer: true,  isMotor: frontIsMotor,
                spec.TireRadius, spec.FrontTireWidth, spec.HubRadius, spec.FrontHubWidth,
                spec, tireMat, hubMat, carLayer);
            BuildWheel(root, "WheelRL", new Vector3(-th, 0f, -wh),
                isSteer: false, isMotor: true,
                spec.TireRadius, spec.RearTireWidth, spec.HubRadius, spec.RearHubWidth,
                spec, tireMat, hubMat, carLayer);
            BuildWheel(root, "WheelRR", new Vector3( th, 0f, -wh),
                isSteer: false, isMotor: true,
                spec.TireRadius, spec.RearTireWidth, spec.HubRadius, spec.RearHubWidth,
                spec, tireMat, hubMat, carLayer);

            var airGO = new GameObject("AirPhysics"); airGO.transform.SetParent(root.transform, false);
            airGO.AddComponent<R8EOX.Vehicle.Internal.RCAirPhysics>();
            var dtGO = new GameObject("Drivetrain"); dtGO.transform.SetParent(root.transform, false);
            var drivetrain = dtGO.AddComponent<R8EOX.Vehicle.Internal.Drivetrain>();

            ConfigureDrivetrain(drivetrain, spec);
            ConfigureVehicleManager(vehicleManager, spec);
            root.AddComponent<R8EOX.Vehicle.Internal.CollisionTracker>();
            AddAttachmentPoints(root, spec);

            SetLayerRecursive(root, carLayer);
            return root;
        }

        // ---- Save Helper ----
        static void BuildAndSave(BuggySpec spec)
        {
            GameObject go = BuildFromSpec(spec);
            string path = $"Assets/Prefabs/{spec.Name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            Debug.Log($"[RCBuggyBuilder] Saved {path}");
        }

        // ---- Configuration ----
        static void ConfigureDrivetrain(R8EOX.Vehicle.Internal.Drivetrain drivetrain, BuggySpec spec)
        {
            var dtSO = new SerializedObject(drivetrain);
            dtSO.FindProperty("_driveLayout").enumValueIndex    = (int)MapLayout(spec.Layout);
            dtSO.FindProperty("_rearDiffType").enumValueIndex   = (int)MapDiff(spec.RearDiff);
            dtSO.FindProperty("_rearPreload").floatValue        = spec.RearPreload;
            dtSO.FindProperty("_frontDiffType").enumValueIndex  = (int)MapDiff(spec.FrontDiff);
            dtSO.FindProperty("_frontPreload").floatValue       = spec.FrontPreload;
            dtSO.FindProperty("_centerDiffType").enumValueIndex = (int)MapDiff(spec.CenterDiff);
            dtSO.FindProperty("_centerPreload").floatValue      = spec.CenterPreload;
            dtSO.FindProperty("_centerFrontBias").floatValue    = spec.CenterFrontBias;
            dtSO.ApplyModifiedProperties();
        }

        static void ConfigureVehicleManager(VehicleManager vehicleManager, BuggySpec spec)
        {
            var vmSO = new SerializedObject(vehicleManager);
            vmSO.FindProperty("_motorPreset").enumValueIndex        = (int)spec.Motor;
            vmSO.FindProperty("_frontSpringStrength").floatValue    = spec.FrontSpringStrength;
            vmSO.FindProperty("_frontSpringDamping").floatValue     = spec.FrontSpringDamping;
            vmSO.FindProperty("_rearSpringStrength").floatValue     = spec.RearSpringStrength;
            vmSO.FindProperty("_rearSpringDamping").floatValue      = spec.RearSpringDamping;
            vmSO.FindProperty("_gripCoeff").floatValue              = spec.GripCoeff;
            vmSO.FindProperty("_steeringMax").floatValue            = spec.SteeringMax;
            vmSO.FindProperty("_comGround").vector3Value            = spec.CenterOfMass;
            vmSO.ApplyModifiedProperties();
        }

        // ---- Enum Mappers ----
        static R8EOX.Vehicle.Internal.Drivetrain.DriveLayout MapLayout(BuggyDriveLayout layout) =>
            layout == BuggyDriveLayout.AWD
                ? R8EOX.Vehicle.Internal.Drivetrain.DriveLayout.AWD
                : R8EOX.Vehicle.Internal.Drivetrain.DriveLayout.RWD;

        static R8EOX.Vehicle.Internal.Drivetrain.DiffType MapDiff(BuggyDiffType diff) =>
            diff switch {
                BuggyDiffType.BallDiff => R8EOX.Vehicle.Internal.Drivetrain.DiffType.BallDiff,
                BuggyDiffType.Spool    => R8EOX.Vehicle.Internal.Drivetrain.DiffType.Spool,
                _                      => R8EOX.Vehicle.Internal.Drivetrain.DiffType.Open,
            };

        // ---- Private Helpers ----

        static void AddRigidbody(GameObject root, BuggySpec spec)
        {
            var rb = root.AddComponent<Rigidbody>();
            rb.mass = spec.Mass; rb.linearDamping = 0f; rb.angularDamping = 0.05f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        static void AddColliders(GameObject root)
        {
            AddBoxCollider(root, new Vector3(0.56f, 0.12f, 1.56f), new Vector3(0f, -0.17f,  0f));
            AddBoxCollider(root, new Vector3(0.48f, 0.28f, 1.0f),  new Vector3(0f,  0.03f,  0f));
            AddBoxCollider(root, new Vector3(0.44f, 0.14f, 0.16f), new Vector3(0f, -0.10f,  0.86f));
            AddBoxCollider(root, new Vector3(0.36f, 0.18f, 0.18f), new Vector3(0f, -0.08f, -0.86f));
        }

        static void AddBodyMeshes(GameObject root, Material darkGrey, Material medGrey,
                                   Material bodySemi, Material bodyWing)
        {
            AddBoxMesh(root, "ChassisPlate",    new Vector3(0.52f, 0.032f, 1.36f),
                new Vector3(0f, -0.233f,  0f),    darkGrey);
            AddBoxMesh(root, "FrontBumperMesh", new Vector3(0.48f, 0.12f,  0.1f),
                new Vector3(0f, -0.153f,  0.78f), darkGrey);
            AddBoxMesh(root, "RearBumperMesh",  new Vector3(0.4f,  0.16f,  0.16f),
                new Vector3(0f, -0.153f, -0.72f), darkGrey);
            AddBoxMesh(root, "FrontShockTower", new Vector3(0.4f,  0.24f,  0.02f),
                new Vector3(0f, -0.073f,  0.48f), medGrey);
            AddBoxMesh(root, "RearShockTower",  new Vector3(0.32f, 0.24f,  0.02f),
                new Vector3(0f, -0.073f, -0.48f), medGrey);
            AddBoxMesh(root, "BodyShell",       new Vector3(0.48f, 0.16f,  1.12f),
                new Vector3(0f, -0.05f,   0.08f), bodySemi);
            AddBoxMesh(root, "RearWing",
                new Vector3(0.48f, 0.008f, 0.16f), new Vector3(0f, 0.167f, -0.6f), bodyWing)
                .transform.localRotation = Quaternion.Euler(22.5f, 0f, 0f);
        }

        static void AddControlArms(GameObject root, Material darkGrey, BuggySpec spec)
        {
            float armX = spec.TrackHalf * 0.52f; float wh = spec.WheelbaseHalf;
            var armSize = new Vector3(armX, 0.02f, 0.08f);
            AddBoxMesh(root, "FrontArmL", armSize, new Vector3(-armX, -0.213f,  wh), darkGrey);
            AddBoxMesh(root, "FrontArmR", armSize, new Vector3( armX, -0.213f,  wh), darkGrey);
            AddBoxMesh(root, "RearArmL",  armSize, new Vector3(-armX, -0.213f, -wh), darkGrey);
            AddBoxMesh(root, "RearArmR",  armSize, new Vector3( armX, -0.213f, -wh), darkGrey);
        }

        static void BuildWheel(GameObject parent, string name, Vector3 localPos,
            bool isSteer, bool isMotor,
            float tireRadius, float tireHeight, float hubRadius, float hubHeight,
            BuggySpec spec, Material tireMat, Material hubMat, int layer)
        {
            GameObject pivot = new GameObject(name);
            pivot.transform.SetParent(parent.transform, false);
            pivot.transform.localPosition = localPos;
            pivot.layer = layer;

            var wheel = pivot.AddComponent<R8EOX.Vehicle.Internal.RaycastWheel>();
            wheel.IsSteer = isSteer; wheel.IsMotor = isMotor;
            bool front = localPos.z > 0f;
            wheel.SpringStrength = front ? spec.FrontSpringStrength : spec.RearSpringStrength;
            wheel.SpringDamping  = front ? spec.FrontSpringDamping  : spec.RearSpringDamping;
            wheel.RestDistance   = spec.RestDistance;
            var so = new SerializedObject(wheel);
            so.FindProperty("_wheelRadius").floatValue    = tireRadius;
            so.FindProperty("_overExtend").floatValue     = spec.OverExtend;
            so.FindProperty("_minSpringLen").floatValue   = spec.MinSpringLen;
            so.FindProperty("_maxSpringForce").floatValue = spec.MaxSpringForce;
            so.ApplyModifiedProperties();

            BuildWheelVisual(pivot, "WheelVisual", tireRadius, tireHeight, tireMat, layer);
            BuildWheelVisual(pivot, "HubVisual",   hubRadius,  hubHeight,  hubMat,  layer);
        }

        static void BuildWheelVisual(GameObject pivot, string name,
            float radius, float height, Material mat, int layer)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name; go.transform.SetParent(pivot.transform, false);
            go.transform.localPosition = new Vector3(0f, -0.2f, 0f);
            go.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            go.transform.localScale    = new Vector3(radius * 2f, height * 0.5f, radius * 2f);
            go.GetComponent<Renderer>().material = mat;
            Object.DestroyImmediate(go.GetComponent<Collider>()); go.layer = layer;
        }

        static void AddBoxCollider(GameObject parent, Vector3 size, Vector3 center)
        { var col = parent.AddComponent<BoxCollider>(); col.size = size; col.center = center; }

        static GameObject AddBoxMesh(GameObject parent, string name,
            Vector3 size, Vector3 localPos, Material mat)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent.transform, false);
            go.transform.localPosition = localPos;
            go.transform.localScale    = size;
            go.GetComponent<Renderer>().material = mat;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }

        static Material CreateMaterial(string name, Color color, bool transparent = false)
        {
            var mat = new Material(Shader.Find(k_UrpLitShader));
            mat.name = name;
            mat.SetColor("_BaseColor", color);
            if (transparent)
            {
                mat.SetFloat("_Surface", 1f);
                mat.SetFloat("_Blend", 0f);
                mat.SetFloat("_AlphaClip", 0f);
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetInt("_SrcBlend",
                    (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend",
                    (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.renderQueue = 3000;
            }
            return mat;
        }

        static void AddAttachmentPoints(GameObject root, BuggySpec spec)
        {
            AddEmptyChild(root, "CameraTarget",    new Vector3(0f,  0.15f,  0.3f));
            AddEmptyChild(root, "CameraMountPoint",new Vector3(0f,  3f,    -8f));
            AddEmptyChild(root, "ExhaustPoint",    new Vector3(0f, -0.15f, -spec.ChassisLength * 0.5f));
            foreach (Transform child in root.transform)
                if (child.GetComponent<R8EOX.Vehicle.Internal.RaycastWheel>() != null)
                    AddEmptyChild(child.gameObject, "VFXPoint", new Vector3(0f, -spec.RestDistance, 0f));
        }

        static void AddEmptyChild(GameObject parent, string name, Vector3 localPos)
        { var go = new GameObject(name); go.transform.SetParent(parent.transform, false); go.transform.localPosition = localPos; }

        static void SetLayerRecursive(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
                SetLayerRecursive(child.gameObject, layer);
        }
    }
}
#endif
