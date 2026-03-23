#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LD = R8EOX.Editor.Builders.VehicleSelectLayoutData;

namespace R8EOX.Editor.Builders
{
    internal static class OverlayPreviewPanelBuilder
    {
        internal static void Apply(Transform previewT)
        {
            ApplyVehicleName(previewT);
            ApplyPreviewImage(previewT);
            ApplyStatsDisplay(previewT);
        }

        private static void ApplyVehicleName(Transform previewT)
        {
            var nameT = previewT.Find("VehicleName");
            if (nameT == null) return;
            VehicleSelectLayoutBuilder.SetRect(nameT.GetComponent<RectTransform>(),
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(15, -45), new Vector2(-15, 0), new Vector2(0.5f, 1));
            var tmp = nameT.GetComponent<TextMeshProUGUI>();
            if (tmp == null) return;
            tmp.fontSize = 32;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = LD.TextWhite;
            tmp.fontStyle = FontStyles.Bold;
        }

        private static void ApplyPreviewImage(Transform previewT)
        {
            var imgT = previewT.Find("PreviewImage");
            if (imgT == null) return;
            VehicleSelectLayoutBuilder.SetRect(imgT.GetComponent<RectTransform>(),
                new Vector2(0, 0.35f), new Vector2(1, 1), new Vector2(10, 10), new Vector2(-10, -50), new Vector2(0.5f, 0.5f));
        }

        private static void ApplyStatsDisplay(Transform previewT)
        {
            var statsT = previewT.Find("StatsDisplay");
            if (statsT == null) return;
            VehicleSelectLayoutBuilder.SetRect(statsT.GetComponent<RectTransform>(),
                new Vector2(0, 0), new Vector2(1, 0.35f), new Vector2(10, 10), new Vector2(-10, -5), new Vector2(0.5f, 0.5f));
            ApplyStatBars(statsT);
            ApplyDescriptionText(statsT);
        }

        private static void ApplyStatBars(Transform statsDisplayT)
        {
            string[] barNames = { "TopSpeedBar", "AccelerationBar", "HandlingBar", "WeightBar" };
            Color[] barColors = { LD.StatGreen, LD.StatOrange, LD.StatBlue, LD.StatRed };
            string[] labelTexts = { "TOP SPEED", "ACCEL", "HANDLING", "WEIGHT" };

            for (int i = 0; i < barNames.Length; i++)
            {
                var barT = statsDisplayT.Find(barNames[i]);
                if (barT == null) { Debug.LogWarning($"[OverlayPreviewPanelBuilder] {barNames[i]} not found."); continue; }
                float yTop = -i * 34f;
                VehicleSelectLayoutBuilder.SetRect(barT.GetComponent<RectTransform>(),
                    new Vector2(0, 1), new Vector2(1, 1),
                    new Vector2(110, yTop - 28), new Vector2(-10, yTop), new Vector2(0.5f, 1));
                var barImage = barT.GetComponent<Image>();
                if (barImage != null) LD.ConfigureStatBar(barImage, barColors[i]);
                CreateStatBarBg(statsDisplayT, barT, barNames[i], yTop);
                CreateStatBarLabel(statsDisplayT, barNames[i], labelTexts[i], yTop);
            }
        }

        private static void CreateStatBarBg(Transform parent, Transform barT, string barName, float yTop)
        {
            var bgGO = new GameObject($"{barName}Bg");
            bgGO.transform.SetParent(parent, false);
            bgGO.transform.SetSiblingIndex(barT.GetSiblingIndex());
            VehicleSelectLayoutBuilder.SetRect(bgGO.AddComponent<RectTransform>(),
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(110, yTop - 28), new Vector2(-10, yTop), new Vector2(0.5f, 1));
            bgGO.AddComponent<Image>().color = LD.StatBarBg;
        }

        private static void CreateStatBarLabel(Transform parent, string barName, string labelText, float yTop)
        {
            var labelGO = new GameObject($"{barName}Label");
            labelGO.transform.SetParent(parent, false);
            VehicleSelectLayoutBuilder.SetRect(labelGO.AddComponent<RectTransform>(),
                new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(10, yTop - 28), new Vector2(105, yTop), new Vector2(0, 1));
            var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
            labelTMP.text = labelText;
            labelTMP.fontSize = 14;
            labelTMP.color = LD.TextGrey;
            labelTMP.alignment = TextAlignmentOptions.MidlineLeft;
            labelTMP.enableAutoSizing = false;
        }

        private static void ApplyDescriptionText(Transform statsDisplayT)
        {
            var descT = statsDisplayT.Find("DescriptionText");
            if (descT == null) return;
            float descY = -4 * 34f;
            VehicleSelectLayoutBuilder.SetRect(descT.GetComponent<RectTransform>(),
                new Vector2(0, 0), new Vector2(1, 1),
                new Vector2(10, 0), new Vector2(-10, descY), new Vector2(0.5f, 0));
            var descTMP = descT.GetComponent<TextMeshProUGUI>();
            if (descTMP == null) return;
            descTMP.fontSize = 14;
            descTMP.color = LD.TextGrey;
            descTMP.alignment = TextAlignmentOptions.TopLeft;
            descTMP.enableWordWrapping = true;
        }
    }
}
#endif
