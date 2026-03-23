#if UNITY_EDITOR
namespace R8EOX.Editor.Builders
{
    internal static class OptionsLayoutData
    {
        // Panel
        internal const float PanelWidth = 840f;
        internal const float PanelHeight = 620f;
        internal const int PanelPaddingH = 24;
        internal const int PanelPaddingTop = 20;
        internal const int PanelPaddingBottom = 12;

        // Tab bar
        internal const float TabBarHeight = 36f;
        internal const float TabSpacing = 8f;

        // Content
        internal const float ContentSpacing = 8f;
        internal const float SectionSpacing = 16f;

        // Header
        internal const float HeaderHeight = 40f;
        internal const float HeaderFontSize = 40f;

        // Back button
        internal const float BackButtonHeight = 52f;
        internal const float BackButtonWidth = 140f;

        // Colors
        internal static readonly UnityEngine.Color PanelBg = new UnityEngine.Color(0.078f, 0.082f, 0.102f);
        internal static readonly UnityEngine.Color BackdropColor = new UnityEngine.Color(0f, 0f, 0f, 0.8f);
        internal static readonly UnityEngine.Color CyanAccent = new UnityEngine.Color(0f, 0.784f, 1f);
        internal static readonly UnityEngine.Color CyanBorder = new UnityEngine.Color(0f, 0.784f, 1f, 0.15f);
    }
}
#endif
