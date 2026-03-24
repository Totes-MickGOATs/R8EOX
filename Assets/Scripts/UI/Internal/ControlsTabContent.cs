using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ControlsSettingsData = R8EOX.Settings.Internal.ControlsSettings;

namespace R8EOX.UI.Internal
{
    internal class ControlsTabContent : MonoBehaviour
    {
        private Settings.SettingsManager settingsManager;

        private const float STEP_DEADZONE       = 0.005f;
        private const float STEP_CURVE          = 0.05f;
        private const float BINDINGS_AREA_HEIGHT = 180f;

        // ── Public Entry Point ────────────────────────────────────────────

        internal void Initialize(Settings.SettingsManager settings)
        {
            settingsManager = settings;
            BuildUI();
        }

        // ── Build ─────────────────────────────────────────────────────────

        private void BuildUI()
        {
            OptionsUIFactory.SetupTabLayout(gameObject);

            BuildProfileSection();
            BuildInputSection();
            BuildBindingsSection();
            BuildResetButton();
        }

        // ── Controller Profile ────────────────────────────────────────────

        private void BuildProfileSection()
        {
            OptionsUIFactory.CreateSectionHeader(transform, "Controller Profile");

            string[] names   = settingsManager.GetProfileNames();
            string   current = settingsManager.ActiveProfileName;
            int      index   = System.Array.IndexOf(names, current);
            if (index < 0) index = 0;

            OptionsUIFactory.CreateDropdownRow(
                transform, "Profile", names, index,
                i => settingsManager.SwitchProfile(names[i]));

            var buttonRow = CreateHRow("ProfileButtons");
            OptionsUIFactory.CreateActionButton(
                buttonRow.transform, "Save As New",
                OptionsUIFactory.STYLE_SECONDARY,
                () => settingsManager.CreateProfile(
                    $"Profile {settingsManager.GetProfileNames().Length + 1}"));
            OptionsUIFactory.CreateActionButton(
                buttonRow.transform, "Delete",
                OptionsUIFactory.STYLE_DANGER,
                OnDeleteProfile);
        }

        private void OnDeleteProfile()
        {
            string name = settingsManager.ActiveProfileName;
            ConfirmDialog.Show(
                "Delete Profile",
                $"Delete \"{name}\"? This cannot be undone.",
                "DELETE",
                isDanger: true,
                onConfirm: () => settingsManager.DeleteProfile(name));
        }

        // ── Input Settings ────────────────────────────────────────────────

        private void BuildInputSection()
        {
            OptionsUIFactory.CreateSectionHeader(transform, "Input Settings");

            ControlsSettingsData controls = settingsManager.GetControlsSettings();

            OptionsUIFactory.CreateSliderRow(
                transform, "Steer Deadzone",
                0f, 0.3f, STEP_DEADZONE,
                controls.SteerDeadzone,
                v => settingsManager.SetControlsSettings(c => c.SteerDeadzone = v));

            OptionsUIFactory.CreateSliderRow(
                transform, "Throttle DZ",
                0f, 0.3f, STEP_DEADZONE,
                controls.ThrottleDeadzone,
                v => settingsManager.SetControlsSettings(c => c.ThrottleDeadzone = v));

            OptionsUIFactory.CreateSliderRow(
                transform, "Curve Exponent",
                1f, 3f, STEP_CURVE,
                controls.CurveExponent,
                v => settingsManager.SetControlsSettings(c => c.CurveExponent = v));
        }

        // ── Key / Button Bindings ─────────────────────────────────────────

        private void BuildBindingsSection()
        {
            OptionsUIFactory.CreateSectionHeader(transform, "Key / Button Bindings");

            // Scrollable placeholder for future RebindEntry rows
            var scrollGO = new GameObject("BindingsScroll", typeof(RectTransform));
            scrollGO.transform.SetParent(transform, false);

            var scrollLE = scrollGO.AddComponent<LayoutElement>();
            scrollLE.preferredHeight = BINDINGS_AREA_HEIGHT;
            scrollLE.minHeight       = BINDINGS_AREA_HEIGHT;

            var scrollImg = scrollGO.AddComponent<Image>();
            scrollImg.color = new Color(0f, 0f, 0f, 0.25f);

            var scrollRect = scrollGO.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical   = true;

            var viewportGO = new GameObject("Viewport", typeof(RectTransform));
            viewportGO.transform.SetParent(scrollGO.transform, false);
            viewportGO.AddComponent<RectMask2D>();
            var vpRT = viewportGO.GetComponent<RectTransform>();
            vpRT.anchorMin = Vector2.zero;
            vpRT.anchorMax = Vector2.one;
            vpRT.offsetMin = Vector2.zero;
            vpRT.offsetMax = Vector2.zero;
            scrollRect.viewport = vpRT;

            var contentGO = new GameObject("Content", typeof(RectTransform));
            contentGO.transform.SetParent(viewportGO.transform, false);
            var contentRT = contentGO.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0f, 1f);
            contentRT.anchorMax = new Vector2(1f, 1f);
            contentRT.pivot     = new Vector2(0.5f, 1f);
            contentRT.sizeDelta = new Vector2(0f, 0f);
            var contentCSF = contentGO.AddComponent<ContentSizeFitter>();
            contentCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var contentVG = contentGO.AddComponent<VerticalLayoutGroup>();
            contentVG.childForceExpandWidth  = true;
            contentVG.childForceExpandHeight = false;
            contentVG.padding = new RectOffset(12, 12, 8, 8);
            contentVG.spacing = 4f;
            scrollRect.content = contentRT;

            // Placeholder label — replaced when RebindEntry rows are wired in
            var placeholderGO = new GameObject("Placeholder", typeof(RectTransform));
            placeholderGO.transform.SetParent(contentGO.transform, false);
            var lbl = placeholderGO.AddComponent<TextMeshProUGUI>();
            lbl.text      = "Rebinding coming soon";
            lbl.fontSize  = 16f;
            lbl.color     = UIColors.MutedText;
            lbl.alignment = TextAlignmentOptions.Center;
            var lblLE = placeholderGO.AddComponent<LayoutElement>();
            lblLE.preferredHeight = 44f;
        }

        // ── Reset Defaults ────────────────────────────────────────────────

        private void BuildResetButton()
        {
            OptionsUIFactory.CreateActionButton(
                transform, "Reset to Defaults",
                OptionsUIFactory.STYLE_DANGER,
                () => ConfirmDialog.Show(
                    "Reset Controls",
                    "Reset all input settings to defaults?",
                    "RESET",
                    isDanger: true,
                    onConfirm: ApplyDefaults));
        }

        private void ApplyDefaults()
        {
            ControlsSettingsData defaults = ControlsSettingsData.CreateDefault();
            settingsManager.SetControlsSettings(c =>
            {
                c.SteerDeadzone    = defaults.SteerDeadzone;
                c.ThrottleDeadzone = defaults.ThrottleDeadzone;
                c.CurveExponent    = defaults.CurveExponent;
            });
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private GameObject CreateHRow(string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(transform, false);
            var hg = go.AddComponent<HorizontalLayoutGroup>();
            hg.childForceExpandWidth  = true;
            hg.childForceExpandHeight = false;
            hg.spacing = 8f;
            go.AddComponent<LayoutElement>().minHeight = 36f;
            return go;
        }
    }
}
