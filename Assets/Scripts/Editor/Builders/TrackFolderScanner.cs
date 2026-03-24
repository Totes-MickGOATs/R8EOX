#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Scans a track folder and discovers files by naming convention.
    /// Returns a <see cref="TrackFolderData"/> with all discovered paths.
    /// </summary>
    internal static class TrackFolderScanner
    {
        internal static TrackFolderData Scan(string trackFolderPath)
        {
            string trackName = Path.GetFileName(trackFolderPath);
            string generatedFolder = trackFolderPath + "/Generated";

            // Environment
            string environmentFolder = trackFolderPath + "/Environment";
            string skyboxHdrPath = FindHdrInFolder(environmentFolder);
            EnvironmentSettings envSettings =
                FindScriptableObject<EnvironmentSettings>(environmentFolder);

            // TrackConfig (root of track folder)
            TrackConfig trackConfig =
                FindScriptableObject<TrackConfig>(trackFolderPath);

            // Terrain
            string terrainFolder = trackFolderPath + "/Terrain";
            string heightmapPath = FindHeightmap(terrainFolder);
            TerrainSettings terrainSettings =
                FindScriptableObject<TerrainSettings>(terrainFolder);
            // Layers
            List<LayerData> layers = ScanLayers(trackFolderPath);

            return new TrackFolderData(
                trackName,
                heightmapPath,
                terrainSettings,
                layers,
                skyboxHdrPath,
                envSettings,
                generatedFolder,
                trackConfig);
        }

        static string FindHdrInFolder(string folder)
        {
            string[] guids = AssetDatabase.FindAssets(
                "t:Texture2D", new[] { folder });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(".hdr", StringComparison.OrdinalIgnoreCase))
                    return path;
            }
            return null;
        }

        static string FindHeightmap(string terrainFolder)
        {
            string relative = terrainFolder + "/heightmap.raw";
            string absolute =
                Path.GetDirectoryName(Application.dataPath) + "/" + relative;
            return File.Exists(absolute) ? relative : null;
        }

        static T FindScriptableObject<T>(string folder) where T : ScriptableObject
        {
            string typeName = typeof(T).Name;
            string[] guids = AssetDatabase.FindAssets(
                $"t:{typeName}", new[] { folder });
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }

            // Fallback: name search without type filter, then direct path
            string[] nameGuids = AssetDatabase.FindAssets(
                typeName, new[] { folder });
            foreach (string guid in nameGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith(".asset")) continue;
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null) return asset;
            }

            return AssetDatabase.LoadAssetAtPath<T>(
                $"{folder}/{typeName}.asset");
        }

        static List<LayerData> ScanLayers(string trackFolderPath)
        {
            string layersFolder = trackFolderPath + "/Terrain/Layers";
            string layersAbsolute =
                Path.GetDirectoryName(Application.dataPath) + "/" + layersFolder;
            if (!Directory.Exists(layersAbsolute))
                return new List<LayerData>();

            string[] subdirs = Directory.GetDirectories(layersAbsolute);
            var sorted = subdirs
                .Select(d => new { Full = d, Name = Path.GetFileName(d) })
                .OrderBy(d => ParsePrefix(d.Name))
                .ToList();

            var layers = new List<LayerData>();
            foreach (var dir in sorted)
            {
                string relDir = layersFolder + "/" + dir.Name;
                int index = ParsePrefix(dir.Name);
                string name = ParseLayerName(dir.Name);
                string diffuse = FindTextureInFolder(relDir, "diffuse");
                string normal = FindTextureInFolder(relDir, "normal");
                string arm = FindTextureInFolder(relDir, "arm");
                LayerSettings ls =
                    FindScriptableObject<LayerSettings>(relDir);
                string blendMask =
                    FindTextureInFolder(relDir, "blend-mask");

                layers.Add(new LayerData(
                    index, name, diffuse, normal, arm, ls, blendMask));
            }

            return layers;
        }

        static string FindTextureInFolder(
            string relativeFolder, string baseName)
        {
            string[] guids = AssetDatabase.FindAssets(
                "t:Texture2D", new[] { relativeFolder });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (fileName.Equals(
                        baseName, StringComparison.OrdinalIgnoreCase))
                    return path;
            }
            return null;
        }

        internal static int ParsePrefix(string folderName)
        {
            int underscoreIdx = folderName.IndexOf('_');
            if (underscoreIdx > 0 &&
                int.TryParse(
                    folderName.Substring(0, underscoreIdx), out int prefix))
                return prefix;
            return int.MaxValue;
        }

        internal static string ParseLayerName(string folderName)
        {
            int underscoreIdx = folderName.IndexOf('_');
            return underscoreIdx > 0
                ? folderName.Substring(underscoreIdx + 1)
                : folderName;
        }
    }
}
#endif
