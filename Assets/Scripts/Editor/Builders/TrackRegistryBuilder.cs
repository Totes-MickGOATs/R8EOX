#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Scans Assets/Art/Tracks/ for track folders, creates or updates a
    /// TrackDefinition asset per track, assembles them into a TrackRegistry,
    /// and wires the registry into the SessionChannel SO.
    /// </summary>
    internal static class TrackRegistryBuilder
    {
        private const string TracksArtRoot     = "Assets/Art/Tracks";
        private const string OutputDir         = "Assets/Settings/Tracks";
        private const string RegistryAssetPath = OutputDir + "/TrackRegistry.asset";

        // ---- Menu Item -------------------------------------------------------

        [MenuItem("R8EOX/Build Track Registry")]
        private static void BuildTrackRegistryMenuItem() => BuildTrackRegistry();

        // ---- Public API ------------------------------------------------------

        internal static void BuildTrackRegistry()
        {
            EnsureOutputDirectory();

            var definitions = new List<TrackDefinition>();
            ScanConventionFolders(definitions);

            if (!definitions.Exists(d => d.SceneName == "PhysicsTestTrack"))
                definitions.Add(BuildPhysicsTestFallback());

            var registry = AssembleRegistry(definitions);
            WireSessionChannel(registry);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                $"[TrackRegistryBuilder] Done. " +
                $"{definitions.Count} TrackDefinition(s) created/updated. " +
                $"Registry: {RegistryAssetPath}");
        }

        // ---- Scan -----------------------------------------------------------

        private static void ScanConventionFolders(List<TrackDefinition> results)
        {
            if (!AssetDatabase.IsValidFolder(TracksArtRoot))
            {
                Debug.LogWarning(
                    "[TrackRegistryBuilder] Assets/Art/Tracks/ not found. " +
                    "No convention-based tracks scanned.");
                return;
            }

            string[] guids = AssetDatabase.FindAssets("", new[] { TracksArtRoot });
            var visited   = new HashSet<string>();

            foreach (string guid in guids)
            {
                string path   = AssetDatabase.GUIDToAssetPath(guid);
                string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');

                if (parent != TracksArtRoot)
                    continue;
                if (!AssetDatabase.IsValidFolder(path))
                    continue;
                if (!visited.Add(path))
                    continue;

                string trackName = Path.GetFileName(path);
                var def = BuildFromTrackFolder(path, trackName);
                if (def != null)
                    results.Add(def);
            }

            if (visited.Count == 0)
                Debug.LogWarning(
                    "[TrackRegistryBuilder] No subfolders found under " +
                    TracksArtRoot + ".");
        }

        // ---- Per-folder build -----------------------------------------------

        private static TrackDefinition BuildFromTrackFolder(
            string folderPath, string trackName)
        {
            TrackConfig config  = LoadTrackConfig(folderPath);
            string      defPath = OutputDir + "/" + trackName + "_TrackDef.asset";

            var def = CreateOrLoad<TrackDefinition>(defPath);
            ApplyTrackDefProps(def, config, trackName);
            return def;
        }

        private static void ApplyTrackDefProps(
            TrackDefinition def, TrackConfig config, string trackName)
        {
            string displayName  = config != null ? config.TrackName : trackName;
            int    trackTypeIdx = config != null ? (int)config.TrackType : 0;

            var so = new SerializedObject(def);
            so.FindProperty("displayName").stringValue  = displayName;
            so.FindProperty("sceneName").stringValue    = trackName;
            so.FindProperty("trackType").enumValueIndex = trackTypeIdx;
            so.FindProperty("isLocked").boolValue       = false;
            so.FindProperty("description").stringValue  = "";
            SetSinglePracticeMode(so);
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(def);
        }

        private static TrackDefinition BuildPhysicsTestFallback()
        {
            const string defPath = OutputDir + "/PhysicsTestTrack_TrackDef.asset";
            var def = CreateOrLoad<TrackDefinition>(defPath);
            var so  = new SerializedObject(def);
            so.FindProperty("displayName").stringValue  = "Physics Test Track";
            so.FindProperty("sceneName").stringValue    = "PhysicsTestTrack";
            so.FindProperty("trackType").enumValueIndex = (int)TrackType.Circuit;
            so.FindProperty("isLocked").boolValue       = false;
            so.FindProperty("description").stringValue  = "";
            SetSinglePracticeMode(so);
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(def);
            return def;
        }

        // ---- Registry assembly -----------------------------------------------

        private static TrackRegistry AssembleRegistry(
            List<TrackDefinition> definitions)
        {
            var registry = CreateOrLoad<TrackRegistry>(RegistryAssetPath);
            var so       = new SerializedObject(registry);
            var arr      = so.FindProperty("tracks");
            arr.arraySize = definitions.Count;
            for (int i = 0; i < definitions.Count; i++)
                arr.GetArrayElementAtIndex(i).objectReferenceValue = definitions[i];
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(registry);
            return registry;
        }

        // ---- SessionChannel wiring ------------------------------------------

        private static void WireSessionChannel(TrackRegistry registry)
        {
            string[] guids = AssetDatabase.FindAssets("t:SessionChannel");
            if (guids.Length == 0)
            {
                Debug.LogWarning(
                    "[TrackRegistryBuilder] No SessionChannel asset found.");
                return;
            }

            string channelPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            var channel = AssetDatabase.LoadAssetAtPath<ScriptableObject>(channelPath);
            if (channel == null)
                return;

            var so   = new SerializedObject(channel);
            var prop = so.FindProperty("trackRegistry");
            if (prop == null)
            {
                Debug.LogWarning(
                    "[TrackRegistryBuilder] SessionChannel missing " +
                    "'trackRegistry' property.");
                return;
            }

            prop.objectReferenceValue = registry;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(channel);
        }

        // ---- Helpers --------------------------------------------------------

        private static void SetSinglePracticeMode(SerializedObject so)
        {
            var arr = so.FindProperty("supportedModes");
            arr.arraySize = 1;
            arr.GetArrayElementAtIndex(0).enumValueIndex = (int)SessionMode.Practice;
        }

        private static TrackConfig LoadTrackConfig(string folderPath)
        {
            string[] guids = AssetDatabase.FindAssets(
                "t:TrackConfig", new[] { folderPath });
            if (guids.Length == 0)
                return null;

            return AssetDatabase.LoadAssetAtPath<TrackConfig>(
                AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        private static T CreateOrLoad<T>(string assetPath)
            where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existing != null)
                return existing;

            var instance = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(instance, assetPath);
            return instance;
        }

        private static void EnsureOutputDirectory()
        {
            if (!AssetDatabase.IsValidFolder(OutputDir))
                AssetDatabase.CreateFolder("Assets/Settings", "Tracks");
        }
    }
}
#endif
