#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using BRH = R8EOX.Editor.BuilderRectHelper;

namespace R8EOX.Editor.Builders
{
    internal static class TrackSelectPanelBuilder
    {
        private static readonly Color PanelColor = new Color(0.078f, 0.082f, 0.102f);

        internal static void Build(Transform p, R8EOX.Menu.Internal.TrackSelectScreen s)
        {
            BuildTitle(p);
            BuildListArea(p, s, out var tlp);
            BuildPreviewArea(p, s);
            BuildButtons(p, s);

            var soScreen = new SerializedObject(s);
            SerializedPropertyHelper.SetRef(soScreen, "trackListPanel", tlp);
            soScreen.ApplyModifiedProperties();
        }

        private static void BuildTitle(Transform p)
        {
            var go = new GameObject("TitleLabel");
            go.transform.SetParent(p, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.88f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "SELECT TRACK";
            tmp.fontSize = 36f;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
        }

        private static void BuildListArea(Transform p, R8EOX.Menu.Internal.TrackSelectScreen s,
            out R8EOX.Menu.Internal.TrackListPanel tlp)
        {
            var listAreaGo = new GameObject("TrackListArea");
            listAreaGo.transform.SetParent(p, false);
            var areaRt = listAreaGo.AddComponent<RectTransform>();
            areaRt.anchorMin = new Vector2(0f, 0.15f);
            areaRt.anchorMax = new Vector2(0.4f, 0.85f);
            areaRt.offsetMin = Vector2.zero;
            areaRt.offsetMax = Vector2.zero;
            var bgImg = listAreaGo.AddComponent<Image>();
            bgImg.color = PanelColor;

            tlp = listAreaGo.AddComponent<R8EOX.Menu.Internal.TrackListPanel>();

            // ScrollRect to enable scrolling through the list
            var scrollRect = listAreaGo.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            var listContentGo = new GameObject("ListContent");
            listContentGo.transform.SetParent(listAreaGo.transform, false);
            var contentRt = listContentGo.AddComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0f, 1f);
            contentRt.anchorMax = new Vector2(1f, 1f);
            contentRt.pivot = new Vector2(0.5f, 1f);
            contentRt.offsetMin = Vector2.zero;
            contentRt.offsetMax = Vector2.zero;

            var vlg = listContentGo.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 4f;
            vlg.childControlHeight = false;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;

            var csf = listContentGo.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = contentRt;

            var entryTemplate = TrackListEntryBuilder.Build(listAreaGo.transform);

            var soList = new SerializedObject(tlp);
            SerializedPropertyHelper.SetRef(soList, "listContent", listContentGo.GetComponent<Transform>());
            SerializedPropertyHelper.SetRef(soList, "trackEntryPrefab", entryTemplate);
            soList.ApplyModifiedProperties();
        }

        private static void BuildPreviewArea(Transform p, R8EOX.Menu.Internal.TrackSelectScreen s)
        {
            var previewAreaGo = new GameObject("TrackPreviewArea");
            previewAreaGo.transform.SetParent(p, false);
            var areaRt = previewAreaGo.AddComponent<RectTransform>();
            areaRt.anchorMin = new Vector2(0.42f, 0.15f);
            areaRt.anchorMax = new Vector2(1f, 0.85f);
            areaRt.offsetMin = Vector2.zero;
            areaRt.offsetMax = Vector2.zero;
            var bgImg = previewAreaGo.AddComponent<Image>();
            bgImg.color = PanelColor;

            var tpp = previewAreaGo.AddComponent<R8EOX.Menu.Internal.TrackPreviewPanel>();
            BuildTrackPreviewContent(previewAreaGo.transform, tpp);

            var soScreen = new SerializedObject(s);
            SerializedPropertyHelper.SetRef(soScreen, "trackPreviewPanel", tpp);
            soScreen.ApplyModifiedProperties();
        }

        private static void BuildButtons(Transform p, R8EOX.Menu.Internal.TrackSelectScreen s)
        {
            var startGo = CreateButton("StartButton", "START", p);
            var backGo  = CreateButton("BackButton_Track", "BACK", p);

            var startRt = startGo.GetComponent<RectTransform>();
            startRt.anchorMin = new Vector2(0.6f, 0.05f);
            startRt.anchorMax = new Vector2(0.6f, 0.05f);
            startRt.pivot = new Vector2(0.5f, 0.5f);
            startRt.sizeDelta = new Vector2(200f, 50f);
            startRt.anchoredPosition = Vector2.zero;

            var backRt = backGo.GetComponent<RectTransform>();
            backRt.anchorMin = new Vector2(0.3f, 0.05f);
            backRt.anchorMax = new Vector2(0.3f, 0.05f);
            backRt.pivot = new Vector2(0.5f, 0.5f);
            backRt.sizeDelta = new Vector2(200f, 50f);
            backRt.anchoredPosition = Vector2.zero;

            var soScreen = new SerializedObject(s);
            SerializedPropertyHelper.SetRef(soScreen, "startButton", startGo.GetComponent<Button>());
            SerializedPropertyHelper.SetRef(soScreen, "backButton",  backGo.GetComponent<Button>());
            soScreen.ApplyModifiedProperties();
        }

