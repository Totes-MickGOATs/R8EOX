#if UNITY_EDITOR
namespace R8EOX.Editor.Builders
{
    internal static class LoadingLayoutData
    {
        // --- Colors ---

        internal static readonly UnityEngine.Color BackgroundColor =
            new UnityEngine.Color(0.051f, 0.055f, 0.071f);

        internal static readonly UnityEngine.Color BarTrackColor =
            new UnityEngine.Color(0.078f, 0.082f, 0.102f);

        internal static readonly UnityEngine.Color BarFillColor =
            new UnityEngine.Color(0f, 0.784f, 1f);

        internal static readonly UnityEngine.Color TitleColor =
            new UnityEngine.Color(0.9f, 0.95f, 1f);

        internal static readonly UnityEngine.Color MutedTextColor =
            new UnityEngine.Color(0.533f, 0.533f, 0.533f);

        // --- Size Constants ---

        internal const float ContentWidth    = 800f;
        internal const float TitleFontSize   = 64f;
        internal const float TitleHeight     = 70f;
        internal const float BarHeight       = 8f;
        internal const float BarTopOffset    = 94f;
        internal const float LabelFontSize   = 16f;
        internal const float TipFontSize     = 18f;
        internal const float TitleCharSpacing = 12f;

        // --- Font Asset Paths ---

        internal const string TitleFontPath = "Assets/Fonts/Rajdhani-Bold SDF.asset";
        internal const string MonoFontPath  = "Assets/Fonts/SourceCodePro-Regular SDF.asset";
        internal const string BodyFontPath  = "Assets/Fonts/Rajdhani-SemiBold SDF.asset";
    }
}
#endif
