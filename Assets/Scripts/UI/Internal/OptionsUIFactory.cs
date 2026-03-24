using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.UI.Internal
{
    internal static class OptionsUIFactory
    {
        internal const int STYLE_PRIMARY   = 0;
        internal const int STYLE_SECONDARY = 1;
        internal const int STYLE_DANGER    = 2;

        private const float LABEL_WIDTH         = 160f;
        private const float VALUE_WIDTH         = 52f;
        private const float SLIDER_HEIGHT       = 6f;
        private const float ROW_HEIGHT          = 36f;
        private const float SECTION_SPACING     = 16f;
        private const float SECTION_RULE_HEIGHT = 1f;

        internal static VerticalLayoutGroup SetupTabLayout(
            GameObject go, float spacing = 8f,
            RectOffset padding = null, bool addFitter = false)
        {
            var vlg = go.GetComponent<VerticalLayoutGroup>();
            if (vlg == null) vlg = go.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = spacing;
            vlg.padding = padding ?? new RectOffset(0, 0, 0, 0);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            if (addFitter)
            {
                var fitter = go.GetComponent<ContentSizeFitter>();
                if (fitter == null) fitter = go.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            return vlg;
        }

        internal static GameObject CreateSectionHeader(Transform parent, string text)
        {
            var root = NewGO("Section_" + text, parent);
            var vg = root.AddComponent<VerticalLayoutGroup>();
            vg.childForceExpandWidth = true; vg.childForceExpandHeight = false;
            vg.spacing = 4f; vg.padding = new RectOffset(0, 0, (int)SECTION_SPACING, 12);
            root.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var lbl = NewGO("Label", root.transform).AddComponent<TextMeshProUGUI>();
            lbl.text = text.ToUpperInvariant(); lbl.fontSize = 16f; lbl.color = UIColors.Primary;
            SetH(lbl.gameObject, 20f);

            var rule = NewGO("Rule", root.transform).AddComponent<Image>();
            rule.color = UIColors.BorderMedium;
            SetH(rule.gameObject, SECTION_RULE_HEIGHT);
            return root;
        }

        internal static Slider CreateSliderRow(
            Transform parent, string label,
            float min, float max, float step, float value, Action<float> onChange)
        {
            var row = MakeHRow("SliderRow_" + label, parent);
            AddLabel(row.transform, label, LABEL_WIDTH, 20f, FontStyles.Bold, Color.white);

            var sliderGO = NewGO("Slider", row.transform);
            sliderGO.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var slider = BuildSlider(sliderGO, min, max, value);

            var valGO = NewGO("Value", row.transform);
            var valElem = valGO.AddComponent<LayoutElement>();
            valElem.minWidth = VALUE_WIDTH; valElem.preferredWidth = VALUE_WIDTH;
            var valTmp = valGO.AddComponent<TextMeshProUGUI>();
            valTmp.text = value.ToString("F2"); valTmp.fontSize = 16f;
            valTmp.color = UIColors.Warning; valTmp.alignment = TextAlignmentOptions.Right;

            slider.onValueChanged.AddListener(v =>
            {
                float s = step > 0f ? Mathf.Round(v / step) * step : v;
                valTmp.text = s.ToString("F2");
                onChange?.Invoke(s);
            });
            return slider;
        }

        internal static TMP_Dropdown CreateDropdownRow(
            Transform parent, string label,
            string[] options, int selectedIndex, Action<int> onChange)
        {
            var row = MakeHRow("DropdownRow_" + label, parent);
            AddLabel(row.transform, label, LABEL_WIDTH, 20f, FontStyles.Bold, Color.white);

            var ddGO = NewGO("Dropdown", row.transform);
            ddGO.AddComponent<LayoutElement>().flexibleWidth = 1f;
            ddGO.AddComponent<Image>().color = UIColors.SliderTrack;

            var dd = ddGO.AddComponent<TMP_Dropdown>();
            dd.ClearOptions();
            foreach (var opt in options) dd.options.Add(new TMP_Dropdown.OptionData(opt));
            dd.value = Mathf.Clamp(selectedIndex, 0, options.Length - 1);

            var cap = NewGO("Label", ddGO.transform).AddComponent<TextMeshProUGUI>();
            cap.fontSize = 18f; cap.color = Color.white;
            dd.captionText = cap;

            dd.onValueChanged.AddListener(i => onChange?.Invoke(i));
            return dd;
        }

        internal static Toggle CreateCheckboxRow(
            Transform parent, string label, bool isChecked, Action<bool> onChange)
        {
            var row = MakeHRow("CheckboxRow_" + label, parent);
            var lbl = AddLabel(row.transform, label, 0f, 20f, FontStyles.Bold, Color.white);
            lbl.GetComponent<LayoutElement>().flexibleWidth = 1f;

            var tGO = NewGO("Toggle", row.transform);
            var te = tGO.AddComponent<LayoutElement>();
            te.minWidth = 28f; te.preferredWidth = 28f; te.minHeight = 28f;

            var bg = tGO.AddComponent<Image>(); bg.color = UIColors.SliderTrack;
            var toggle = tGO.AddComponent<Toggle>(); toggle.targetGraphic = bg;

            var checkGO = NewGO("Checkmark", tGO.transform);
            var checkImg = checkGO.AddComponent<Image>(); checkImg.color = UIColors.Primary;
            Fill(checkGO.GetComponent<RectTransform>(), 4f);
            toggle.graphic = checkImg;

            toggle.isOn = isChecked;
            toggle.onValueChanged.AddListener(v => onChange?.Invoke(v));
            return toggle;
        }

        internal static GameObject CreateTierButtons(
            Transform parent, string[] labels, int selectedIndex, Action<int> onChange)
        {
            var row = MakeHRow("TierButtons", parent);
            var buttons = new Button[labels.Length];
            var activeBg   = UIColors.BorderHalf;
            var inactiveBg = new Color(0f, 0f, 0f, 0f);

            for (int i = 0; i < labels.Length; i++)
            {
                int idx = i;
                var bGO = NewGO("Tier_" + labels[i], row.transform);
                bGO.AddComponent<LayoutElement>().flexibleWidth = 1f;
                var bg = bGO.AddComponent<Image>(); bg.color = (i == selectedIndex) ? activeBg : inactiveBg;
                var btn = bGO.AddComponent<Button>(); btn.targetGraphic = bg;

                var txt = NewGO("Text", bGO.transform).AddComponent<TextMeshProUGUI>();
                txt.text = labels[i]; txt.fontSize = 18f;
                txt.color = Color.white; txt.alignment = TextAlignmentOptions.Center;
                Fill(txt.GetComponent<RectTransform>(), 0f);

                btn.onClick.AddListener(() =>
                {
                    onChange?.Invoke(idx);
                    for (int j = 0; j < buttons.Length; j++)
                        if (buttons[j] != null && buttons[j].targetGraphic is Image img)
                            img.color = (j == idx) ? activeBg : inactiveBg;
                });
                buttons[i] = btn;
            }
            return row;
        }

        internal static Button CreateActionButton(
            Transform parent, string text, int style, Action onClick)
        {
            var bGO = NewGO("Button_" + text, parent);
            bGO.AddComponent<LayoutElement>().minHeight = ROW_HEIGHT;

            Color bgCol, txtCol;
            if      (style == STYLE_DANGER)    { bgCol = new Color(1f, 0.318f, 0.329f, 0.15f); txtCol = UIColors.Danger; }
            else if (style == STYLE_SECONDARY) { bgCol = UIColors.SliderTrack;                  txtCol = UIColors.MutedText; }
            else                               { bgCol = new Color(0f, 0f, 0f, 0f);             txtCol = Color.white; }

            var bg = bGO.AddComponent<Image>(); bg.color = bgCol;
            var btn = bGO.AddComponent<Button>(); btn.targetGraphic = bg;

            var tGO = NewGO("Text", bGO.transform);
            var tmp = tGO.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = 18f; tmp.color = txtCol;
            tmp.alignment = TextAlignmentOptions.Center;
            Fill(tGO.GetComponent<RectTransform>(), 0f);

            btn.onClick.AddListener(() => onClick?.Invoke());
            return btn;
        }

        private static GameObject MakeHRow(string name, Transform parent)
        {
            var go = NewGO(name, parent);
            var hg = go.AddComponent<HorizontalLayoutGroup>();
            hg.childAlignment = TextAnchor.MiddleLeft;
            hg.childForceExpandWidth = false; hg.childForceExpandHeight = true;
            hg.spacing = 12f;
            go.AddComponent<LayoutElement>().minHeight = ROW_HEIGHT;
            return go;
        }

        private static GameObject AddLabel(
            Transform parent, string text, float width, float fontSize, FontStyles style, Color color)
        {
            var go = NewGO("Label", parent);
            var le = go.AddComponent<LayoutElement>();
            if (width > 0f) { le.minWidth = width; le.preferredWidth = width; }
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = fontSize; tmp.fontStyle = style;
            tmp.color = color; tmp.alignment = TextAlignmentOptions.Left;
            return go;
        }

        private static Slider BuildSlider(GameObject c, float min, float max, float value)
        {
            var s = c.AddComponent<Slider>();
            s.minValue = min; s.maxValue = max; s.value = value;
            s.direction = Slider.Direction.LeftToRight;

            var bg = NewGO("Background", c.transform).AddComponent<Image>();
            bg.color = UIColors.SliderTrack; SetH(bg.gameObject, SLIDER_HEIGHT);
            Fill(bg.GetComponent<RectTransform>(), 0f); s.targetGraphic = bg;

            var fillArea = NewGO("Fill Area", c.transform);
            Fill(fillArea.GetComponent<RectTransform>(), 0f);
            var fill = NewGO("Fill", fillArea.transform).AddComponent<Image>();
            fill.color = UIColors.Primary; Fill(fill.GetComponent<RectTransform>(), 0f);
            s.fillRect = fill.GetComponent<RectTransform>();

            var hArea = NewGO("Handle Slide Area", c.transform);
            Fill(hArea.GetComponent<RectTransform>(), 0f);
            var handle = NewGO("Handle", hArea.transform).AddComponent<Image>();
            handle.color = Color.white;
            handle.GetComponent<RectTransform>().sizeDelta = new Vector2(14f, 14f);
            s.handleRect = handle.GetComponent<RectTransform>();
            return s;
        }

        private static GameObject NewGO(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void SetH(GameObject go, float h)
        {
            var le = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            le.preferredHeight = h; le.minHeight = h;
        }

        private static void Fill(RectTransform rt, float inset)
        {
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(inset, inset); rt.offsetMax = new Vector2(-inset, -inset);
        }
    }
}
