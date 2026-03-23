using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using R8EOX.Settings;

namespace R8EOX.UI.Internal
{
    internal class VideoTabContent : MonoBehaviour
    {
        [Header("Runtime State")]
        [SerializeField] private bool isInitialized;

        private SettingsManager settingsManager;
        private Slider renderScaleSlider;
        private TMP_Dropdown windowModeDropdown;
        private TMP_Dropdown resolutionDropdown;
        private TMP_Dropdown vSyncDropdown;
        private TMP_Dropdown fpsCapDropdown;

        // Cached resolution list built once during BuildUI
        private Resolution[] availableResolutions;

        private static readonly int[] FpsCapValues = { 0, 30, 60, 120, 144, 240 };

        // ── Initialization ────────────────────────────────────────────────

        internal void Initialize(SettingsManager settings)
        {
            if (isInitialized) return;
            settingsManager = settings;
            SetupLayoutGroup();
            BuildUI();
            PopulateFromSettings();
            isInitialized = true;
        }

        private void SetupLayoutGroup()
        {
            var vlg = gameObject.GetComponent<VerticalLayoutGroup>();
            if (vlg == null) vlg = gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 4f;
            vlg.padding = new RectOffset(8, 8, 8, 8);

            var csf = gameObject.GetComponent<ContentSizeFitter>();
            if (csf == null) csf = gameObject.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // ── Build UI ──────────────────────────────────────────────────────

        private void BuildUI()
        {
            var vs = settingsManager.GetVideoSettings();

            // Graphics Quality
            OptionsUIFactory.CreateSectionHeader(transform, "Graphics Quality");
            OptionsUIFactory.CreateTierButtons(transform,
                new[] { "ULTRA", "HIGH", "BALANCED", "PERF" },
                (int)vs.QualityTier, OnTierSelected);

            // Display
            OptionsUIFactory.CreateSectionHeader(transform, "Display");
            windowModeDropdown = OptionsUIFactory.CreateDropdownRow(transform, "Window Mode",
                new[] { "Fullscreen", "Borderless", "Windowed" },
                (int)vs.WindowMode, OnWindowModeChanged);
            resolutionDropdown = OptionsUIFactory.CreateDropdownRow(transform, "Resolution",
                BuildResolutionOptions(), 0, OnResolutionChanged);

            // Sync
            OptionsUIFactory.CreateSectionHeader(transform, "Sync");
            vSyncDropdown = OptionsUIFactory.CreateDropdownRow(transform, "V-Sync",
                new[] { "Off", "On" }, 0, OnVSyncChanged);
            fpsCapDropdown = OptionsUIFactory.CreateDropdownRow(transform, "FPS Cap",
                new[] { "Unlimited", "30", "60", "120", "144", "240" }, 0, OnFpsCapChanged);

            // Rendering
            OptionsUIFactory.CreateSectionHeader(transform, "Rendering");
            renderScaleSlider = OptionsUIFactory.CreateSliderRow(transform, "Render Scale",
                0.5f, 1.0f, 0.05f, vs.RenderScale, OnRenderScaleChanged);
        }

        // ── Populate from Settings ────────────────────────────────────────

        private void PopulateFromSettings()
        {
            if (settingsManager == null) return;
            var vs = settingsManager.GetVideoSettings();

            SetDropdownSilent(windowModeDropdown, (int)vs.WindowMode);
            SetDropdownSilent(resolutionDropdown, FindResolutionIndex(vs.ResolutionWidth, vs.ResolutionHeight));
            SetDropdownSilent(vSyncDropdown, vs.VSync ? 1 : 0);
            SetDropdownSilent(fpsCapDropdown, FpsCapToIndex(vs.FpsCap));
            SetSliderSilent(renderScaleSlider, vs.RenderScale);
        }

        // ── Callbacks ─────────────────────────────────────────────────────

        private void OnTierSelected(int index)
        {
            if (settingsManager == null) return;
            // Index maps directly to QualityTier: 0=Ultra, 1=High, 2=Balanced, 3=Performance
            settingsManager.SetQualityTier(index);
            // Re-sync render scale slider — tier preset may have changed it
            if (renderScaleSlider != null)
                SetSliderSilent(renderScaleSlider, settingsManager.GetVideoSettings().RenderScale);
        }

        private void OnWindowModeChanged(int index)
        {
            if (settingsManager == null) return;
            // Index maps directly to WindowMode: 0=Fullscreen, 1=BorderlessFullscreen, 2=Windowed
            settingsManager.SetVideoSettings(v => v.WindowMode = (R8EOX.Settings.Internal.WindowMode)index);
        }

        private void OnResolutionChanged(int index)
        {
            if (settingsManager == null) return;
            if (availableResolutions == null || index < 0 || index >= availableResolutions.Length) return;
            var res = availableResolutions[index];
            settingsManager.SetVideoSettings(v =>
            {
                v.ResolutionWidth = res.width;
                v.ResolutionHeight = res.height;
            });
        }

        private void OnVSyncChanged(int index)
        {
            if (settingsManager == null) return;
            bool enabled = index == 1;
            settingsManager.SetVideoSettings(v => v.VSync = enabled);
        }

        private void OnFpsCapChanged(int index)
        {
            if (settingsManager == null) return;
            int cap = IndexToFpsCap(index);
            settingsManager.SetVideoSettings(v => v.FpsCap = cap);
        }

        private void OnRenderScaleChanged(float value)
        {
            if (settingsManager == null) return;
            settingsManager.SetVideoSettings(v => v.RenderScale = value);
        }

        // ── Resolution Helpers ────────────────────────────────────────────

        private string[] BuildResolutionOptions()
        {
            var raw = Screen.resolutions;
            if (raw == null || raw.Length == 0)
                raw = new[] { Screen.currentResolution };

            // Deduplicate by WxH — ignore duplicate refresh-rate entries
            var seen = new HashSet<string>();
            var unique = new List<Resolution>();
            foreach (var r in raw)
            {
                string key = $"{r.width}x{r.height}";
                if (seen.Add(key)) unique.Add(r);
            }
            availableResolutions = unique.ToArray();

            var labels = new string[availableResolutions.Length];
            for (int i = 0; i < availableResolutions.Length; i++)
                labels[i] = $"{availableResolutions[i].width}x{availableResolutions[i].height}";
            return labels;
        }

        private int FindResolutionIndex(int width, int height)
        {
            if (availableResolutions == null) return 0;
            for (int i = 0; i < availableResolutions.Length; i++)
            {
                if (availableResolutions[i].width == width && availableResolutions[i].height == height)
                    return i;
            }
            return 0;
        }

        // ── FPS Cap Mapping ───────────────────────────────────────────────

        private static int FpsCapToIndex(int fpsCap)
        {
            for (int i = 0; i < FpsCapValues.Length; i++)
                if (FpsCapValues[i] == fpsCap) return i;
            return 0;
        }

        private static int IndexToFpsCap(int index)
        {
            if (index < 0 || index >= FpsCapValues.Length) return 0;
            return FpsCapValues[index];
        }

        // ── Silent Setters (suppress callbacks during populate) ───────────

        private static void SetDropdownSilent(TMP_Dropdown dd, int index)
        {
            if (dd == null) return;
            dd.SetValueWithoutNotify(Mathf.Clamp(index, 0, dd.options.Count - 1));
        }

        private static void SetSliderSilent(Slider slider, float value)
        {
            if (slider == null) return;
            slider.SetValueWithoutNotify(value);
        }
    }
}