        private static void BuildTrackPreviewContent(Transform p, R8EOX.Menu.Internal.TrackPreviewPanel pp)
        {
            var mutedGray = new Color(0.533f, 0.533f, 0.533f);

            var imgGo = new GameObject("PreviewImage");
            imgGo.transform.SetParent(p, false);
            BRH.SetAnchors(imgGo.AddComponent<RectTransform>(), 0.05f, 0.6f, 0.95f, 0.98f);
            var previewImg = imgGo.AddComponent<Image>();

            var nameGo = new GameObject("TrackNameLabel");
            nameGo.transform.SetParent(p, false);
            BRH.SetAnchors(nameGo.AddComponent<RectTransform>(), 0.05f, 0.48f, 0.95f, 0.58f);
            var nameTmp = nameGo.AddComponent<TextMeshProUGUI>();
            nameTmp.fontSize  = 28f;
            nameTmp.alignment = TextAlignmentOptions.Left;
            nameTmp.color     = Color.white;

            var typeGo = new GameObject("TrackTypeLabel");
            typeGo.transform.SetParent(p, false);
            BRH.SetAnchors(typeGo.AddComponent<RectTransform>(), 0.05f, 0.40f, 0.95f, 0.48f);
            var typeTmp = typeGo.AddComponent<TextMeshProUGUI>();
            typeTmp.fontSize  = 16f;
            typeTmp.alignment = TextAlignmentOptions.Left;
            typeTmp.color     = mutedGray;

            var descGo = new GameObject("DescriptionLabel");
            descGo.transform.SetParent(p, false);
            BRH.SetAnchors(descGo.AddComponent<RectTransform>(), 0.05f, 0.15f, 0.95f, 0.40f);
            var descTmp = descGo.AddComponent<TextMeshProUGUI>();
            descTmp.fontSize         = 16f;
            descTmp.alignment        = TextAlignmentOptions.TopLeft;
            descTmp.color            = mutedGray;
            descTmp.textWrappingMode = TextWrappingModes.Normal;

            var statusGo = new GameObject("StatusLabel");
            statusGo.transform.SetParent(p, false);
            BRH.SetAnchors(statusGo.AddComponent<RectTransform>(), 0.10f, 0.05f, 0.95f, 0.13f);
            var statusTmp = statusGo.AddComponent<TextMeshProUGUI>();
            statusTmp.fontSize  = 14f;
            statusTmp.alignment = TextAlignmentOptions.Left;
            statusTmp.color     = Color.white;

            var indGo = new GameObject("StatusIndicator");
            indGo.transform.SetParent(p, false);
            BRH.SetAnchors(indGo.AddComponent<RectTransform>(), 0.05f, 0.06f, 0.09f, 0.12f);
            var indImg = indGo.AddComponent<Image>();
            indImg.color = Color.green;

            var emptyGo = new GameObject("EmptyState");
            emptyGo.transform.SetParent(p, false);
            BRH.StretchFill(emptyGo.AddComponent<RectTransform>());
            var emptyTmp = emptyGo.AddComponent<TextMeshProUGUI>();
            emptyTmp.text = "Select a track";
            emptyTmp.alignment = TextAlignmentOptions.Center;

            var so = new SerializedObject(pp);
            SerializedPropertyHelper.SetRef(so, "previewImage",     previewImg);
            SerializedPropertyHelper.SetRef(so, "trackNameLabel",   nameTmp);
            SerializedPropertyHelper.SetRef(so, "trackTypeLabel",   typeTmp);
            SerializedPropertyHelper.SetRef(so, "descriptionLabel", descTmp);
            SerializedPropertyHelper.SetRef(so, "statusLabel",      statusTmp);
            SerializedPropertyHelper.SetRef(so, "statusIndicator",  indImg);
            SerializedPropertyHelper.SetRef(so, "emptyState",       emptyGo);
            so.ApplyModifiedProperties();
        }

        private static void SetAnchors(RectTransform rt, float minX, float minY, float maxX, float maxY)
        {
            rt.anchorMin = new Vector2(minX, minY);
            rt.anchorMax = new Vector2(maxX, maxY);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static GameObject CreateButton(string name, string label, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            go.AddComponent<Image>().color = PanelColor;
            go.AddComponent<Button>();
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            BRH.StretchFill(textGo.AddComponent<RectTransform>());
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 24f;
            return go;
        }
    }
}
#endif
