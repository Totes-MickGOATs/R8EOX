#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Configures Unity TerrainLayer assets with PBR textures (diffuse, normal, ARM).
    /// Remaps ARM (AO=R, Roughness=G, Metallic=B) to URP MOHS
    /// (Metallic=R, Occlusion=G, Height=B, Smoothness=A) at build time.
    /// </summary>
    internal static class TerrainLayerBuilder
    {
        // -- Defaults matching LayerSettings ScriptableObject defaults --
        const float k_DefaultTileSize = 25f;
        const float k_DefaultMetallic = 0f;
        const float k_DefaultSmoothness = 0f;
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

            layer.name = $"TerrainLayer_{layerData.Index}_{layerData.Name}";
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

            // ARM map → remap to URP MOHS mask map
            if (!string.IsNullOrEmpty(layerData.ArmPath))
            {
                Texture2D maskMap = RemapArmToMohs(
                    layerData.ArmPath, generatedFolder,
                    layerData.Index, layerData.Name);
                if (maskMap != null)
                    layer.maskMapTexture = maskMap;
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

        /// <summary>
        /// Remap ARM (AO=R, Roughness=G, Metallic=B) to URP terrain
        /// mask map MOHS (Metallic=R, Occlusion=G, Height=B, Smoothness=A).
        /// Saves the remapped texture to the Generated folder.
        /// </summary>
        static Texture2D RemapArmToMohs(
            string armAssetPath, string generatedFolder,
            int layerIndex, string layerName)
        {
            // Ensure ARM source is readable and linear
            var armImporter =
                AssetImporter.GetAtPath(armAssetPath) as TextureImporter;
            bool reimport = false;
            if (armImporter != null && armImporter.sRGBTexture)
            {
                armImporter.sRGBTexture = false;
                reimport = true;
            }
            if (armImporter != null && !armImporter.isReadable)
            {
                armImporter.isReadable = true;
                reimport = true;
            }
            if (reimport)
                armImporter.SaveAndReimport();

            Texture2D arm =
                AssetDatabase.LoadAssetAtPath<Texture2D>(armAssetPath);
            if (arm == null) return null;

            int w = arm.width;
            int h = arm.height;
            Color[] armPixels = arm.GetPixels();
            var mohsPixels = new Color[armPixels.Length];

            for (int i = 0; i < armPixels.Length; i++)
                mohsPixels[i] = RemapArmPixelToMohs(armPixels[i]);

            var mohs = new Texture2D(w, h, TextureFormat.RGBA32, false);
            mohs.SetPixels(mohsPixels);
            mohs.Apply();

            string maskPath = $"{generatedFolder}/" +
                $"MaskMap_{layerIndex}_{layerName}.png";
            string absPath = Path.GetDirectoryName(Application.dataPath)
                + "/" + maskPath;
            File.WriteAllBytes(absPath, mohs.EncodeToPNG());
            Object.DestroyImmediate(mohs);

            AssetDatabase.ImportAsset(maskPath);
            var maskImporter =
                AssetImporter.GetAtPath(maskPath) as TextureImporter;
            if (maskImporter != null)
            {
                bool needsReimport = false;
                if (maskImporter.sRGBTexture)
                {
                    maskImporter.sRGBTexture = false;
                    needsReimport = true;
                }
                if (!maskImporter.isReadable)
                {
                    maskImporter.isReadable = true;
                    needsReimport = true;
                }
                if (needsReimport)
                    maskImporter.SaveAndReimport();
            }

            Debug.Log($"[TrackBuilder] ARM→MOHS remap: {maskPath}");
            return AssetDatabase.LoadAssetAtPath<Texture2D>(maskPath);
        }

        /// <summary>
        /// Remap a single ARM pixel to URP MOHS channel layout.
        /// ARM: R=AO, G=Roughness, B=Metallic
        /// MOHS: R=Metallic, G=Occlusion, B=Height, A=Smoothness
        /// </summary>
        internal static Color RemapArmPixelToMohs(Color arm)
        {
            return new Color(
                arm.b,          // R = Metallic  (ARM.B)
                arm.r,          // G = Occlusion (ARM.R = AO)
                0.5f,           // B = Height    (default)
                1f - arm.g);    // A = Smoothness (1 - Roughness)
        }
    }
}
#endif
