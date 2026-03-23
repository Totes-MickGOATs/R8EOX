#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

namespace R8EOX.Editor.Builders
{
    /// <summary>TerrainData, heightmap, GO creation, and material setup.</summary>
    internal static class TerrainBuilder
    {
        // -- Default terrain settings (must match TerrainSettings defaults) --
        const float k_DefaultWidth = 100f;
        const float k_DefaultHeight = 2f;
        const float k_DefaultLength = 100f;
        const int k_DefaultHeightmapRes = 2049;
        const int k_DefaultAlphamapRes = 2048;
        const int k_DefaultDetailRes = 1024;
        const int k_DefaultBaseMapRes = 1024;

        /// <summary>
        /// Load or create TerrainData using convention-based paths from a track folder.
        /// Asset path = {generatedFolder}/{trackName}TerrainData.asset
        /// </summary>
        internal static TerrainData LoadOrCreateTerrainData(
            string generatedFolder, string trackName, TerrainSettings settings)
        {
            string terrainDataAsset =
                $"{generatedFolder}/{trackName}TerrainData.asset";

            int heightmapRes = settings != null
                ? settings.HeightmapResolution : k_DefaultHeightmapRes;
            float terrainWidth = settings != null
                ? settings.TerrainWidth : k_DefaultWidth;
            float terrainHeight = settings != null
                ? settings.TerrainHeight : k_DefaultHeight;
            float terrainLength = settings != null
                ? settings.TerrainLength : k_DefaultLength;
            int alphamapRes = settings != null
                ? settings.AlphamapResolution : k_DefaultAlphamapRes;
            int detailRes = settings != null
                ? settings.DetailResolution : k_DefaultDetailRes;
            int baseMapRes = settings != null
                ? settings.BaseMapResolution : k_DefaultBaseMapRes;

            return LoadOrCreateTerrainData(
                terrainDataAsset, heightmapRes,
                terrainWidth, terrainHeight, terrainLength,
                alphamapRes, detailRes, baseMapRes);
        }

        /// <summary>
        /// Load or create TerrainData with explicit parameters (legacy API).
        /// </summary>
        internal static TerrainData LoadOrCreateTerrainData(
            string terrainDataAsset, int heightmapRes, float terrainWidth,
            float terrainHeight, float terrainLength, int alphamapRes,
            int detailRes, int baseMapRes)
        {
            var existing =
                AssetDatabase.LoadAssetAtPath<TerrainData>(terrainDataAsset);
            if (existing != null)
            {
                existing.heightmapResolution = heightmapRes;
                existing.size = new Vector3(
                    terrainWidth, terrainHeight, terrainLength);
                existing.alphamapResolution = alphamapRes;
                existing.SetDetailResolution(detailRes, 16);
                existing.baseMapResolution = baseMapRes;
                return existing;
            }

            var data = new TerrainData();
            data.heightmapResolution = heightmapRes;
            data.size = new Vector3(
                terrainWidth, terrainHeight, terrainLength);
            data.alphamapResolution = alphamapRes;
            data.SetDetailResolution(detailRes, 16);
            data.baseMapResolution = baseMapRes;
            AssetDatabase.CreateAsset(data, terrainDataAsset);
            return data;
        }

        /// <summary>
        /// Import a 16-bit RAW heightmap from the given file path.
        /// </summary>
        internal static void ImportHeightmap(
            TerrainData terrainData, string heightmapRawPath, int heightmapRes)
        {
            string absolutePath = heightmapRawPath.StartsWith("Assets")
                ? Path.Combine(
                    Application.dataPath.Replace("Assets", ""),
                    heightmapRawPath)
                : heightmapRawPath;

            if (!File.Exists(absolutePath))
            {
                Debug.LogError(
                    $"[TrackBuilder] Heightmap not found: {absolutePath}");
                return;
            }

            byte[] rawBytes = File.ReadAllBytes(absolutePath);
            float[,] heights =
                ParseRawHeightmap(rawBytes, heightmapRes);
            if (heights == null)
            {
                Debug.LogError(
                    $"[TrackBuilder] Heightmap size mismatch: " +
                    $"{rawBytes.Length} vs {heightmapRes * heightmapRes * 2}");
                return;
            }

            terrainData.SetHeights(0, 0, heights);
            Debug.Log("[TrackBuilder] Heightmap imported successfully.");
        }

        /// <summary>
        /// Parse a 16-bit little-endian RAW heightmap into a float[,] array.
        /// Returns null if the byte array size doesn't match the resolution.
        /// </summary>
        internal static float[,] ParseRawHeightmap(
            byte[] rawBytes, int resolution)
        {
            int expectedSize = resolution * resolution * 2;
            if (rawBytes == null || rawBytes.Length != expectedSize)
                return null;

            var heights = new float[resolution, resolution];
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int idx = (y * resolution + x) * 2;
                    ushort raw =
                        (ushort)(rawBytes[idx] | (rawBytes[idx + 1] << 8));
                    heights[y, x] = raw / 65535f;
                }
            }

            return heights;
        }

        /// <summary>
        /// Create or replace the terrain GameObject, centered at origin.
        /// </summary>
        internal static GameObject CreateTerrainGameObject(
            TerrainData terrainData, float terrainWidth, float terrainLength)
        {
            string goName =
                terrainData.name.Replace("TerrainData", "Terrain");
            if (string.IsNullOrEmpty(goName) || goName == "Terrain")
                goName = "Terrain";

            var existing = GameObject.Find(goName);
            if (existing != null) Object.DestroyImmediate(existing);

            GameObject terrainGO =
                Terrain.CreateTerrainGameObject(terrainData);
            terrainGO.name = goName;
            terrainGO.isStatic = true;
            terrainGO.transform.position = new Vector3(
                -terrainWidth * 0.5f, 0f, -terrainLength * 0.5f);

            return terrainGO;
        }

        /// <summary>
        /// Configure terrain material using convention-based paths.
        /// Material path = {generatedFolder}/TerrainMaterial.mat
        /// </summary>
        internal static void ConfigureTerrain(
            GameObject terrainGO, string generatedFolder, string trackName,
            TerrainSettings settings = null)
        {
            string materialPath = $"{generatedFolder}/TerrainMaterial.mat";
            ConfigureTerrainInternal(terrainGO, materialPath, settings);
        }

        static void ConfigureTerrainInternal(
            GameObject terrainGO, string terrainMaterialPath,
            TerrainSettings settings = null)
        {
            var terrain = terrainGO.GetComponent<Terrain>();
            Shader terrainShader =
                Shader.Find("Universal Render Pipeline/Terrain/Lit");

            if (terrainShader == null)
            {
                Debug.LogWarning(
                    "[TrackBuilder] URP Terrain/Lit shader not found. " +
                    "Ensure URP package is installed.");
            }
            else
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(
                    terrainMaterialPath);
                if (mat != null)
                {
                    mat.shader = terrainShader;
                    EditorUtility.SetDirty(mat);
                }
                else
                {
                    mat = new Material(terrainShader);
                    AssetHelper.SaveOrReplaceAsset(mat, terrainMaterialPath);
                }

                terrain.materialTemplate = mat;
                EditorUtility.SetDirty(terrain);
            }

            terrain.heightmapPixelError = settings != null
                ? settings.HeightmapPixelError : 5f;
            terrain.basemapDistance = settings != null
                ? settings.BasemapDistance : 1000f;
            terrain.drawInstanced = settings == null || settings.DrawInstanced;

            var collider = terrainGO.GetComponent<TerrainCollider>();
            if (collider != null)
                collider.terrainData = terrain.terrainData;
        }
    }
}
#endif
