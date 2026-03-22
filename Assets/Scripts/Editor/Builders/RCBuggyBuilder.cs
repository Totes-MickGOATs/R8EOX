#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using R8EOX.Vehicle;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Constructs the RCBuggy GameObject hierarchy including chassis, wheels,
    /// colliders, visuals, and core components.
    /// </summary>
    internal static class RCBuggyBuilder
    {
        // ---- Constants ----

        const int k_DefaultCarLayer = 8;
        const string k_UrpLitShader = "Universal Render Pipeline/Lit";

        // ---- Public API ----

        /// <summary>
        /// Builds the full RCBuggy hierarchy and returns the root GameObject.
        /// </summary>
        internal static GameObject BuildRCBuggy()
        {
            GameObject root = new GameObject("RCBuggy");

            int carLayer = LayerMask.NameToLayer("Vehicle");
            if (carLayer < 0) carLayer = k_DefaultCarLayer;
            root.layer = carLayer;

            AddRigidbody(root);
            root.AddComponent<R8EOX.Input.RCInput>();
            root.AddComponent<VehicleManager>();
            AddColliders(root);

            Material darkGrey = CreateMaterial("DarkGrey", new Color(0.2f, 0.2f, 0.2f));
            Material medGrey = CreateMaterial("MediumGrey", new Color(0.5f, 0.5f, 0.5f));
            Material blueSemi = CreateMaterial("BlueSemi",
                new Color(0.18f, 0.45f, 0.9f, 0.85f), transparent: true);
            Material blueSolid = CreateMaterial("BlueSolid", new Color(0.18f, 0.45f, 0.9f));
            Material tireMat = CreateMaterial("BlackTire", new Color(0.05f, 0.05f, 0.05f));
            Material hubMat = CreateMaterial("WhiteHub", new Color(0.9f, 0.9f, 0.9f));

            AddBodyMeshes(root, darkGrey, medGrey, blueSemi, blueSolid);
            AddControlArms(root, darkGrey);

            BuildWheel(root, "WheelFL", new Vector3(-0.5f, 0f,  0.68f),
                isSteer: true,  isMotor: false, 0.166f, 0.112f, 0.11f, 0.128f, tireMat, hubMat, carLayer);
            BuildWheel(root, "WheelFR", new Vector3( 0.5f, 0f,  0.68f),
                isSteer: true,  isMotor: false, 0.166f, 0.112f, 0.11f, 0.128f, tireMat, hubMat, carLayer);
            BuildWheel(root, "WheelRL", new Vector3(-0.5f, 0f, -0.68f),
                isSteer: false, isMotor: true,  0.166f, 0.168f, 0.11f, 0.184f, tireMat, hubMat, carLayer);
            BuildWheel(root, "WheelRR", new Vector3( 0.5f, 0f, -0.68f),
                isSteer: false, isMotor: true,  0.166f, 0.168f, 0.11f, 0.184f, tireMat, hubMat, carLayer);

            GameObject airPhysGO = new GameObject("AirPhysics");
            airPhysGO.transform.SetParent(root.transform, false);
            airPhysGO.AddComponent<R8EOX.Vehicle.Internal.RCAirPhysics>();

            GameObject drivetrainGO = new GameObject("Drivetrain");
            drivetrainGO.transform.SetParent(root.transform, false);
            drivetrainGO.AddComponent<R8EOX.Vehicle.Internal.Drivetrain>();

            SetLayerRecursive(root, carLayer);
            return root;
        }

        // ---- Private Helpers ----

        static void AddRigidbody(GameObject root)
        {
            var rb = root.AddComponent<Rigidbody>();
            rb.mass = 1.5f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        static void AddColliders(GameObject root)
        {
            AddBoxCollider(root, new Vector3(0.56f, 0.12f, 1.56f), new Vector3(0f, -0.17f, 0f));
            AddBoxCollider(root, new Vector3(0.48f, 0.28f, 1.0f),  new Vector3(0f,  0.03f, 0f));
            AddBoxCollider(root, new Vector3(0.44f, 0.14f, 0.16f), new Vector3(0f, -0.10f,  0.86f));
            AddBoxCollider(root, new Vector3(0.36f, 0.18f, 0.18f), new Vector3(0f, -0.08f, -0.86f));
        }

        static void AddBodyMeshes(GameObject root, Material darkGrey, Material medGrey,
                                   Material blueSemi, Material blueSolid)
        {
            AddBoxMesh(root, "ChassisPlate",     new Vector3(0.52f, 0.032f, 1.36f),
                new Vector3(0f, -0.233f,  0f),    darkGrey);
            AddBoxMesh(root, "FrontBumperMesh",  new Vector3(0.48f, 0.12f,  0.1f),
                new Vector3(0f, -0.153f,  0.78f), darkGrey);
            AddBoxMesh(root, "RearBumperMesh",   new Vector3(0.4f,  0.16f,  0.16f),
                new Vector3(0f, -0.153f, -0.72f), darkGrey);
            AddBoxMesh(root, "FrontShockTower",  new Vector3(0.4f,  0.24f,  0.02f),
                new Vector3(0f, -0.073f,  0.48f), medGrey);
            AddBoxMesh(root, "RearShockTower",   new Vector3(0.32f, 0.24f,  0.02f),
                new Vector3(0f, -0.073f, -0.48f), medGrey);
            AddBoxMesh(root, "BodyShell",        new Vector3(0.48f, 0.16f,  1.12f),
                new Vector3(0f, -0.05f,   0.08f), blueSemi);

            GameObject wing = AddBoxMesh(root, "RearWing",
                new Vector3(0.48f, 0.008f, 0.16f), new Vector3(0f, 0.167f, -0.6f), blueSolid);
            wing.transform.localRotation = Quaternion.Euler(-22.5f, 0f, 0f);
        }

        static void AddControlArms(GameObject root, Material darkGrey)
        {
            AddBoxMesh(root, "FrontArmL", new Vector3(0.26f, 0.02f, 0.08f),
                new Vector3(-0.26f, -0.213f,  0.68f), darkGrey);
            AddBoxMesh(root, "FrontArmR", new Vector3(0.26f, 0.02f, 0.08f),
                new Vector3( 0.26f, -0.213f,  0.68f), darkGrey);
            AddBoxMesh(root, "RearArmL",  new Vector3(0.26f, 0.02f, 0.08f),
                new Vector3(-0.26f, -0.213f, -0.68f), darkGrey);
            AddBoxMesh(root, "RearArmR",  new Vector3(0.26f, 0.02f, 0.08f),
                new Vector3( 0.26f, -0.213f, -0.68f), darkGrey);
        }

        static void BuildWheel(GameObject parent, string name, Vector3 localPos,
            bool isSteer, bool isMotor,
            float tireRadius, float tireHeight, float hubRadius, float hubHeight,
            Material tireMat, Material hubMat, int layer)
        {
            GameObject pivot = new GameObject(name);
            pivot.transform.SetParent(parent.transform, false);
            pivot.transform.localPosition = localPos;
            pivot.layer = layer;

            var wheel = pivot.AddComponent<R8EOX.Vehicle.Internal.RaycastWheel>();
            wheel.IsSteer = isSteer;
            wheel.IsMotor = isMotor;
            wheel.SpringStrength = 75f;
            wheel.SpringDamping = 4.25f;
            wheel.RestDistance = 0.20f;
            var so = new SerializedObject(wheel);
            so.FindProperty("_overExtend").floatValue = 0.08f;
            so.FindProperty("_minSpringLen").floatValue = 0.032f;
            so.FindProperty("_wheelRadius").floatValue = tireRadius;
            so.FindProperty("_maxSpringForce").floatValue = 50f;
            so.ApplyModifiedProperties();

            BuildWheelVisual(pivot, "WheelVisual", tireRadius, tireHeight, tireMat, layer);
            BuildWheelVisual(pivot, "HubVisual",   hubRadius,  hubHeight,  hubMat,  layer);
        }

        static void BuildWheelVisual(GameObject pivot, string name,
            float radius, float height, Material mat, int layer)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(pivot.transform, false);
            go.transform.localPosition = new Vector3(0f, -0.2f, 0f);
            go.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            go.transform.localScale = new Vector3(radius * 2f, height * 0.5f, radius * 2f);
            go.GetComponent<Renderer>().material = mat;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.layer = layer;
        }

        static void AddBoxCollider(GameObject parent, Vector3 size, Vector3 center)
        {
            var col = parent.AddComponent<BoxCollider>();
            col.size = size;
            col.center = center;
        }

        static GameObject AddBoxMesh(GameObject parent, string name,
            Vector3 size, Vector3 localPos, Material mat)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent.transform, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = size;
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

        static void SetLayerRecursive(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
                SetLayerRecursive(child.gameObject, layer);
        }
    }
}
#endif
