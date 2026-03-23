using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.UI.Internal
{
    /// <summary>
    /// Draws a gamepad stick state using Unity UI elements.
    /// Attach to a RectTransform sized 140x140px.
    /// Call Initialize() once, then UpdateInput() each frame.
    /// </summary>
    internal sealed class StickVisualizer : MonoBehaviour
    {
        // ── Layout constants ──────────────────────────────────────────────
        private const float PanelSize      = 140f;
        private const float CrosshairThick = 1f;
        private const float DotDiameter    = 10f;

        // ── Child Image references ────────────────────────────────────────
        private Image _background;
        private Image _crosshairH;
        private Image _crosshairV;
        private Image _deadzone;
        private Image _dot;

        // ── Runtime state ─────────────────────────────────────────────────
        private float _deadzoneRadius;
        private RectTransform _dotRect;

        // ── Color palette ─────────────────────────────────────────────────
        private static readonly Color BackgroundColor = new Color(77f / 255f, 77f / 255f, 77f / 255f, 1f);
        private static readonly Color CrosshairColor  = new Color(51f / 255f, 51f / 255f, 51f / 255f, 0.6f);
        private static readonly Color DeadzoneColor   = new Color(51f / 255f, 51f / 255f, 51f / 255f, 0.4f);
        private static readonly Color DotColor        = new Color(0f, 200f / 255f, 1f, 1f);

        // ── Public API ────────────────────────────────────────────────────

        /// <summary>
        /// Creates all child visuals. Must be called once before UpdateInput.
        /// </summary>
        internal void Initialize(string label, float deadzoneRadius)
        {
            _deadzoneRadius = Mathf.Clamp01(deadzoneRadius);

            BuildBackground();
            BuildCrosshair();
            BuildDeadzone();
            BuildDot();
            ApplyDeadzoneSize();
        }

        /// <summary>
        /// Moves the position dot based on calibratedPosition (x/y each in -1..1).
        /// The dot is clamped within the panel bounds minus half the dot size.
        /// </summary>
        internal void UpdateInput(Vector2 rawPosition, Vector2 calibratedPosition)
        {
            if (_dotRect == null)
                return;

            float halfPanel  = PanelSize * 0.5f;
            float halfDot    = DotDiameter * 0.5f;
            float clampRange = halfPanel - halfDot;

            float x = Mathf.Clamp(calibratedPosition.x * halfPanel, -clampRange, clampRange);
            float y = Mathf.Clamp(calibratedPosition.y * halfPanel, -clampRange, clampRange);

            _dotRect.anchoredPosition = new Vector2(x, y);
        }

        /// <summary>Updates the deadzone indicator circle to reflect a new radius (0–1).</summary>
        internal void SetDeadzoneRadius(float radius)
        {
            _deadzoneRadius = Mathf.Clamp01(radius);
            ApplyDeadzoneSize();
        }

        // ── Builder helpers ───────────────────────────────────────────────

        private void BuildBackground()
        {
            var go = CreateChild("Background");
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            _background       = go.AddComponent<Image>();
            _background.color = BackgroundColor;
        }

        private void BuildCrosshair()
        {
            var goH = CreateChild("CrosshairH");
            var rtH = goH.GetComponent<RectTransform>();
            SetStrip(rtH, horizontal: true);
            _crosshairH       = goH.AddComponent<Image>();
            _crosshairH.color = CrosshairColor;

            var goV = CreateChild("CrosshairV");
            var rtV = goV.GetComponent<RectTransform>();
            SetStrip(rtV, horizontal: false);
            _crosshairV       = goV.AddComponent<Image>();
            _crosshairV.color = CrosshairColor;
        }

        private void BuildDeadzone()
        {
            var go = CreateChild("Deadzone");
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot     = new Vector2(0.5f, 0.5f);

            _deadzone       = go.AddComponent<Image>();
            _deadzone.color = DeadzoneColor;
        }

        private void BuildDot()
        {
            var go = CreateChild("Dot");
            _dotRect = go.GetComponent<RectTransform>();
            _dotRect.anchorMin        = new Vector2(0.5f, 0.5f);
            _dotRect.anchorMax        = new Vector2(0.5f, 0.5f);
            _dotRect.pivot            = new Vector2(0.5f, 0.5f);
            _dotRect.sizeDelta        = new Vector2(DotDiameter, DotDiameter);
            _dotRect.anchoredPosition = Vector2.zero;

            _dot       = go.AddComponent<Image>();
            _dot.color = DotColor;
        }

        // ── Utility ───────────────────────────────────────────────────────

        private void ApplyDeadzoneSize()
        {
            if (_deadzone == null)
                return;

            float diameter = _deadzoneRadius * PanelSize;
            _deadzone.GetComponent<RectTransform>().sizeDelta = new Vector2(diameter, diameter);
        }

        private GameObject CreateChild(string childName)
        {
            var go = new GameObject(childName);
            go.AddComponent<RectTransform>();
            go.transform.SetParent(transform, false);
            return go;
        }

        private static void SetStrip(RectTransform rt, bool horizontal)
        {
            if (horizontal)
            {
                rt.anchorMin = new Vector2(0f, 0.5f);
                rt.anchorMax = new Vector2(1f, 0.5f);
                rt.pivot     = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(0f, CrosshairThick);
            }
            else
            {
                rt.anchorMin = new Vector2(0.5f, 0f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot     = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(CrosshairThick, 0f);
            }
        }
    }
}
