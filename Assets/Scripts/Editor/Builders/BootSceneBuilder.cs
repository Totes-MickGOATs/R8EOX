#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace R8EOX.Editor.Builders
{
    internal static class BootSceneBuilder
    {
        [MenuItem("R8EOX/Build Boot Scene")]
        private static void Build()
        {
            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var root = new GameObject("[AppRoot]");
            root.AddComponent<R8EOX.App.AppManager>();

            WireSessionChannel(root.GetComponent<R8EOX.App.AppManager>());

            string path = "Assets/Scenes/Boot.unity";
            EditorSceneManager.SaveScene(scene, path);
            UpdateBuildSettings(path);

            Debug.Log("[BootSceneBuilder] Boot scene created at " + path);
        }

        private static void WireSessionChannel(R8EOX.App.AppManager appManager)
        {
            var guids = AssetDatabase.FindAssets("t:SessionChannel");
            if (guids.Length == 0)
            {
                Debug.LogWarning("[BootSceneBuilder] No SessionChannel found.");
                return;
            }
            var channel = AssetDatabase.LoadAssetAtPath<R8EOX.SessionChannel>(
                AssetDatabase.GUIDToAssetPath(guids[0]));
            var so = new SerializedObject(appManager);
            so.FindProperty("sessionChannel").objectReferenceValue = channel;
            so.ApplyModifiedProperties();
        }

        private static void UpdateBuildSettings(string bootPath)
        {
            const string mainMenuPath = "Assets/Scenes/MainMenu.unity";

            var current = EditorBuildSettings.scenes;
            var scenes = new List<EditorBuildSettingsScene>(current);

            // Remove any existing Boot.unity entry
            scenes.RemoveAll(s => s.path == bootPath);

            // Insert Boot at index 0
            scenes.Insert(0, new EditorBuildSettingsScene(bootPath, true));

            // Ensure MainMenu.unity is at index 1 if it exists
            if (AssetDatabase.AssetPathExists(mainMenuPath))
            {
                scenes.RemoveAll(s => s.path == mainMenuPath);
                scenes.Insert(1, new EditorBuildSettingsScene(mainMenuPath, true));
                Debug.Log("[BootSceneBuilder] MainMenu.unity pinned to index 1.");
            }

            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("[BootSceneBuilder] Build Settings updated. Boot at index 0.");
        }
    }
}
#endif
