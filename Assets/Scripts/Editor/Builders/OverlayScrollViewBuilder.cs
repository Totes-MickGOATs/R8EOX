#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using LD = R8EOX.Editor.Builders.VehicleSelectLayoutData;
using BRH = R8EOX.Editor.BuilderRectHelper;

namespace R8EOX.Editor.Builders
{
    internal static class OverlayScrollViewBuilder
    {
        internal static void Apply(Transform listPanelT)
        {
            var svT = listPanelT.Find("ScrollView");
            if (svT == null) { Debug.LogWarning("[OverlayScrollViewBuilder] ScrollView not found."); return; }
            var scrollViewRT = svT.GetComponent<RectTransform>();
            BRH.StretchFill(scrollViewRT);
            scrollViewRT.offsetMin = new Vector2(5, 5);
            scrollViewRT.offsetMax = new Vector2(-5, -5);

            var svImg = svT.GetComponent<Image>();
            if (svImg != null) svImg.color = Color.clear;

            ApplyContent(svT);
        }

        private static void ApplyContent(Transform svT)
        {
            var contentT = svT.Find("Content");
            if (contentT == null) return;
            VehicleSelectLayoutBuilder.SetRect(contentT.GetComponent<RectTransform>(),
                new Vector2(0, 1), new Vector2(1, 1), Vector2.zero, Vector2.zero, new Vector2(0.5f, 1));
            var vlg = contentT.GetComponent<VerticalLayoutGroup>();
            if (vlg != null) { vlg.spacing = 4; vlg.padding = new RectOffset(4, 4, 4, 4); }
            var csf = contentT.GetComponent<ContentSizeFitter>();
            if (csf != null) csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}
#endif
