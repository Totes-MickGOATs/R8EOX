using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace R8EOX.UI.Internal
{
    internal class OptionsTabBar : MonoBehaviour
    {
        private static readonly string[] TabDisplayNames =
        {
            "VIDEO", "AUDIO", "CONTROLS", "CALIB", "GAMEPLAY", "PROFILE"
        };

        private static readonly Color ActiveColor   = new Color(0f,     0.784f, 1f);
        private static readonly Color InactiveColor = new Color(0.333f, 0.333f, 0.333f);

        private OptionsTab activeTab = OptionsTab.Video;
        private Button[] tabButtons;
        private Image[] tabUnderlines;
        private TextMeshProUGUI[] tabLabelTexts;
        private Action<OptionsTab> onTabChanged;

        private const int TabCount = 6;

        // ------------------------------------------------------------------ //
        // Internal API                                                        //
        // ------------------------------------------------------------------ //

        internal OptionsTab ActiveTab => activeTab;

        internal void Initialize(Action<OptionsTab> tabChangedCallback)
        {
            onTabChanged = tabChangedCallback;
            BuildTabs();
            SetActiveTab(OptionsTab.Video);
        }

        internal void SetActiveTab(OptionsTab tab)
        {
            activeTab = tab;
            RefreshVisuals();
            onTabChanged?.Invoke(activeTab);
        }

        // ------------------------------------------------------------------ //
        // Unity Lifecycle                                                     //
        // ------------------------------------------------------------------ //

        private void Update()
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.qKey.wasPressedThisFrame) StepToPreviousTab();
                if (Keyboard.current.eKey.wasPressedThisFrame) StepToNextTab();
            }

            if (Gamepad.current != null)
            {
                if (Gamepad.current.leftShoulder.wasPressedThisFrame)  StepToPreviousTab();
                if (Gamepad.current.rightShoulder.wasPressedThisFrame) StepToNextTab();
            }
        }

        // ------------------------------------------------------------------ //
        // Tab Cycling                                                         //
        // ------------------------------------------------------------------ //

        private void StepToNextTab()
        {
            int next = ((int)activeTab + 1) % TabCount;
            SetActiveTab((OptionsTab)next);
        }

        private void StepToPreviousTab()
        {
            int prev = ((int)activeTab - 1 + TabCount) % TabCount;
            SetActiveTab((OptionsTab)prev);
        }

        // ------------------------------------------------------------------ //
        // Construction                                                        //
        // ------------------------------------------------------------------ //

        private void BuildTabs()
        {
            tabButtons    = new Button[TabCount];
            tabUnderlines = new Image[TabCount];
            tabLabelTexts = new TextMeshProUGUI[TabCount];

            EnsureHorizontalLayout();

            for (int i = 0; i < TabCount; i++)
            {
                OptionsTab tab = (OptionsTab)i;
                GameObject buttonGo = CreateTabButton(i, tab);
                tabButtons[i] = buttonGo.GetComponent<Button>();
            }
        }

        private void EnsureHorizontalLayout()
        {
            HorizontalLayoutGroup layout = GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            layout.childAlignment         = TextAnchor.MiddleCenter;
            layout.childControlWidth      = true;
            layout.childControlHeight     = true;
            layout.childForceExpandWidth  = true;
            layout.childForceExpandHeight = false;
            layout.spacing                = 0f;
            layout.padding                = new RectOffset(0, 0, 0, 0);
        }

        private GameObject CreateTabButton(int index, OptionsTab tab)
        {
            GameObject go = new GameObject("Tab_" + tab.ToString(), typeof(RectTransform));
            go.transform.SetParent(transform, false);

            // Transparent bg required by Button
            Image bg = go.AddComponent<Image>();
            bg.color = Color.clear;

            Button button = go.AddComponent<Button>();
            button.transition  = Selectable.Transition.None;
            button.targetGraphic = bg;

            // Vertical stack: label on top, underline on bottom
            VerticalLayoutGroup vLayout = go.AddComponent<VerticalLayoutGroup>();
            vLayout.childAlignment         = TextAnchor.LowerCenter;
            vLayout.childControlWidth      = true;
            vLayout.childControlHeight     = true;
            vLayout.childForceExpandWidth  = true;
            vLayout.childForceExpandHeight = false;
            vLayout.spacing                = 4f;
            vLayout.padding                = new RectOffset(8, 8, 8, 8);

            tabLabelTexts[index] = BuildLabel(go.transform, index);
            tabUnderlines[index] = BuildUnderline(go.transform);

            int capturedIndex = index;
            button.onClick.AddListener(() => SetActiveTab((OptionsTab)capturedIndex));

            return go;
        }

        private TextMeshProUGUI BuildLabel(Transform parent, int index)
        {
            GameObject go = new GameObject("Label", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            TextMeshProUGUI label = go.AddComponent<TextMeshProUGUI>();
            label.text      = TabDisplayNames[index];
            label.fontSize  = 14f;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color     = InactiveColor;

            LayoutElement le = go.AddComponent<LayoutElement>();
            le.flexibleWidth   = 1f;
            le.preferredHeight = 20f;

            return label;
        }

        private Image BuildUnderline(Transform parent)
        {
            GameObject go = new GameObject("Underline", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            Image img = go.AddComponent<Image>();
            img.color = InactiveColor;

            LayoutElement le = go.AddComponent<LayoutElement>();
            le.flexibleWidth   = 1f;
            le.minHeight       = 2f;
            le.preferredHeight = 2f;

            return img;
        }

        // ------------------------------------------------------------------ //
        // Visual Refresh                                                      //
        // ------------------------------------------------------------------ //

        private void RefreshVisuals()
        {
            if (tabLabelTexts == null) return;

            for (int i = 0; i < TabCount; i++)
            {
                bool isActive = i == (int)activeTab;
                Color c = isActive ? ActiveColor : InactiveColor;

                if (tabLabelTexts[i]  != null) tabLabelTexts[i].color  = c;
                if (tabUnderlines[i]  != null) tabUnderlines[i].color  = c;
            }
        }
    }
}
