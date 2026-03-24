#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using LD = R8EOX.Editor.Builders.LoadingLayoutData;

namespace R8EOX.Editor.Builders
{
    internal static class LoadingPanelBuilder
    {
        internal static void Build(
            Transform parent,
            out Image progressFill,
            out TextMeshProUGUI progressLabel,
            out TextMeshProUGUI tipLabel)
        {
            var titleFont = LoadFont(LD.TitleFontPath);
            var monoFont  = LoadFont(LD.MonoFontPath);
            var bodyFont  = LoadFont(LD.BodyFontPath);

            CreateBackground(parent);

            var content = CreateContentArea(parent);

            CreateTitleGlow(content, titleFont);
            CreateTitle(content, titleFont);

            progressFill  = CreateProgressBar(content);
            progressLabel = CreateProgressLabel(content, monoFont);
            tipLabel      = CreateTipLabel(content, bodyFont);
        }

        private static void CreateBackground(Transform parent)
        {
            var go = new GameObject("Background");
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            SetRect(rt,
                new Vector2(0f, 0f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(0.5f, 0.5f));
            go.AddComponent<Image>().color = LD.BackgroundColor;
        }

        private static RectTransform CreateContentArea(Transform parent)
        {
            var go = new GameObject("ContentArea");
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            SetRect(rt,
                new Vector2(0.5f, 0.35f), new Vector2(0.5f, 0.55f),
                new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(0.5f, 0.5f));
            rt.sizeDelta = new Vector2(LD.ContentWidth, 0f);
            return rt;
        }

        private static void CreateTitleGlow(Transform content, TMP_FontAsset font)
        {
            var go = new GameObject("TitleGlow");
            go.transform.SetParent(content, false);
            var rt = go.AddComponent<RectTransform>();
            SetRect(rt,
                new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, -LD.TitleHeight), new Vector2(0f, 0f),
                new Vector2(0.5f, 1f));

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "LOADING";
            tmp.fontSize = LD.TitleFontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.characterSpacing = LD.TitleCharSpacing;
            AssignFont(tmp, font);
            var glowColor = LD.TitleColor;
            glowColor.a = 0.2f;
            tmp.color = glowColor;

            go.transform.localScale = new Vector3(1.03f, 1.03f, 1f);
        }

        private static void CreateTitle(Transform content, TMP_FontAsset font)
        {
            var go = new GameObject("TitleLabel");
            go.transform.SetParent(content, false);
            var rt = go.AddComponent<RectTransform>();
            SetRect(rt,
                new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, -LD.TitleHeight), new Vector2(0f, 0f),
                new Vector2(0.5f, 1f));

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "LOADING";
            tmp.fontSize = LD.TitleFontSize;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.characterSpacing = LD.TitleCharSpacing;
            AssignFont(tmp, font);
            tmp.color = LD.TitleColor;
        }

        private static Image CreateProgressBar(Transform content)
        {
            var trackGo = new GameObject("BarTrack");
            trackGo.transform.SetParent(content, false);
            var trackRt = trackGo.AddComponent<RectTransform>();
            float barBottom = -(LD.BarTopOffset + LD.BarHeight);
            SetRect(trackRt,
                new Vector2(0.05f, 1f), new Vector2(0.95f, 1f),
                new Vector2(0f, barBottom), new Vector2(0f, -LD.BarTopOffset),
                new Vector2(0.5f, 1f));
            trackGo.AddComponent<Image>().color = LD.BarTrackColor;

            var fillGo = new GameObject("ProgressFill");
            fillGo.transform.SetParent(trackGo.transform, false);
            var fillRt = fillGo.AddComponent<RectTransform>();
            SetRect(fillRt,
                new Vector2(0f, 0f), new Vector2(1f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(0.5f, 0.5f));
            var fillImg = fillGo.AddComponent<Image>();
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillAmount = 0f;
            fillImg.color = LD.BarFillColor;
            return fillImg;
        }

        private static TextMeshProUGUI CreateProgressLabel(Transform content, TMP_FontAsset font)
        {
            var go = new GameObject("ProgressLabel");
            go.transform.SetParent(content, false);
            var rt = go.AddComponent<RectTransform>();
            SetRect(rt,
                new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, -130f), new Vector2(0f, -106f),
                new Vector2(0.5f, 1f));

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "Loading... 0%";
            tmp.fontSize = LD.LabelFontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            AssignFont(tmp, font);
            tmp.color = LD.MutedTextColor;
            return tmp;
        }

        private static TextMeshProUGUI CreateTipLabel(Transform content, TMP_FontAsset font)
        {
            var go = new GameObject("TipLabel");
            go.transform.SetParent(content, false);
            var rt = go.AddComponent<RectTransform>();
            SetRect(rt,
                new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, -190f), new Vector2(0f, -138f),
                new Vector2(0.5f, 1f));

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = string.Empty;
            tmp.fontSize = LD.TipFontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            tmp.fontStyle = FontStyles.Italic;
            AssignFont(tmp, font);
            tmp.color = LD.MutedTextColor;
            return tmp;
        }

        private static TMP_FontAsset LoadFont(string path)
        {
            var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (font == null)
                Debug.LogWarning($"[LoadingPanelBuilder] Font not found at: {path}");
            return font;
        }

        private static void AssignFont(TextMeshProUGUI tmp, TMP_FontAsset font)
        {
            if (font == null) return;
            EnsureAtlas(font);
            var so = new SerializedObject(tmp);
            so.FindProperty("m_fontAsset").objectReferenceValue = font;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureAtlas(TMP_FontAsset font)
        {
            var so = new SerializedObject(font);
            var prop = so.FindProperty("m_AtlasTextures");
            if (prop == null || prop.arraySize > 0) return;
            var tex = new Texture2D(1, 1, TextureFormat.Alpha8, false) { name = font.name + " Atlas" };
            prop.arraySize = 1;
            prop.GetArrayElementAtIndex(0).objectReferenceValue = tex;
            var widthProp = so.FindProperty("m_AtlasWidth");
            var heightProp = so.FindProperty("m_AtlasHeight");
            if (widthProp != null) widthProp.intValue = 1;
            if (heightProp != null) heightProp.intValue = 1;
            so.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.AddObjectToAsset(tex, font);
            AssetDatabase.SaveAssetIfDirty(font);
        }

        internal static void SetRect(
            RectTransform rt,
            Vector2 aMin, Vector2 aMax,
            Vector2 oMin, Vector2 oMax,
            Vector2 pivot)
        {
            rt.anchorMin  = aMin;
            rt.anchorMax  = aMax;
            rt.pivot      = pivot;
            rt.offsetMin  = oMin;
            rt.offsetMax  = oMax;
        }
    }
}
#endif
