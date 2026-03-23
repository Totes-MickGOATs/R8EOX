#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace R8EOX.Editor
{
    [InitializeOnLoad]
    public static class BuildSettingsValidator
    {
        private static readonly HashSet<string> EditorOnlyScenes = new HashSet<string>
        {
            "PhysicsTestTrack",
            "UITestScene"
        };

        static BuildSettingsValidator()
        {
            EditorApplication.delayCall += Validate;
        }

        [MenuItem("R8EOX/Validate Build Settings")]
        public static void Validate()
        {
            string[] guids = AssetDatabase.FindAssets("t:SceneAsset", new[] { "Assets/Scenes" });
            List<string> runtimeScenePaths = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !EditorOnlyScenes.Contains(GetSceneName(path)))
                .ToList();

            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
            HashSet<string> buildPaths = new HashSet<string>(buildScenes.Select(s => s.path));

            bool anyIssue = false;

            foreach (string path in runtimeScenePaths)
            {
                if (!buildPaths.Contains(path))
                {
                    Debug.LogWarning($"[BuildSettingsValidator] Runtime scene missing from Build Settings: {path}");
                    anyIssue = true;
                }
            }

            foreach (EditorBuildSettingsScene scene in buildScenes)
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path) == null)
                {
                    Debug.LogWarning($"[BuildSettingsValidator] Ghost entry in Build Settings (file not found): {scene.path}");
                    anyIssue = true;
                }
            }

            if (!anyIssue)
            {
                Debug.Log("[BuildSettingsValidator] Build Settings are valid — no issues found.");
            }
        }

        [MenuItem("R8EOX/Fix Build Settings")]
        public static void Fix()
        {
            string[] guids = AssetDatabase.FindAssets("t:SceneAsset", new[] { "Assets/Scenes" });
            List<string> runtimeScenePaths = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !EditorOnlyScenes.Contains(GetSceneName(path)))
                .ToList();

            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

            List<EditorBuildSettingsScene> cleaned = buildScenes
                .Where(s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path) != null)
                .ToList();

            int ghostsRemoved = buildScenes.Length - cleaned.Count;

            HashSet<string> existingPaths = new HashSet<string>(cleaned.Select(s => s.path));
            List<EditorBuildSettingsScene> missing = runtimeScenePaths
                .Where(p => !existingPaths.Contains(p))
                .Select(p => new EditorBuildSettingsScene(p, true))
                .ToList();

            if (missing.Count > 0)
            {
                cleaned.InsertRange(0, missing);
            }

            EditorBuildSettings.scenes = cleaned.ToArray();

            Debug.Log($"[BuildSettingsValidator] Fix complete — {ghostsRemoved} ghost(s) removed, {missing.Count} scene(s) added.");
        }

        private static string GetSceneName(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }
    }
}
#endif
