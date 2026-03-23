#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Builds a complete physics test track scene with ground, obstacles,
    /// waypoints, lighting, camera, and PhysicsTestManager — ready to play.
    /// </summary>
    internal static class PhysicsTestTrackBuilder
    {
        const string k_UrpLitShader = "Universal Render Pipeline/Lit";
        const string k_ScenePath    = "Assets/Scenes/PhysicsTestTrack.unity";
        const string k_PrefabPath   = "Assets/Prefabs/RCBuggy.prefab";

        // ---- Menu Item -------------------------------------------------------

        [MenuItem("R8EOX/Build Physics Test Track")]
        internal static void Build()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            Material groundMat   = CreateMaterial("Ground",   new Color(0.4f,  0.4f,  0.4f));
            Material obstacleMat = CreateMaterial("Obstacle", new Color(0.25f, 0.25f, 0.25f));

            AddLighting();
            AddCamera();
            BuildGround(groundMat);
            BuildObstacles(obstacleMat);

            GameObject waypointPathGO = BuildWaypoints();
            Transform  spawnPoint     = BuildSpawnPoint();
            WireManager(waypointPathGO, spawnPoint);

            System.IO.Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene(), k_ScenePath);

            AssetDatabase.Refresh();
            Debug.Log("[PhysicsTestTrackBuilder] Scene saved to " + k_ScenePath);
        }

        // ---- Scene elements --------------------------------------------------

        static void AddLighting()
        {
            GameObject go = new GameObject("Directional Light");
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Light lt = go.AddComponent<Light>();
            lt.type      = LightType.Directional;
            lt.color     = new Color(1f, 0.96f, 0.88f);
            lt.intensity = 1.1f;
        }

        static void AddCamera()
        {
            GameObject go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            go.transform.SetPositionAndRotation(
                new Vector3(0f, 3f, -8f),
                Quaternion.LookRotation(new Vector3(0f, -0.35f, 1f).normalized));
            go.AddComponent<UnityEngine.Camera>();
            go.AddComponent<AudioListener>();
        }

        static void BuildGround(Material mat)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Ground";
            go.transform.SetPositionAndRotation(new Vector3(4f, -0.25f, 20f), Quaternion.identity);
            go.transform.localScale = new Vector3(20f, 0.5f, 80f);
            go.GetComponent<Renderer>().material = mat;
        }

        static void BuildObstacles(Material mat)
        {
            GameObject parent = new GameObject("Obstacles");

            // Speed bumps at Z = 10, 12, 14
            for (int i = 0; i < 3; i++)
                SpawnBox(parent, mat, new Vector3(0f, 0f, 10f + i * 2f), new Vector3(2f, 0.03f, 0.15f), 0f);

            // Tabletop jump: ramp up, flat top, ramp down
            SpawnBox(parent, mat, new Vector3(0f, 0f, 18f), new Vector3(2f, 0.15f, 1f),  15f);
            SpawnBox(parent, mat, new Vector3(0f, 0.075f, 20f), new Vector3(2f, 0.15f, 2f), 0f);
            SpawnBox(parent, mat, new Vector3(0f, 0f, 22f), new Vector3(2f, 0.15f, 1f), -15f);

            // Washboard — 8 ridges spaced 0.75 m apart from Z=26
            for (int i = 0; i < 8; i++)
                SpawnBox(parent, mat, new Vector3(0f, 0f, 26f + i * 0.75f), new Vector3(2f, 0.02f, 0.08f), 0f);
        }

        static void SpawnBox(GameObject parent, Material mat,
            Vector3 pos, Vector3 size, float xRotDeg)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Obstacle";
            go.transform.SetParent(parent.transform, false);
            go.transform.localPosition = pos;
            go.transform.localScale    = size;
            go.transform.localRotation = Quaternion.Euler(xRotDeg, 0f, 0f);
            go.GetComponent<Renderer>().material = mat;
        }

        static GameObject BuildWaypoints()
        {
            Vector3[] positions =
            {
                new Vector3(0f, 0f,  0f),   // WP_00 start
                new Vector3(0f, 0f,  4f),   // WP_01 straight
                new Vector3(0f, 0f,  8f),   // WP_02 end straight
                new Vector3(0f, 0f, 10f),   // WP_03 speed bumps start
                new Vector3(0f, 0f, 12f),   // WP_04 speed bumps mid
                new Vector3(0f, 0f, 14f),   // WP_05 speed bumps end
                new Vector3(0f, 0f, 16f),   // WP_06 approach jump
                new Vector3(0f, 0f, 20f),   // WP_07 jump
                new Vector3(0f, 0f, 24f),   // WP_08 post jump
                new Vector3(0f, 0f, 26f),   // WP_09 washboard start
                new Vector3(0f, 0f, 30f),   // WP_10 washboard mid
                new Vector3(0f, 0f, 34f),   // WP_11 washboard end
                new Vector3(0f, 0f, 38f),   // WP_12 sweeper entry
                new Vector3(5f, 0f, 42f),   // WP_13 sweeper apex
                new Vector3(8f, 0f, 38f),   // WP_14 sweeper exit
                new Vector3(8f, 0f, 30f),   // WP_15 return straight
                new Vector3(8f, 0f, 20f),   // WP_16 return mid
                new Vector3(5f, 0f, 10f),   // WP_17 return curve
                new Vector3(3f, 0f,  4f),   // WP_18 approach start
                new Vector3(1f, 0f,  1f),   // WP_19 close loop
            };

            GameObject pathGO = new GameObject("WaypointPath");
            pathGO.AddComponent<R8EOX.PhysicsTest.Internal.WaypointPath>();

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject wp = new GameObject($"WP_{i:D2}");
                wp.transform.SetParent(pathGO.transform, false);
                wp.transform.localPosition = positions[i];
            }

            return pathGO;
        }

        static Transform BuildSpawnPoint()
        {
            GameObject go = new GameObject("SpawnPoint");
            go.transform.SetPositionAndRotation(new Vector3(0f, 0.5f, 0f), Quaternion.identity);
            return go.transform;
        }

        static void WireManager(GameObject pathGO, Transform spawnTransform)
        {
            GameObject go = new GameObject("PhysicsTestManager");
            var mgr = go.AddComponent<R8EOX.PhysicsTest.PhysicsTestManager>();
            var so  = new SerializedObject(mgr);

            so.FindProperty("waypointPath").objectReferenceValue =
                pathGO.GetComponent<R8EOX.PhysicsTest.Internal.WaypointPath>();
            so.FindProperty("spawnPoint").objectReferenceValue   = spawnTransform;
            so.FindProperty("vehiclePrefab").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<GameObject>(k_PrefabPath);

            string[] names   = { "Straight", "Bumps", "Jump", "Washboard", "Sweeper", "Return" };
            int[]    indices = { 0, 3, 6, 9, 12, 15 };

            SerializedProperty namesProp = so.FindProperty("segmentNames");
            namesProp.arraySize = names.Length;
            for (int i = 0; i < names.Length; i++)
                namesProp.GetArrayElementAtIndex(i).stringValue = names[i];

            SerializedProperty idxProp = so.FindProperty("segmentWaypointIndices");
            idxProp.arraySize = indices.Length;
            for (int i = 0; i < indices.Length; i++)
                idxProp.GetArrayElementAtIndex(i).intValue = indices[i];

            so.ApplyModifiedProperties();
        }

        // ---- Helpers ---------------------------------------------------------

        static Material CreateMaterial(string matName, Color color)
        {
            var mat = new Material(Shader.Find(k_UrpLitShader)) { name = matName };
            mat.SetColor("_BaseColor", color);
            return mat;
        }
    }
}
#endif
