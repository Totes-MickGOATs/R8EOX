#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using R8EOX.Editor.Builders;

namespace R8EOX.Editor
{
    /// <summary>
    /// Generic track builder that scans a track folder by convention,
    /// then delegates to existing builder classes.
    /// Use: Menu -> R8EOX -> Tracks -> Build Selected / Build All
    /// </summary>
    public static class TrackBuilder
    {
        // Default terrain dimensions (used when no TerrainSettings SO)
        const float k_DefaultTerrainWidth = 500f;
        const float k_DefaultTerrainLength = 500f;
        const int k_DefaultHeightmapRes = 2049;

        [MenuItem("R8EOX/Tracks/Build Selected")]
        static void BuildSelected()
        {
            Object selected = Selection.activeObject;
            if (selected == null)
            {
                Debug.LogError(
                    "[TrackBuilder] No folder selected in Project window.");
                return;
            }

            string path = AssetDatabase.GetAssetPath(selected);
            if (!AssetDatabase.IsValidFolder(path) ||
                !path.StartsWith("Assets/Tracks/"))
            {
                Debug.LogError(
                    "[TrackBuilder] Select a folder under Assets/Tracks/.");
                return;
            }

            Build(path);
        }

        [MenuItem("R8EOX/Tracks/Build All")]
        static void BuildAllTracks()
        {
            string tracksAbsolute = Application.dataPath + "/Tracks";
            if (!Directory.Exists(tracksAbsolute))
            {
                Debug.LogError(
                    "[TrackBuilder] Assets/Tracks/ directory not found.");
                return;
            }

            string[] trackDirs = Directory.GetDirectories(tracksAbsolute);
            int built = 0;
            foreach (string dir in trackDirs)
            {
                string terrainSub = Path.Combine(dir, "Terrain");
                if (!Directory.Exists(terrainSub))
                    continue;

                string folderName = Path.GetFileName(dir);
                Build("Assets/Tracks/" + folderName);
                built++;
            }

            Debug.Log(
                $"[TrackBuilder] Build All complete. " +
                $"{built} track(s) processed.");
        }

        internal static void Build(string trackFolderPath)
        {
            Debug.Log($"[TrackBuilder] Building track: {trackFolderPath}");

            TrackFolderData scan = TrackFolderScanner.Scan(trackFolderPath);

            // Validate required assets
            if (string.IsNullOrEmpty(scan.HeightmapPath))
            {
                Debug.LogError(
                    "[TrackBuilder] No heightmap.raw found in " +
                    $"{trackFolderPath}/Terrain");
                return;
            }
            if (scan.Layers.Count == 0)
            {
                Debug.LogError(
                    "[TrackBuilder] No terrain layers found in " +
                    $"{trackFolderPath}/Terrain/Layers/");
                return;
            }

            // Ensure Generated folder exists
            if (!AssetDatabase.IsValidFolder(scan.GeneratedFolder))
                AssetDatabase.CreateFolder(trackFolderPath, "Generated");

            // Resolve dimensions for CreateTerrainGameObject
            TerrainSettings ts = scan.TerrainSettingsAsset;
            int heightmapRes = ts != null
                ? ts.HeightmapResolution : k_DefaultHeightmapRes;
            float terrainWidth = ts != null
                ? ts.TerrainWidth : k_DefaultTerrainWidth;
            float terrainLength = ts != null
                ? ts.TerrainLength : k_DefaultTerrainLength;

            // 1. Load or create TerrainData
            TerrainData terrainData =
                TerrainBuilder.LoadOrCreateTerrainData(
                    scan.GeneratedFolder, scan.TrackName,
                    scan.TerrainSettingsAsset);

            // 2. Import heightmap
            TerrainBuilder.ImportHeightmap(
                terrainData, scan.HeightmapPath, heightmapRes);

            // 3. Configure terrain layers
            TerrainLayerBuilder.ConfigureTerrainLayers(
                terrainData, scan.Layers, scan.GeneratedFolder);

            // 4. Apply per-layer blend mask splatmap
            TerrainSplatmapBuilder.ApplyBlendMaskSplatmap(
                terrainData, scan.Layers);

            EditorUtility.SetDirty(terrainData);
            AssetDatabase.SaveAssets();

            // 5. Create terrain GameObject
            GameObject terrainGO =
                TerrainBuilder.CreateTerrainGameObject(
                    terrainData, terrainWidth, terrainLength);
            TerrainBuilder.ConfigureTerrain(
                terrainGO, scan.GeneratedFolder, scan.TrackName,
                scan.TerrainSettingsAsset);

            // 6. Environment
            if (!string.IsNullOrEmpty(scan.SkyboxHdrPath))
            {
                EnvironmentBuilder.SetupEnvironment(
                    scan.SkyboxHdrPath,
                    scan.GeneratedFolder,
                    scan.TrackName,
                    scan.EnvironmentSettingsAsset);
            }

            // 7. Place managers (convention — every track gets a complete scene)
            SceneSetupBuilder.PlaceManagers();

            // 7b. Wire TrackConfig from scan data
            if (scan.TrackConfigAsset != null)
            {
                var tm = Object.FindAnyObjectByType<R8EOX.Track.TrackManager>();
                if (tm != null)
                {
                    var tmSO = new SerializedObject(tm);
                    tmSO.FindProperty("config").objectReferenceValue =
                        scan.TrackConfigAsset;
                    tmSO.ApplyModifiedProperties();
                }
            }
            else
            {
                Debug.LogWarning(
                    "[TrackBuilder] No TrackConfig found in " +
                    $"{trackFolderPath}. Create one at the track root.");
            }

            // 8. Save and finish
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(
                EditorSceneManager.GetActiveScene());

            Debug.Log(
                $"[TrackBuilder] Track '{scan.TrackName}' built " +
                "successfully!");
            Debug.Log(
                $"  Terrain: {terrainWidth}x{terrainLength}m");
            Debug.Log(
                $"  Layers: {scan.Layers.Count}, " +
                $"Heightmap: {heightmapRes}x{heightmapRes}");
            Selection.activeGameObject = terrainGO;
        }
    }
}
#endif
