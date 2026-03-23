#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using LD = R8EOX.Editor.Builders.VehicleSelectLayoutData;

namespace R8EOX.Editor.Builders
{
    internal static class OverlayScrollViewBuilder
    {
        internal static void Apply(Transform listPanelT)
        {
            var svT = listPanelT.Find("ScrollView");
            if (svT == null) { Debug.LogWarning("[OverlayScrollViewBuilder] ScrollView not found."); return; }
            var scrollViewRT = svT.GetComponent<RectTransform>();
            LD.SetRectStretch(scrollViewRT);
            scrollViewRT.offsetMin = new Vector2(5, 5);
            scrollViewRT.offsetMax = new Vector2(-5, -5);
            ApplyViewport(svT);
            ApplyScrollbarVertical(svT);
        }

        private static void ApplyViewport(Transform svT)
        {
            var vpT = svT.Find("Viewport");
            if (vpT == null) return;
            LD.SetRectStretch(vpT.GetComponent<RectTransform>());
            var vpImg = vpT.GetComponent<Image>();
            if (vpImg != null) vpImg.color = Color.clear;
            ApplyContent(vpT);
        }

        private static void ApplyContent(Transform vpT)
        {
            var contentT = vpT.Find("Content");
            if (contentT == null) return;
            VehicleSelectLayoutBuilder.SetRect(contentT.GetComponent<RectTransform>(),
                new Vector2(0, 1), new Vector2(1, 1), Vector2.zero, Vector2.zero, new Vector2(0.5f, 1));
            var vlg = contentT.GetComponent<VerticalLayoutGroup>();
            if (vlg != null) { vlg.spacing = 4; vlg.padding = new RectOffset(4, 4, 4, 4); }
            var csf = contentT.GetComponent<ContentSizeFitter>();
            if (csf != null) csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        private static void ApplyScrollbarVertical(Transform svT)
        {
            var sbT = svT.Find("Scrollbar Vertical");
            if (sbT == null) return;
            VehicleSelectLayoutBuilder.SetRect(sbT.GetComponent<RectTransform>(),
                new Vector2(1, 0), new Vector2(1, 1), new Vector2(-12, 0), Vector2.zero, new Vector2(1, 0.5f));
            var sbImg = sbT.GetComponent<Image>();
            if (sbImg != null) sbImg.color = LD.PanelDark;
            var handleT = sbT.Find("Sliding Area/Handle");
            if (handleT == null) return;
            var hImg = handleT.GetComponent<Image>();
            if (hImg != null) hImg.color = LD.PanelMedium;
        }
    }
}
#endif
