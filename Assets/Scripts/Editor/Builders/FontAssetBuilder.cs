#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace R8EOX.Editor.Builders
{
    internal static class FontAssetBuilder
    {
        private const int SamplingPointSize = 48;
        private const int AtlasPadding = 5;
        private const int AtlasWidth = 512;
        private const int AtlasHeight = 512;

        [MenuItem("R8EOX/Build Font Assets")]
        private static void Build()
        {
            CreateFontAsset(
                "Assets/Fonts/Rajdhani-Bold.ttf",
                "Assets/Fonts/Rajdhani-Bold SDF.asset");

            CreateFontAsset(
                "Assets/Fonts/Rajdhani-SemiBold.ttf",
                "Assets/Fonts/Rajdhani-SemiBold SDF.asset");

            CreateFontAsset(
                "Assets/Fonts/Rajdhani-Regular.ttf",
                "Assets/Fonts/Rajdhani-Regular SDF.asset");

            CreateFontAsset(
                "Assets/Fonts/SourceCodePro-Regular.ttf",
                "Assets/Fonts/SourceCodePro-Regular SDF.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            WireMenuThemeConfig();

            Debug.Log("[FontAssetBuilder] Done — font assets created and wired into MenuThemeConfig.");
        }

        private static void CreateFontAsset(string ttfPath, string outputPath)
        {
            var font = AssetDatabase.LoadAssetAtPath<Font>(ttfPath);
            if (font == null)
            {
                Debug.LogError($"[FontAssetBuilder] TTF not found at {ttfPath}");
                return;
            }

            var existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(outputPath);
            if (existing != null)
            {
                Debug.Log($"[FontAssetBuilder] Already exists, skipping: {outputPath}");
                return;
            }

            var fontAsset = TMP_FontAsset.CreateFontAsset(
                font,
                SamplingPointSize,
                AtlasPadding,
                GlyphRenderMode.SDFAA,
                AtlasWidth,
                AtlasHeight,
                AtlasPopulationMode.Dynamic,
                enableMultiAtlasSupport: true);

            if (fontAsset == null)
            {
                Debug.LogError($"[FontAssetBuilder] CreateFontAsset returned null for {ttfPath}");
                return;
            }

            AssetDatabase.CreateAsset(fontAsset, outputPath);
            Debug.Log($"[FontAssetBuilder] Created {outputPath}");
        }

        private static void WireMenuThemeConfig()
        {
            var guids = AssetDatabase.FindAssets("t:MenuThemeConfig");
            if (guids.Length == 0)
            {
                Debug.LogWarning("[FontAssetBuilder] No MenuThemeConfig asset found — create one via R8EOX/MenuThemeConfig.");
                return;
            }

            var configPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            var config = AssetDatabase.LoadAssetAtPath<MenuThemeConfig>(configPath);
            if (config == null)
            {
                Debug.LogError($"[FontAssetBuilder] Failed to load MenuThemeConfig at {configPath}");
                return;
            }

            var so = new SerializedObject(config);

            var bold    = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Rajdhani-Bold SDF.asset");
            var semibold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Rajdhani-SemiBold SDF.asset");
            var mono    = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/SourceCodePro-Regular SDF.asset");

            SetFontProperty(so, "titleFont", bold,     "Rajdhani-Bold SDF");
            SetFontProperty(so, "bodyFont",  semibold, "Rajdhani-SemiBold SDF");
            SetFontProperty(so, "monoFont",  mono,     "SourceCodePro-Regular SDF");

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();

            Debug.Log($"[FontAssetBuilder] MenuThemeConfig wired at {configPath}");
        }

        private static void SetFontProperty(
            SerializedObject so,
            string propertyName,
            TMP_FontAsset fontAsset,
            string displayName)
        {
            if (fontAsset == null)
            {
                Debug.LogWarning($"[FontAssetBuilder] {displayName} not found — skipping {propertyName}.");
                return;
            }

            var prop = so.FindProperty(propertyName);
            if (prop == null)
            {
                Debug.LogError($"[FontAssetBuilder] SerializedProperty '{propertyName}' not found on MenuThemeConfig.");
                return;
            }

            prop.objectReferenceValue = fontAsset;
        }
    }
}
#endif
