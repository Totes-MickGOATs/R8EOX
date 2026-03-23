using TMPro;
using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewMenuThemeConfig", menuName = "R8EOX/MenuThemeConfig")]
    public class MenuThemeConfig : ScriptableObject
    {
        [Header("Colors")]
        [SerializeField] private Color background = new Color(0.051f, 0.055f, 0.071f, 1f);
        [SerializeField] private Color panelSurface = new Color(0.078f, 0.082f, 0.102f, 1f);
        [SerializeField] private Color primaryAccent = new Color(0f, 0.784f, 1f, 1f);
        [SerializeField] private Color hoverAccent = new Color(0.91f, 0.722f, 0.286f, 1f);
        [SerializeField] private Color danger = new Color(1f, 0.318f, 0.329f, 1f);
        [SerializeField] private Color textPrimary = Color.white;
        [SerializeField] private Color textSecondary = new Color(0.533f, 0.533f, 0.533f, 1f);

        [Header("Fonts")]
        [Tooltip("Rajdhani Bold — wired in the project after import")]
        [SerializeField] private TMP_FontAsset titleFont;
        [Tooltip("Rajdhani SemiBold")]
        [SerializeField] private TMP_FontAsset bodyFont;
        [Tooltip("Source Code Pro")]
        [SerializeField] private TMP_FontAsset monoFont;

        [Header("Animation")]
        [SerializeField] private float screenTransitionDuration = 0.3f;
        [SerializeField] private float staggerDelay = 0.05f;
        [SerializeField] private float buttonHoverBrightness = 1.12f;
        [SerializeField] private float pulseMinAlpha = 0.3f;
        [SerializeField] private float pulseDuration = 2f;

        public Color Background => background;
        public Color PanelSurface => panelSurface;
        public Color PrimaryAccent => primaryAccent;
        public Color HoverAccent => hoverAccent;
        public Color Danger => danger;
        public Color TextPrimary => textPrimary;
        public Color TextSecondary => textSecondary;

        public TMP_FontAsset TitleFont => titleFont;
        public TMP_FontAsset BodyFont => bodyFont;
        public TMP_FontAsset MonoFont => monoFont;

        public float ScreenTransitionDuration => screenTransitionDuration;
        public float StaggerDelay => staggerDelay;
        public float ButtonHoverBrightness => buttonHoverBrightness;
        public float PulseMinAlpha => pulseMinAlpha;
        public float PulseDuration => pulseDuration;
    }
}
