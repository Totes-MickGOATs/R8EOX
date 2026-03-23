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

        [MenuItem("R8EOX/Build All Buggies")]
        static void BuildAll()
        {
            foreach (var spec in BuggySpecCatalog.GetAll())
                BuildAndSave(spec);
            BuggySpecExporter.Export();
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
            AddSkidPlates(root, carLayer);

            var bc = spec.BodyColor;
            var dg  = GetOrCreateMaterial("DarkGrey",              new Color(0.2f, 0.2f, 0.2f));
            var mg  = GetOrCreateMaterial("MediumGrey",            new Color(0.5f, 0.5f, 0.5f));
            var bs  = GetOrCreateMaterial($"{spec.Name}_BodySemi", new Color(bc.r, bc.g, bc.b, 0.85f), transparent: true);
            var bw  = GetOrCreateMaterial($"{spec.Name}_BodyWing", new Color(bc.r, bc.g, bc.b, 1f));
            var tireMat = GetOrCreateMaterial("BlackTire",         new Color(0.05f, 0.05f, 0.05f));
            var hubMat  = GetOrCreateMaterial("WhiteHub",          new Color(0.9f, 0.9f, 0.9f));

            AddBodyMeshes(root, dg, mg, bs, bw);
            AddControlArms(root, dg, spec);

            bool frontIsMotor = spec.Layout == BuggyDriveLayout.AWD;
            float th = spec.TrackHalf, wh = spec.WheelbaseHalf;
            float tr = spec.TireRadius, ftw = spec.FrontTireWidth, rtw = spec.RearTireWidth;
            float hr = spec.HubRadius,  fhw = spec.FrontHubWidth,  rhw = spec.RearHubWidth;

            BuildWheel(root, "WheelFL", new Vector3(-th, 0f,  wh), true,  frontIsMotor, tr, ftw, hr, fhw, spec, tireMat, hubMat, carLayer);
            BuildWheel(root, "WheelFR", new Vector3( th, 0f,  wh), true,  frontIsMotor, tr, ftw, hr, fhw, spec, tireMat, hubMat, carLayer);
            BuildWheel(root, "WheelRL", new Vector3(-th, 0f, -wh), false, true,         tr, rtw, hr, rhw, spec, tireMat, hubMat, carLayer);
            BuildWheel(root, "WheelRR", new Vector3( th, 0f, -wh), false, true,         tr, rtw, hr, rhw, spec, tireMat, hubMat, carLayer);

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
            var go = BuildFromSpec(spec);
            string path = $"Assets/Prefabs/{spec.Name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, path); Object.DestroyImmediate(go);
            AssetDatabase.SaveAssets(); Debug.Log($"[RCBuggyBuilder] Saved {path}");
        }

        // ---- Configuration ----
        static void ConfigureDrivetrain(R8EOX.Vehicle.Internal.Drivetrain dt, BuggySpec s)
        {
            var so = new SerializedObject(dt);
            so.FindProperty("_driveLayout").enumValueIndex    = (int)MapLayout(s.Layout);
            so.FindProperty("_rearDiffType").enumValueIndex   = (int)MapDiff(s.RearDiff);
            so.FindProperty("_rearPreload").floatValue        = s.RearPreload;
            so.FindProperty("_frontDiffType").enumValueIndex  = (int)MapDiff(s.FrontDiff);
            so.FindProperty("_frontPreload").floatValue       = s.FrontPreload;
            so.FindProperty("_centerDiffType").enumValueIndex = (int)MapDiff(s.CenterDiff);
            so.FindProperty("_centerPreload").floatValue      = s.CenterPreload;
            so.FindProperty("_centerFrontBias").floatValue    = s.CenterFrontBias;
            so.ApplyModifiedProperties();
        }

        static void ConfigureVehicleManager(VehicleManager vm, BuggySpec s)
        {
            var so = new SerializedObject(vm);
            so.FindProperty("_motorPreset").enumValueIndex     = (int)s.Motor;
            so.FindProperty("_frontSpringStrength").floatValue = s.FrontSpringStrength;
            so.FindProperty("_frontSpringDamping").floatValue  = s.FrontSpringDamping;
            so.FindProperty("_rearSpringStrength").floatValue  = s.RearSpringStrength;
            so.FindProperty("_rearSpringDamping").floatValue   = s.RearSpringDamping;
            so.FindProperty("_gripCoeff").floatValue           = s.GripCoeff;
            so.FindProperty("_steeringMax").floatValue         = s.SteeringMax;
            so.FindProperty("_comGround").vector3Value         = s.CenterOfMass;
            so.FindProperty("_gearRatio").floatValue           = s.GearRatio;
            so.ApplyModifiedProperties();
        }

        // ---- Enum Mappers ----
        static R8EOX.Vehicle.Internal.DriveLayout MapLayout(BuggyDriveLayout layout) =>
            layout == BuggyDriveLayout.AWD
                ? R8EOX.Vehicle.Internal.DriveLayout.AWD
                : R8EOX.Vehicle.Internal.DriveLayout.RWD;

        static R8EOX.Vehicle.Internal.DiffType MapDiff(BuggyDiffType diff) =>
            diff switch {
                BuggyDiffType.BallDiff => R8EOX.Vehicle.Internal.DiffType.BallDiff,
                BuggyDiffType.Spool    => R8EOX.Vehicle.Internal.DiffType.Spool,
                _                      => R8EOX.Vehicle.Internal.DiffType.Open,
            };

        // ---- Private Helpers ----

        static void AddRigidbody(GameObject root, BuggySpec spec)
        {
            var rb = root.AddComponent<Rigidbody>();
            rb.mass = spec.Mass; rb.linearDamping = 0f; rb.angularDamping = 0.05f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            if (spec.InertiaTensor != Vector3.zero) rb.inertiaTensor = spec.InertiaTensor;
            rb.solverIterations = 12; rb.solverVelocityIterations = 4;
        }

        static void AddColliders(GameObject root)
        {
            var chassisMat = GetOrCreatePhysicsMaterial("ChassiBottom", 0.15f, 0.25f);
            var bodyMat    = GetOrCreatePhysicsMaterial("BodyShell",    0.40f, 0.35f);
            var bumperMat  = GetOrCreatePhysicsMaterial("Bumper",       0.50f, 0.20f, PhysicsMaterialCombine.Maximum);
            AddBoxCollider(root, new Vector3(0.14f, 0.03f, 0.39f),   new Vector3(0f, -0.0425f,  0f)).material = chassisMat;
            AddBoxCollider(root, new Vector3(0.12f, 0.07f, 0.25f),   new Vector3(0f,  0.0075f,  0f)).material = bodyMat;
            AddBoxCollider(root, new Vector3(0.11f, 0.035f, 0.04f),  new Vector3(0f, -0.025f,  0.215f)).material = bumperMat;
            AddBoxCollider(root, new Vector3(0.09f, 0.045f, 0.045f), new Vector3(0f, -0.02f,  -0.215f)).material = bumperMat;
        }

        static void AddBodyMeshes(GameObject root, Material dg, Material mg, Material bs, Material bw)
        {
            AddBoxMesh(root, "ChassisPlate",    new Vector3(0.13f, 0.008f, 0.34f),  new Vector3(0f, -0.058f,   0f),    dg);
            AddBoxMesh(root, "FrontBumperMesh", new Vector3(0.12f, 0.03f,  0.025f), new Vector3(0f, -0.038f,  0.195f), dg);
            AddBoxMesh(root, "RearBumperMesh",  new Vector3(0.10f, 0.04f,  0.04f),  new Vector3(0f, -0.038f, -0.18f),  dg);
            AddBoxMesh(root, "FrontShockTower", new Vector3(0.10f, 0.06f,  0.005f), new Vector3(0f, -0.018f,  0.12f),  mg);
            AddBoxMesh(root, "RearShockTower",  new Vector3(0.08f, 0.06f,  0.005f), new Vector3(0f, -0.018f, -0.12f),  mg);
            AddBoxMesh(root, "BodyShell",       new Vector3(0.12f, 0.04f,  0.28f),  new Vector3(0f, -0.0125f,  0.02f), bs);
            AddBoxMesh(root, "RearWing", new Vector3(0.12f, 0.002f, 0.04f), new Vector3(0f, 0.042f, -0.15f), bw)
                .transform.localRotation = Quaternion.Euler(22.5f, 0f, 0f);
        }

        static void AddControlArms(GameObject root, Material dg, BuggySpec spec)
        {
            float ax = spec.TrackHalf * 0.52f, wh = spec.WheelbaseHalf;
            var sz = new Vector3(ax, 0.005f, 0.02f);
            AddBoxMesh(root, "FrontArmL", sz, new Vector3(-ax, -0.053f,  wh), dg);
            AddBoxMesh(root, "FrontArmR", sz, new Vector3( ax, -0.053f,  wh), dg);
            AddBoxMesh(root, "RearArmL",  sz, new Vector3(-ax, -0.053f, -wh), dg);
            AddBoxMesh(root, "RearArmR",  sz, new Vector3( ax, -0.053f, -wh), dg);
        }

        static void BuildWheel(GameObject parent, string name, Vector3 localPos,
            bool isSteer, bool isMotor, float tireR, float tireH, float hubR, float hubH,
            BuggySpec spec, Material tireMat, Material hubMat, int layer)
        {
            var pivot = new GameObject(name);
            pivot.transform.SetParent(parent.transform, false);
            pivot.transform.localPosition = localPos; pivot.layer = layer;
            var wheel = pivot.AddComponent<R8EOX.Vehicle.Internal.RaycastWheel>();
            wheel.IsSteer = isSteer; wheel.IsMotor = isMotor;
            bool front = localPos.z > 0f;
            wheel.SpringStrength = front ? spec.FrontSpringStrength : spec.RearSpringStrength;
            wheel.SpringDamping  = front ? spec.FrontSpringDamping  : spec.RearSpringDamping;
            wheel.RestDistance   = spec.RestDistance;
            var so = new SerializedObject(wheel);
            so.FindProperty("_wheelRadius").floatValue    = tireR;
            so.FindProperty("_overExtend").floatValue     = spec.OverExtend;
            so.FindProperty("_minSpringLen").floatValue   = spec.MinSpringLen;
            so.FindProperty("_maxSpringForce").floatValue = spec.MaxSpringForce;
            so.ApplyModifiedProperties();
            BuildWheelVisual(pivot, "WheelVisual", tireR, tireH, tireMat, layer);
            BuildWheelVisual(pivot, "HubVisual",   hubR,  hubH,  hubMat,  layer);
        }

        static void BuildWheelVisual(GameObject pivot, string name, float radius, float height, Material mat, int layer)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name; go.transform.SetParent(pivot.transform, false);
            go.transform.localPosition = new Vector3(0f, -0.05f, 0f);
            go.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            go.transform.localScale    = new Vector3(radius * 2f, height * 0.5f, radius * 2f);
            go.GetComponent<Renderer>().material = mat;
            Object.DestroyImmediate(go.GetComponent<Collider>()); go.layer = layer;
        }

        static BoxCollider AddBoxCollider(GameObject parent, Vector3 size, Vector3 center)
        { var col = parent.AddComponent<BoxCollider>(); col.size = size; col.center = center; return col; }

        // Beveled skid plates — angled box colliders forming a V-shaped keel under the chassis.
        // Deflects terrain seam edges instead of catching on them (legacy anti-snag fix).
        static void AddSkidPlates(GameObject root, int layer)
        {
            var skidMat = GetOrCreatePhysicsMaterial("SkidPlate", 0.10f, 0.15f);
            var size = new Vector3(0.03f, 0.01f, 0.34f);
            MakeSkidPlate(root, "SkidPlateL", new Vector3(-0.045f, -0.055f, 0f),  25f, size, layer).material = skidMat;
            MakeSkidPlate(root, "SkidPlateR", new Vector3( 0.045f, -0.055f, 0f), -25f, size, layer).material = skidMat;
        }

        static BoxCollider MakeSkidPlate(GameObject root, string name, Vector3 pos, float rotZ, Vector3 size, int layer)
        { var go = new GameObject(name); go.transform.SetParent(root.transform, false); go.transform.localPosition = pos; go.transform.localRotation = Quaternion.Euler(0f, 0f, rotZ); go.layer = layer; var col = go.AddComponent<BoxCollider>(); col.size = size; return col; }

        static GameObject AddBoxMesh(GameObject parent, string name, Vector3 size, Vector3 localPos, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name; go.transform.SetParent(parent.transform, false);
            go.transform.localPosition = localPos; go.transform.localScale = size;
            go.GetComponent<Renderer>().material = mat;
            Object.DestroyImmediate(go.GetComponent<Collider>()); return go;
        }

        static PhysicsMaterial GetOrCreatePhysicsMaterial(string name, float bounce, float friction,
            PhysicsMaterialCombine bounceCombine = PhysicsMaterialCombine.Average)
        {
            const string matDir = "Assets/Materials/Physics";
            string path = $"{matDir}/{name}.physicsMaterial";
            var existing = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(path);
            if (existing != null)
            {
                existing.bounciness = bounce; existing.dynamicFriction = friction;
                existing.staticFriction = friction; existing.bounceCombine = bounceCombine;
                existing.frictionCombine = PhysicsMaterialCombine.Average;
                EditorUtility.SetDirty(existing); return existing;
            }
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Application.dataPath, "..", matDir));
            var mat = new PhysicsMaterial(name) {
                bounciness = bounce, dynamicFriction = friction, staticFriction = friction,
                bounceCombine = bounceCombine, frictionCombine = PhysicsMaterialCombine.Average
            };
            AssetDatabase.CreateAsset(mat, path); return mat;
        }

        static Material GetOrCreateMaterial(string name, Color color, bool transparent = false)
        {
            const string matDir = "Assets/Materials/Vehicle";
            string path = $"{matDir}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) { existing.SetColor("_BaseColor", color); EditorUtility.SetDirty(existing); return existing; }
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Application.dataPath, "..", matDir));
            var mat = new Material(Shader.Find(k_UrpLitShader)) { name = name };
            mat.SetColor("_BaseColor", color);
            if (transparent)
            {
                mat.SetFloat("_Surface", 1f); mat.SetFloat("_Blend", 0f); mat.SetFloat("_AlphaClip", 0f);
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0); mat.DisableKeyword("_ALPHATEST_ON"); mat.EnableKeyword("_ALPHABLEND_ON");
                mat.renderQueue = 3000;
            }
            AssetDatabase.CreateAsset(mat, path); return mat;
        }

        static void AddAttachmentPoints(GameObject root, BuggySpec spec)
        {
            AddEmptyChild(root, "CameraTarget",     new Vector3(0f,  0.0375f, 0.075f));
            AddEmptyChild(root, "CameraMountPoint", new Vector3(0f,  0.75f,  -2f));
            AddEmptyChild(root, "ExhaustPoint",     new Vector3(0f, -0.15f,  -spec.ChassisLength * 0.5f));
            foreach (Transform c in root.transform)
                if (c.GetComponent<R8EOX.Vehicle.Internal.RaycastWheel>() != null)
                    AddEmptyChild(c.gameObject, "VFXPoint", new Vector3(0f, -spec.RestDistance, 0f));
        }

        static void AddEmptyChild(GameObject parent, string name, Vector3 pos)
        { var go = new GameObject(name); go.transform.SetParent(parent.transform, false); go.transform.localPosition = pos; }

        static void SetLayerRecursive(GameObject go, int layer)
        { go.layer = layer; foreach (Transform c in go.transform) SetLayerRecursive(c.gameObject, layer); }
    }
}
#endif
