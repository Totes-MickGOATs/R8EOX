#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Blend-mask splatmap and macro normal map application for terrain.
    /// Split from TerrainBuilder to stay under the 300-line limit.
    /// </summary>
    internal static class TerrainSplatmapBuilder
    {
        /// <summary>
        /// Apply a blend-mask PNG as the splatmap. Uses pixel.grayscale.
        /// blendMaskPath is the direct asset path to the blend-mask.png.
        /// </summary>
        internal static void ApplyBlendMaskSplatmap(
            TerrainData terrainData, string blendMaskPath)
        {
            if (string.IsNullOrEmpty(blendMaskPath))
            {
                Debug.LogWarning(
                    "[TrackBuilder] No blend mask path -- skipping splatmap.");
                return;
            }

            string absolutePath = blendMaskPath.StartsWith("Assets")
                ? Path.Combine(
                    Application.dataPath.Replace("Assets", ""),
                    blendMaskPath)
                : blendMaskPath;

            if (!File.Exists(absolutePath))
            {
                Debug.LogWarning(
                    "[TrackBuilder] Blend mask not found, using uniform blend.");
                return;
            }

            AssetDatabase.ImportAsset(blendMaskPath);
            var maskImporter =
                AssetImporter.GetAtPath(blendMaskPath) as TextureImporter;
            if (maskImporter != null && !maskImporter.isReadable)
            {
                maskImporter.isReadable = true;
                maskImporter.SaveAndReimport();
            }

            Texture2D maskTex =
                AssetDatabase.LoadAssetAtPath<Texture2D>(blendMaskPath);
            if (maskTex == null)
            {
                maskTex = new Texture2D(2, 2);
                maskTex.LoadImage(File.ReadAllBytes(absolutePath));
            }

            int layerCount = terrainData.terrainLayers != null
                ? terrainData.terrainLayers.Length : 2;
            if (layerCount < 2) layerCount = 2;

            int alphaRes = terrainData.alphamapResolution;
            float[,,] splatmap = new float[alphaRes, alphaRes, layerCount];

            for (int y = 0; y < alphaRes; y++)
            {
                for (int x = 0; x < alphaRes; x++)
                {
                    float u = (float)x / (alphaRes - 1);
                    float v = 1f - (float)y / (alphaRes - 1);
                    Color pixel = maskTex.GetPixelBilinear(u, v);

                    float maskValue = pixel.grayscale;
                    splatmap[y, x, 0] = 1f - maskValue;
                    splatmap[y, x, 1] = maskValue;
                }
            }

            terrainData.SetAlphamaps(0, 0, splatmap);
            Debug.Log("[TrackBuilder] Blend mask splatmap applied.");
        }

        /// <summary>
        /// Apply a macro normal map to the terrain material (Phase 2).
        /// </summary>
        internal static void ApplyMacroNormalMap(
            Terrain terrain, string normalMapPath)
        {
            AssetDatabase.ImportAsset(normalMapPath);
            Texture2D normalTex =
                AssetDatabase.LoadAssetAtPath<Texture2D>(normalMapPath);

            if (normalTex == null)
            {
                Debug.LogWarning(
                    "[TrackBuilder] Normal map not found at " + normalMapPath);
                return;
            }

            var importer =
                AssetImporter.GetAtPath(normalMapPath) as TextureImporter;
            if (importer != null &&
                importer.textureType != TextureImporterType.NormalMap)
            {
                importer.textureType = TextureImporterType.NormalMap;
                importer.SaveAndReimport();
                normalTex =
                    AssetDatabase.LoadAssetAtPath<Texture2D>(normalMapPath);
            }

            Material mat = terrain.materialTemplate;
            if (mat != null && mat.HasProperty("_TerrainNormalmapTexture"))
            {
                mat.SetTexture("_TerrainNormalmapTexture", normalTex);
                mat.EnableKeyword("_NORMALMAP");
                EditorUtility.SetDirty(mat);
                Debug.Log(
                    "[TrackBuilder] Macro normal map assigned to terrain.");
            }
            else
            {
                Debug.LogWarning(
                    "[TrackBuilder] Terrain material missing or lacks " +
                    "_TerrainNormalmapTexture -- normal map not assigned.");
            }
        }
    }
}
#endif
