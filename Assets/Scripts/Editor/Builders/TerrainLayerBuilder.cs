#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Configures Unity TerrainLayer assets with PBR textures (diffuse, normal, ARM).
    /// Reads layer data from scanned folder structure via LayerData objects.
    /// </summary>
    internal static class TerrainLayerBuilder
    {
        // -- Defaults matching LayerSettings ScriptableObject defaults --
        const float k_DefaultTileSize = 25f;
        const float k_DefaultMetallic = 0f;
        const float k_DefaultSmoothness = 0.3f;
        const float k_DefaultNormalScale = 1.0f;

        /// <summary>
        /// Configure terrain layers from scanned LayerData list.
        /// Each layer's textures come from paths discovered in the track folder.
        /// TerrainLayer assets are saved to generatedFolder.
        /// </summary>
        internal static void ConfigureTerrainLayers(
            TerrainData terrainData,
            IReadOnlyList<LayerData> layers,
            string generatedFolder)
        {
            if (layers == null || layers.Count == 0)
            {
                Debug.LogWarning(
                    "[TrackBuilder] No layers found — skipping layer setup.");
                return;
            }

            var terrainLayers = new TerrainLayer[layers.Count];
            for (int i = 0; i < layers.Count; i++)
            {
                terrainLayers[i] = LoadOrConfigureTerrainLayer(
                    layers[i], generatedFolder);
            }

            terrainData.terrainLayers = terrainLayers;
            Debug.Log(
                $"[TrackBuilder] {layers.Count} terrain layer(s) configured.");
        }

        /// <summary>
        /// Load or create a single TerrainLayer asset from a LayerData descriptor.
        /// Asset path = {generatedFolder}/TerrainLayer_{index}_{name}.asset
        /// </summary>
        internal static TerrainLayer LoadOrConfigureTerrainLayer(
            LayerData layerData, string generatedFolder)
        {
            string layerPath =
                $"{generatedFolder}/TerrainLayer_{layerData.Index}_{layerData.Name}.asset";

            var layer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerPath);
            bool isNew = layer == null;
            if (isNew) layer = new TerrainLayer();

            LayerSettings settings = layerData.LayerSettingsAsset;
            float tileSize = settings != null ? settings.TileSize : k_DefaultTileSize;
            float metallic = settings != null ? settings.Metallic : k_DefaultMetallic;
            float smoothness = settings != null ? settings.Smoothness : k_DefaultSmoothness;
            float normalScale = settings != null ? settings.NormalScale : k_DefaultNormalScale;

            layer.name = layerData.Name;
            layer.tileSize = new Vector2(tileSize, tileSize);
            layer.tileOffset = Vector2.zero;

            // Diffuse
            if (!string.IsNullOrEmpty(layerData.DiffusePath))
            {
                Texture2D diffuse =
                    AssetDatabase.LoadAssetAtPath<Texture2D>(layerData.DiffusePath);
                if (diffuse != null)
                    layer.diffuseTexture = diffuse;
                else
                    Debug.LogWarning(
                        $"[TrackBuilder] Missing diffuse: {layerData.DiffusePath}");
            }

            // Normal map
            if (!string.IsNullOrEmpty(layerData.NormalPath))
            {
                var normalImporter =
                    AssetImporter.GetAtPath(layerData.NormalPath) as TextureImporter;
                if (normalImporter != null &&
                    normalImporter.textureType != TextureImporterType.NormalMap)
                {
                    normalImporter.textureType = TextureImporterType.NormalMap;
                    normalImporter.SaveAndReimport();
                }

                Texture2D normal =
                    AssetDatabase.LoadAssetAtPath<Texture2D>(layerData.NormalPath);
                if (normal != null)
                    layer.normalMapTexture = normal;
                else
                    Debug.LogWarning(
                        $"[TrackBuilder] Missing normal: {layerData.NormalPath}");
            }

            // ARM map (AO=R, Roughness=G, Metallic=B) — import as linear
            if (!string.IsNullOrEmpty(layerData.ArmPath))
            {
                var armImporter =
                    AssetImporter.GetAtPath(layerData.ArmPath) as TextureImporter;
                if (armImporter != null && armImporter.sRGBTexture)
                {
                    armImporter.sRGBTexture = false;
                    armImporter.SaveAndReimport();
                }

                Texture2D arm =
                    AssetDatabase.LoadAssetAtPath<Texture2D>(layerData.ArmPath);
                if (arm != null)
                    layer.maskMapTexture = arm;
            }

            layer.metallic = metallic;
            layer.smoothness = smoothness;
            layer.normalScale = normalScale;

            if (isNew)
                AssetDatabase.CreateAsset(layer, layerPath);
            else
                EditorUtility.SetDirty(layer);

            return layer;
        }
    }
}
#endif
