#if UNITY_EDITOR
using UnityEngine;

namespace R8EOX.Editor.Builders
{
    internal static class VehicleSelectLayoutData
    {
        // Background
        internal static readonly Color Background = new Color(0.05f, 0.05f, 0.08f, 0.95f);
        internal static readonly Color PanelDark = new Color(0.08f, 0.08f, 0.12f, 1f);
        internal static readonly Color PanelMedium = new Color(0.12f, 0.12f, 0.16f, 1f);

        // Accent
        internal static readonly Color Gold = new Color(0.9f, 0.75f, 0.2f, 1f);
        internal static readonly Color ConfirmGreen = new Color(0.15f, 0.65f, 0.3f, 1f);
        internal static readonly Color ConfirmGreenHover = new Color(0.2f, 0.75f, 0.35f, 1f);
        internal static readonly Color BackRed = new Color(0.6f, 0.15f, 0.15f, 1f);
        internal static readonly Color BackRedHover = new Color(0.7f, 0.2f, 0.2f, 1f);

        // Text
        internal static readonly Color TextWhite = new Color(0.9f, 0.9f, 0.9f, 1f);
        internal static readonly Color TextGrey = new Color(0.6f, 0.6f, 0.6f, 1f);

        // Stat Bars
        internal static readonly Color StatGreen = new Color(0.2f, 0.8f, 0.3f, 1f);
        internal static readonly Color StatOrange = new Color(0.9f, 0.6f, 0.15f, 1f);
        internal static readonly Color StatBlue = new Color(0.25f, 0.5f, 0.9f, 1f);
        internal static readonly Color StatRed = new Color(0.85f, 0.25f, 0.2f, 1f);
        internal static readonly Color StatBarBg = new Color(0.15f, 0.15f, 0.18f, 1f);

        // Button
        internal static readonly Color ButtonNormal = new Color(0.15f, 0.15f, 0.2f, 1f);
        internal static readonly Color ButtonHighlight = new Color(0.2f, 0.2f, 0.28f, 1f);
        internal static readonly Color ButtonPressed = new Color(0.1f, 0.1f, 0.15f, 1f);
        internal static readonly Color ButtonDisabled = new Color(0.1f, 0.1f, 0.12f, 0.5f);

        // Entries
        internal static readonly Color EntryNormal = new Color(0.1f, 0.1f, 0.14f, 1f);
        internal static readonly Color EntrySelected = new Color(0.2f, 0.35f, 0.7f, 1f);

        // Configures an Image as a filled horizontal bar (left origin)
        internal static void ConfigureStatBar(UnityEngine.UI.Image image, Color fillColor)
        {
            image.type = UnityEngine.UI.Image.Type.Filled;
            image.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            image.fillOrigin = (int)UnityEngine.UI.Image.OriginHorizontal.Left;
            image.fillAmount = 0.75f;
            image.color = fillColor;
        }

        // Sets RectTransform to stretch-fill (anchor 0,0 -> 1,1, zero offsets)
        internal static void SetRectStretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.pivot = new Vector2(0.5f, 0.5f);
        }

        // Sets RectTransform with specific anchor and offset values
        internal static void SetRectAnchored(
            RectTransform rt,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax,
            Vector2 pivot)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            rt.pivot = pivot;
        }

        // Creates a ColorBlock for Button components
        internal static UnityEngine.UI.ColorBlock MakeColorBlock(
            Color normal,
            Color highlight,
            Color pressed,
            Color disabled)
        {
            var cb = UnityEngine.UI.ColorBlock.defaultColorBlock;
            cb.normalColor = normal;
            cb.highlightedColor = highlight;
            cb.pressedColor = pressed;
            cb.selectedColor = highlight;
            cb.disabledColor = disabled;
            cb.colorMultiplier = 1f;
            cb.fadeDuration = 0.1f;
            return cb;
        }
    }
}
#endif
