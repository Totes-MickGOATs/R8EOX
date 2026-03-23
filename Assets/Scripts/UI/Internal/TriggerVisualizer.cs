using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.UI.Internal
{
    /// <summary>
    /// Draws a gamepad trigger as a horizontal fill bar (120x16px RectTransform).
    /// Call Initialize() once, then UpdateValue() each frame.
    /// </summary>
    internal sealed class TriggerVisualizer : MonoBehaviour
    {
        // ── Layout constants ──────────────────────────────────────────────
        private const float TrackWidth    = 120f;
        private const float TrackHeight   = 16f;
        private const float LabelFontSize = 9f;

        // ── Child references ──────────────────────────────────────────────
        private Image _fill;
        private RectTransform _fillRect;
        private TextMeshProUGUI _labelText;
        private TextMeshProUGUI _valueText;

        // ── Color palette ─────────────────────────────────────────────────
        private static readonly Color TrackColor = new Color(38f / 255f, 38f / 255f, 38f / 255f, 1f);
        private static readonly Color FillColor  = new Color(0f, 200f / 255f, 1f, 0.9f);

        // ── Public API ────────────────────────────────────────────────────

        /// <summary>
        /// Builds child visuals and sets the trigger label (e.g. "LT" or "RT").
        /// Must be called once before UpdateValue.
        /// </summary>
        internal void Initialize(string label)
        {
            BuildTrack();
            BuildFill();
            BuildLabelText(label);
            BuildValueText();
        }

        /// <summary>
        /// Updates the fill bar width and percentage label.
        /// calibratedValue should be in the range 0–1.
        /// </summary>
        internal void UpdateValue(float rawValue, float calibratedValue)
        {
            float clamped = Mathf.Clamp01(calibratedValue);

            if (_fillRect != null)
                _fillRect.sizeDelta = new Vector2(TrackWidth * clamped, TrackHeight);

            if (_valueText != null)
                _valueText.text = $"{Mathf.RoundToInt(clamped * 100f):D2}%";
        }

        // ── Builder helpers ───────────────────────────────────────────────

        private void BuildTrack()
        {
            var go = CreateChild("Track");
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var img   = go.AddComponent<Image>();
            img.color = TrackColor;
        }

        private void BuildFill()
        {
            var go = CreateChild("Fill");
            _fillRect = go.GetComponent<RectTransform>();
            _fillRect.anchorMin        = new Vector2(0f, 0f);
            _fillRect.anchorMax        = new Vector2(0f, 1f);
            _fillRect.pivot            = new Vector2(0f, 0.5f);
            _fillRect.anchoredPosition = Vector2.zero;
            _fillRect.sizeDelta        = new Vector2(0f, TrackHeight);

            _fill       = go.AddComponent<Image>();
            _fill.color = FillColor;
        }

        private void BuildLabelText(string label)
        {
            var go = CreateChild("Label");
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0f, 0f);
            rt.anchorMax        = new Vector2(0.3f, 1f);
            rt.offsetMin        = new Vector2(2f, 0f);
            rt.offsetMax        = Vector2.zero;

            _labelText                = go.AddComponent<TextMeshProUGUI>();
            _labelText.text           = label;
            _labelText.fontSize       = LabelFontSize;
            _labelText.alignment      = TextAlignmentOptions.MidlineLeft;
            _labelText.color          = Color.white;
            _labelText.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
        }

        private void BuildValueText()
        {
            var go = CreateChild("Value");
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0.6f, 0f);
            rt.anchorMax        = new Vector2(1f, 1f);
            rt.offsetMin        = Vector2.zero;
            rt.offsetMax        = new Vector2(-2f, 0f);

            _valueText                = go.AddComponent<TextMeshProUGUI>();
            _valueText.text           = "00%";
            _valueText.fontSize       = LabelFontSize;
            _valueText.alignment      = TextAlignmentOptions.MidlineRight;
            _valueText.color          = Color.white;
            _valueText.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
        }

        // ── Utility ───────────────────────────────────────────────────────

        private GameObject CreateChild(string childName)
        {
            var go = new GameObject(childName);
            go.AddComponent<RectTransform>();
            go.transform.SetParent(transform, false);
            return go;
        }
    }
}
