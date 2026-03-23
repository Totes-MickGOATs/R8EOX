#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using LD = R8EOX.Editor.Builders.VehicleSelectLayoutData;

namespace R8EOX.Editor.Builders
{
    internal static class VehicleSelectLayoutBuilder
    {
        [MenuItem("R8EOX/Build Vehicle Select Layout")]
        private static void BuildLayout()
        {
            BuildOverlayPrefab();
            VehicleListEntryBuilder.BuildEntryPrefab();
            AssetDatabase.SaveAssets();
            Debug.Log("[VehicleSelectLayoutBuilder] Layout applied to VehicleSelectOverlay and VehicleListEntry prefabs.");
        }

        internal static void SetRect(RectTransform rt, Vector2 aMin, Vector2 aMax, Vector2 oMin, Vector2 oMax, Vector2 pivot)
        {
            LD.SetRectAnchored(rt, aMin, aMax, oMin, oMax, pivot);
        }

        // -------------------------------------------------------------------------
        // Overlay prefab
        // -------------------------------------------------------------------------
        private static void BuildOverlayPrefab()
        {
            const string overlayPath = "Assets/Prefabs/UI/VehicleSelectOverlay.prefab";
            var root = PrefabUtility.LoadPrefabContents(overlayPath);

            ApplyCanvas(root);
            ApplyCanvasScaler(root);
            ApplyBackground(root);
            ApplyTitle(root);
            ApplySearchField(root);
            ApplyCategoryDropdown(root);
            ApplyListPanel(root);
            ApplyPreviewPanel(root);
            ApplyConfirmButton(root);
            ApplyBackButton(root);

            PrefabUtility.SaveAsPrefabAsset(root, overlayPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void ApplyCanvas(GameObject root)
        {
            var canvas = root.GetComponent<Canvas>();
            if (canvas == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] Canvas not found on root."); return; }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
        }

        private static void ApplyCanvasScaler(GameObject root)
        {
            var scaler = root.GetComponent<CanvasScaler>();
            if (scaler == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] CanvasScaler not found on root."); return; }
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        }

        private static void ApplyBackground(GameObject root)
        {
            var img = root.GetComponent<Image>();
            if (img == null) img = root.AddComponent<Image>();
            img.color = LD.Background;
        }

        private static void ApplyTitle(GameObject root)
        {
            var t = root.transform.Find("Title");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] Title not found."); return; }
            SetRect(t.GetComponent<RectTransform>(), new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(20, -80), new Vector2(-20, 0), new Vector2(0.5f, 1));
            var tmp = t.GetComponent<TextMeshProUGUI>();
            if (tmp == null) return;
            tmp.fontSize = 42;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = LD.Gold;
            tmp.fontStyle = FontStyles.Bold;
        }

        private static void ApplySearchField(GameObject root)
        {
            var t = root.transform.Find("SearchField");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] SearchField not found."); return; }
            SetRect(t.GetComponent<RectTransform>(), new Vector2(0, 1), new Vector2(0.32f, 1),
                new Vector2(15, -125), new Vector2(-10, -85), new Vector2(0, 1));
            var img = t.GetComponent<Image>();
            if (img != null) img.color = LD.PanelMedium;
            var placeholder = t.Find("Text Area/Placeholder");
            if (placeholder != null) { var ptmp = placeholder.GetComponent<TextMeshProUGUI>(); if (ptmp != null) ptmp.color = LD.TextGrey; }
            var text = t.Find("Text Area/Text");
            if (text != null) { var ttmp = text.GetComponent<TextMeshProUGUI>(); if (ttmp != null) ttmp.color = LD.TextWhite; }
        }

        private static void ApplyCategoryDropdown(GameObject root)
        {
            var t = root.transform.Find("CategoryDropdown");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] CategoryDropdown not found."); return; }
            SetRect(t.GetComponent<RectTransform>(), new Vector2(0, 1), new Vector2(0.32f, 1),
                new Vector2(15, -170), new Vector2(-10, -130), new Vector2(0, 1));
            var img = t.GetComponent<Image>();
            if (img != null) img.color = LD.PanelMedium;
        }

        private static void ApplyListPanel(GameObject root)
        {
            var t = root.transform.Find("ListPanel");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] ListPanel not found."); return; }
            SetRect(t.GetComponent<RectTransform>(), new Vector2(0, 0), new Vector2(0.32f, 1),
                new Vector2(15, 85), new Vector2(-10, -175), new Vector2(0, 0));
            var img = t.GetComponent<Image>();
            if (img != null) img.color = LD.PanelDark;
            OverlayScrollViewBuilder.Apply(t);
        }

        private static void ApplyPreviewPanel(GameObject root)
        {
            var t = root.transform.Find("PreviewPanel");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] PreviewPanel not found."); return; }
            SetRect(t.GetComponent<RectTransform>(), new Vector2(0.34f, 0), new Vector2(1, 1),
                new Vector2(10, 85), new Vector2(-15, -85), new Vector2(0, 0));
            var img = t.GetComponent<Image>();
            if (img != null) img.color = LD.PanelDark;
            OverlayPreviewPanelBuilder.Apply(t);
        }

        private static void ApplyConfirmButton(GameObject root)
        {
            var t = root.transform.Find("ConfirmButton");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] ConfirmButton not found."); return; }
            SetRect(t.GetComponent<RectTransform>(), new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(-220, 15), new Vector2(-15, 65), new Vector2(1, 0));
            var img = t.GetComponent<Image>();
            if (img != null) img.color = LD.ConfirmGreen;
            var btn = t.GetComponent<Button>();
            if (btn != null)
                btn.colors = LD.MakeColorBlock(LD.ConfirmGreen, LD.ConfirmGreenHover, new Color(0.1f, 0.5f, 0.2f), LD.ButtonDisabled);
            var textT = t.Find("ConfirmText");
            if (textT == null) return;
            var tmp = textT.GetComponent<TextMeshProUGUI>();
            if (tmp == null) return;
            tmp.text = "CONFIRM"; tmp.fontSize = 22; tmp.color = LD.TextWhite;
            tmp.alignment = TextAlignmentOptions.Center; tmp.fontStyle = FontStyles.Bold;
        }

        private static void ApplyBackButton(GameObject root)
        {
            var t = root.transform.Find("BackButton");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] BackButton not found."); return; }
            SetRect(t.GetComponent<RectTransform>(), new Vector2(0, 0), new Vector2(0, 0),
                new Vector2(15, 15), new Vector2(220, 65), new Vector2(0, 0));
            var img = t.GetComponent<Image>();
            if (img != null) img.color = LD.BackRed;
            var btn = t.GetComponent<Button>();
            if (btn != null)
                btn.colors = LD.MakeColorBlock(LD.BackRed, LD.BackRedHover, new Color(0.4f, 0.1f, 0.1f), LD.ButtonDisabled);
            var textT = t.Find("BackText");
            if (textT == null) return;
            var tmp = textT.GetComponent<TextMeshProUGUI>();
            if (tmp == null) return;
            tmp.text = "BACK"; tmp.fontSize = 22; tmp.color = LD.TextWhite;
            tmp.alignment = TextAlignmentOptions.Center; tmp.fontStyle = FontStyles.Bold;
        }
    }
}
#endif
