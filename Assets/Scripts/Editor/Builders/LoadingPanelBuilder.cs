#if UNITY_EDITOR
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using LD = R8EOX.Editor.Builders.LoadingLayoutData;
using BRH = R8EOX.Editor.BuilderRectHelper;

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
            CreateBackground(parent);

            var content = CreateContentArea(parent);

            CreateTitleGlow(content);
            CreateTitle(content);

            progressFill  = CreateProgressBar(content);
            progressLabel = CreateProgressLabel(content);
            tipLabel      = CreateTipLabel(content);
        }

        private static void CreateBackground(Transform parent)
        {
            var go = new GameObject("Background");
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            BRH.SetRect(rt,
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
            BRH.SetRect(rt,
                new Vector2(0.5f, 0.35f), new Vector2(0.5f, 0.55f),
                new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(0.5f, 0.5f));
            rt.sizeDelta = new Vector2(LD.ContentWidth, 0f);
            return rt;
        }

        private static void CreateTitleGlow(Transform content)
        {
            var go = new GameObject("TitleGlow");
            go.transform.SetParent(content, false);
            var rt = go.AddComponent<RectTransform>();
            BRH.SetRect(rt,
                new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, -LD.TitleHeight), new Vector2(0f, 0f),
                new Vector2(0.5f, 1f));

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "LOADING";
            tmp.fontSize = LD.TitleFontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.characterSpacing = LD.TitleCharSpacing;
            var glowColor = LD.TitleColor;
            glowColor.a = 0.2f;
            tmp.color = glowColor;
            AssignFont(tmp, LD.TitleFontPath);

            go.transform.localScale = new Vector3(1.03f, 1.03f, 1f);
        }

        private static void CreateTitle(Transform content)
        {
            var go = new GameObject("TitleLabel");
            go.transform.SetParent(content, false);
            var rt = go.AddComponent<RectTransform>();
            BRH.SetRect(rt,
                new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, -LD.TitleHeight), new Vector2(0f, 0f),
                new Vector2(0.5f, 1f));

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "LOADING";
            tmp.fontSize = LD.TitleFontSize;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.characterSpacing = LD.TitleCharSpacing;
            tmp.color = LD.TitleColor;
            AssignFont(tmp, LD.TitleFontPath);
        }

        private static Image CreateProgressBar(Transform content)
        {
            var trackGo = new GameObject("BarTrack");
            trackGo.transform.SetParent(content, false);
            var trackRt = trackGo.AddComponent<RectTransform>();
            float barBottom = -(LD.BarTopOffset + LD.BarHeight);
            BRH.SetRect(trackRt,
                new Vector2(0.05f, 1f), new Vector2(0.95f, 1f),
                new Vector2(0f, barBottom), new Vector2(0f, -LD.BarTopOffset),
                new Vector2(0.5f, 1f));
            trackGo.AddComponent<Image>().color = LD.BarTrackColor;

            var fillGo = new GameObject("ProgressFill");
            fillGo.transform.SetParent(trackGo.transform, false);
            var fillRt = fillGo.AddComponent<RectTransform>();
            BRH.SetRect(fillRt,
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

        private static TextMeshProUGUI CreateProgressLabel(Transform content)
        {
            var go = new GameObject("ProgressLabel");
            go.transform.SetParent(content, false);
            var rt = go.AddComponent<RectTransform>();
            BRH.SetRect(rt,
                new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, -130f), new Vector2(0f, -106f),
                new Vector2(0.5f, 1f));

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "Loading... 0%";
            tmp.fontSize = LD.LabelFontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = LD.MutedTextColor;
            AssignFont(tmp, LD.MonoFontPath);
            return tmp;
        }

        private static TextMeshProUGUI CreateTipLabel(Transform content)
        {
            var go = new GameObject("TipLabel");
            go.transform.SetParent(content, false);
            var rt = go.AddComponent<RectTransform>();
            BRH.SetRect(rt,
                new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, -190f), new Vector2(0f, -138f),
                new Vector2(0.5f, 1f));

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = string.Empty;
            tmp.fontSize = LD.TipFontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            tmp.fontStyle = FontStyles.Italic;
            tmp.color = LD.MutedTextColor;
            AssignFont(tmp, LD.BodyFontPath);
            return tmp;
        }

        private static void AssignFont(TextMeshProUGUI tmp, string fontPath)
        {
            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
            if (fontAsset != null)
                tmp.font = fontAsset;
            else
                Debug.LogWarning($"[LoadingPanelBuilder] Font not found at {fontPath}");
        }

    }
}
#endif
