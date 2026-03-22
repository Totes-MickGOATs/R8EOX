#if UNITY_EDITOR
using System.Collections.Generic;
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
        /// Apply per-layer blend masks as the splatmap. Each non-base
        /// layer can optionally provide a blend-mask.png. Composites
        /// N layers using a top-down remaining-weight algorithm.
        /// </summary>
        internal static void ApplyBlendMaskSplatmap(
            TerrainData terrainData, IReadOnlyList<LayerData> layers)
        {
            int layerCount = layers.Count;
            if (layerCount < 1) return;

            // Load blend mask textures (index 0 = base, never has mask)
            var masks = new Texture2D[layerCount];
            int maskCount = 0;
            for (int i = 1; i < layerCount; i++)
            {
                masks[i] = LoadReadableMask(layers[i].BlendMaskPath);
                if (masks[i] != null) maskCount++;
            }

            int alphaRes = terrainData.alphamapResolution;
            float[,,] splatmap =
                ComputeSplatmap(alphaRes, layerCount, masks);

            terrainData.SetAlphamaps(0, 0, splatmap);
            Debug.Log(
                $"[TrackBuilder] Splatmap applied: {layerCount} layers, " +
                $"{maskCount} blend masks.");
        }

        /// <summary>
        /// Pure computation: build an N-layer splatmap from blend masks.
        /// Processes layers top-down with a remaining-weight algorithm.
        /// </summary>
        internal static float[,,] ComputeSplatmap(
            int alphaRes, int layerCount, Texture2D[] masks)
        {
            var splatmap = new float[alphaRes, alphaRes, layerCount];

            for (int y = 0; y < alphaRes; y++)
            {
                for (int x = 0; x < alphaRes; x++)
                {
                    float u = (float)x / (alphaRes - 1);
                    float v = 1f - (float)y / (alphaRes - 1);

                    float remaining = 1f;
                    for (int i = layerCount - 1; i >= 1; i--)
                    {
                        float maskValue = 0f;
                        if (masks[i] != null)
                        {
                            maskValue = masks[i]
                                .GetPixelBilinear(u, v).grayscale;
                        }

                        float weight = remaining * maskValue;
                        splatmap[y, x, i] = weight;
                        remaining -= weight;
                    }
                    splatmap[y, x, 0] = remaining;
                }
            }

            return splatmap;
        }

        static Texture2D LoadReadableMask(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath)) return null;

            string absolutePath = assetPath.StartsWith("Assets")
                ? Path.Combine(
                    Application.dataPath.Replace("Assets", ""),
                    assetPath)
                : assetPath;

            if (!File.Exists(absolutePath)) return null;

            AssetDatabase.ImportAsset(assetPath);
            var importer =
                AssetImporter.GetAtPath(assetPath) as TextureImporter;
            bool reimport = false;
            if (importer != null && !importer.isReadable)
            {
                importer.isReadable = true;
                reimport = true;
            }
            if (importer != null && importer.sRGBTexture)
            {
                importer.sRGBTexture = false;
                reimport = true;
            }
            if (reimport)
                importer.SaveAndReimport();

            Texture2D tex =
                AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (tex == null)
            {
                tex = new Texture2D(2, 2);
                tex.LoadImage(File.ReadAllBytes(absolutePath));
            }

            return tex;
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
