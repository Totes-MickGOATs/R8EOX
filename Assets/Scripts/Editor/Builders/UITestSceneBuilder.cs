#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace R8EOX.Editor.Builders
{
    internal static class UITestSceneBuilder
    {
        [MenuItem("R8EOX/Build UI Test Scene")]
        static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "UITestScene";

            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            var cam = camGO.AddComponent<UnityEngine.Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
            cam.orthographic = false;
            cam.fieldOfView = 60f;
            camGO.AddComponent<AudioListener>();

            var lightGO = new GameObject("Directional Light");
            var light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var bootstrapGO = new GameObject("UITestBootstrapper");
            var bootstrapper = bootstrapGO.AddComponent<R8EOX.UI.Internal.UITestBootstrapper>();

            var registryAsset = AssetDatabase.LoadAssetAtPath<R8EOX.VehicleRegistry>(
                "Assets/Settings/VehicleRegistry.asset");
            if (registryAsset != null)
            {
                var so = new SerializedObject(bootstrapper);
                so.FindProperty("registry").objectReferenceValue = registryAsset;
                so.ApplyModifiedProperties();
                Debug.Log("[UITestSceneBuilder] VehicleRegistry wired.");
            }
            else
            {
                Debug.LogWarning("[UITestSceneBuilder] VehicleRegistry.asset not found at Assets/Settings/VehicleRegistry.asset");
            }

            string scenePath = "Assets/Scenes/UITestScene.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[UITestSceneBuilder] Saved {scenePath}. Enter Play Mode to test overlay.");
        }
    }
}
#endif
