#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
            soScreen.FindProperty("trackListPanel").objectReferenceValue = tlp;
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

            var soList = new SerializedObject(tlp);
            soList.FindProperty("listContent").objectReferenceValue = listContentGo.GetComponent<Transform>();
            soList.ApplyModifiedProperties();

            var entryTemplate = TrackListEntryBuilder.Build(listAreaGo.transform);
            var soList2 = new SerializedObject(tlp);
            soList2.FindProperty("trackEntryPrefab").objectReferenceValue = entryTemplate;
            soList2.ApplyModifiedProperties();
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
            soScreen.FindProperty("trackPreviewPanel").objectReferenceValue = tpp;
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
            soScreen.FindProperty("startButton").objectReferenceValue = startGo.GetComponent<Button>();
            soScreen.FindProperty("backButton").objectReferenceValue  = backGo.GetComponent<Button>();
            soScreen.ApplyModifiedProperties();
        }

        private static void BuildTrackPreviewContent(Transform p, R8EOX.Menu.Internal.TrackPreviewPanel pp)
        {
            var imgGo = new GameObject("PreviewImage");
            imgGo.transform.SetParent(p, false);
            StretchFill(imgGo.AddComponent<RectTransform>());
            var previewImg = imgGo.AddComponent<Image>();

            var nameGo   = CreateLabel("TrackNameLabel",   string.Empty, p);
            var typeGo   = CreateLabel("TrackTypeLabel",   string.Empty, p);
            var descGo   = CreateLabel("DescriptionLabel", string.Empty, p);
            var statusGo = CreateLabel("StatusLabel",      string.Empty, p);

            var indGo = new GameObject("StatusIndicator");
            indGo.transform.SetParent(p, false);
            indGo.AddComponent<RectTransform>().sizeDelta = new Vector2(16f, 16f);
            var indImg = indGo.AddComponent<Image>();
            indImg.color = Color.green;

            var emptyGo = new GameObject("EmptyState");
            emptyGo.transform.SetParent(p, false);
            StretchFill(emptyGo.AddComponent<RectTransform>());
            var emptyTmp = emptyGo.AddComponent<TextMeshProUGUI>();
            emptyTmp.text = "Select a track";
            emptyTmp.alignment = TextAlignmentOptions.Center;

            var so = new SerializedObject(pp);
            so.FindProperty("previewImage").objectReferenceValue     = previewImg;
            so.FindProperty("trackNameLabel").objectReferenceValue   = nameGo.GetComponent<TextMeshProUGUI>();
            so.FindProperty("trackTypeLabel").objectReferenceValue   = typeGo.GetComponent<TextMeshProUGUI>();
            so.FindProperty("descriptionLabel").objectReferenceValue = descGo.GetComponent<TextMeshProUGUI>();
            so.FindProperty("statusLabel").objectReferenceValue      = statusGo.GetComponent<TextMeshProUGUI>();
            so.FindProperty("statusIndicator").objectReferenceValue  = indImg;
            so.FindProperty("emptyState").objectReferenceValue       = emptyGo;
            so.ApplyModifiedProperties();
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
            StretchFill(textGo.AddComponent<RectTransform>());
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 24f;
            return go;
        }

        private static GameObject CreateLabel(string name, string text, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            StretchFill(go.AddComponent<RectTransform>());
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 24f;
            return go;
        }

        private static void StretchFill(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
#endif
