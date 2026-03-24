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
            ApplyDimBackdrop(root);
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

        private static void ApplyDimBackdrop(GameObject root)
        {
            var t = root.transform.Find("DimBackdrop");
            if (t == null)
            {
                var go = new GameObject("DimBackdrop", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(root.transform, false);
                go.transform.SetAsFirstSibling();
                t = go.transform;
            }
            LD.SetRectStretch(t.GetComponent<RectTransform>());
            t.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
        }

        private static void ApplyBackground(GameObject root)
        {
            var t = root.transform.Find("Background");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] Background not found."); return; }
            var rt = t.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(40f, 40f);
            rt.offsetMax = new Vector2(-40f, -40f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            var img = t.GetComponent<Image>();
            if (img != null) img.color = LD.Background;
        }

        private static void ApplyTitle(GameObject root)
        {
            var t = root.transform.Find("Title");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] Title not found."); return; }
            SetRect(t.GetComponent<RectTransform>(), new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(60, -120), new Vector2(-60, -40), new Vector2(0.5f, 1));
            var tmp = t.GetComponent<TextMeshProUGUI>();
            if (tmp == null) return;
            tmp.fontSize = 42;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = LD.Gold;
            tmp.fontStyle = FontStyles.Bold;
        }

        private static void ApplySearchField(GameObject root)
        {
            var t = root.transform.Find("ListPanel/SearchField");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] SearchField not found."); return; }
            SetRect(t.GetComponent<RectTransform>(), new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(5, -40), new Vector2(-5, 0), new Vector2(0, 1));
            var img = t.GetComponent<Image>();
            if (img != null) img.color = LD.PanelMedium;
        }

        private static void ApplyCategoryDropdown(GameObject root)
        {
            var t = root.transform.Find("ListPanel/CategoryDropdown");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] CategoryDropdown not found."); return; }
            SetRect(t.GetComponent<RectTransform>(), new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(5, -80), new Vector2(-5, -42), new Vector2(0, 1));
            var img = t.GetComponent<Image>();
            if (img != null) img.color = LD.PanelMedium;
        }

        private static void ApplyListPanel(GameObject root)
        {
            var t = root.transform.Find("ListPanel");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] ListPanel not found."); return; }
            var go = t.gameObject;
            var rt = LD.EnsureRectTransform(go);
            SetRect(rt, new Vector2(0, 0), new Vector2(0.32f, 1),
                new Vector2(55, 125), new Vector2(-10, -125), new Vector2(0, 0));
            var img = go.GetComponent<Image>();
            if (img == null) img = go.AddComponent<Image>();
            img.color = LD.PanelDark;
            OverlayScrollViewBuilder.Apply(rt.transform);
        }

        private static void ApplyPreviewPanel(GameObject root)
        {
            var t = root.transform.Find("PreviewPanel");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] PreviewPanel not found."); return; }
            var go = t.gameObject;
            var rt = LD.EnsureRectTransform(go);
            SetRect(rt, new Vector2(0.34f, 0), new Vector2(1, 1),
                new Vector2(10, 125), new Vector2(-55, -125), new Vector2(0, 0));
            var img = go.GetComponent<Image>();
            if (img == null) img = go.AddComponent<Image>();
            img.color = LD.PanelDark;
            OverlayPreviewPanelBuilder.Apply(rt.transform);
        }

        private static void ApplyConfirmButton(GameObject root)
        {
            var t = root.transform.Find("ConfirmButton");
            if (t == null) { Debug.LogWarning("[VehicleSelectLayoutBuilder] ConfirmButton not found."); return; }
            SetRect(t.GetComponent<RectTransform>(), new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(-260, 55), new Vector2(-55, 105), new Vector2(1, 0));
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
                new Vector2(55, 55), new Vector2(260, 105), new Vector2(0, 0));
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
