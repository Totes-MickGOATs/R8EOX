using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace R8EOX.UI.Internal
{
    internal class CalibrationTabContent : MonoBehaviour
    {
        private Settings.SettingsManager _settingsManager;

        private StickVisualizer _leftStick;
        private StickVisualizer _rightStick;
        private TriggerVisualizer _leftTrigger;
        private TriggerVisualizer _rightTrigger;

        // Slider references held for RESET
        private Slider _steerSensSlider;
        private Slider _throttleSensSlider;
        private Slider _brakeSensSlider;
        private Slider _steerOuterSlider;
        private Slider _throttleOuterSlider;
        private Slider _brakeOuterSlider;
        private Slider _leftXOffsetSlider;
        private Slider _leftYOffsetSlider;
        private Slider _rightXOffsetSlider;
        private Slider _rightYOffsetSlider;

        internal void Initialize(Settings.SettingsManager settings)
        {
            _settingsManager = settings;

            OptionsUIFactory.SetupTabLayout(gameObject, padding: new RectOffset(0, 0, 0, 16), addFitter: true);

            BuildControllerSection();
            BuildAxisTestSection();
            BuildPerAxisSection();
            BuildActionButtons();
        }

        private void BuildControllerSection()
        {
            OptionsUIFactory.CreateSectionHeader(transform, "Preferred Controller");
            OptionsUIFactory.CreateDropdownRow(
                transform,
                "Controller",
                new[] { "Any Controller" },
                0,
                _ => { /* future: populate from connected devices */ });
        }

        private void BuildAxisTestSection()
        {
            OptionsUIFactory.CreateSectionHeader(transform, "Axis Test");

            // Row: two sticks side by side
            var sticksRow = new GameObject("SticksRow", typeof(RectTransform));
            sticksRow.transform.SetParent(transform, false);
            var hg = sticksRow.AddComponent<HorizontalLayoutGroup>();
            hg.childForceExpandWidth  = false;
            hg.childForceExpandHeight = false;
            hg.spacing = 24f;
            var hle = sticksRow.AddComponent<LayoutElement>();
            hle.minHeight = 160f;
            hle.preferredHeight = 160f;

            _leftStick  = CreateStickVisualizer(sticksRow.transform, "Left Stick");
            _rightStick = CreateStickVisualizer(sticksRow.transform, "Right Stick");

            // Row: two triggers side by side
            var triggersRow = new GameObject("TriggersRow", typeof(RectTransform));
            triggersRow.transform.SetParent(transform, false);
            var thg = triggersRow.AddComponent<HorizontalLayoutGroup>();
            thg.childForceExpandWidth  = false;
            thg.childForceExpandHeight = false;
            thg.spacing = 24f;
            var tle = triggersRow.AddComponent<LayoutElement>();
            tle.minHeight = 36f;
            tle.preferredHeight = 36f;

            _leftTrigger  = CreateTriggerVisualizer(triggersRow.transform, "LT");
            _rightTrigger = CreateTriggerVisualizer(triggersRow.transform, "RT");
        }

        private void BuildPerAxisSection()
        {
            var cal = _settingsManager.GetCalibrationSettings();

            OptionsUIFactory.CreateSectionHeader(transform, "Per-Axis Settings");

            // Inversions
            OptionsUIFactory.CreateCheckboxRow(transform, "Invert Steer",    cal.InvertSteer,
                v => _settingsManager.SetCalibrationSettings(c => c.InvertSteer    = v));
            OptionsUIFactory.CreateCheckboxRow(transform, "Invert Throttle", cal.InvertThrottle,
                v => _settingsManager.SetCalibrationSettings(c => c.InvertThrottle = v));
            OptionsUIFactory.CreateCheckboxRow(transform, "Invert Brake",    cal.InvertBrake,
                v => _settingsManager.SetCalibrationSettings(c => c.InvertBrake    = v));

            // Sensitivities (1–3, step 0.01)
            _steerSensSlider = OptionsUIFactory.CreateSliderRow(transform, "Steer Sensitivity",
                1f, 3f, 0.01f, cal.SteerSensitivity,
                v => _settingsManager.SetCalibrationSettings(c => c.SteerSensitivity = v));
            _throttleSensSlider = OptionsUIFactory.CreateSliderRow(transform, "Throttle Sensitivity",
                1f, 3f, 0.01f, cal.ThrottleSensitivity,
                v => _settingsManager.SetCalibrationSettings(c => c.ThrottleSensitivity = v));
            _brakeSensSlider = OptionsUIFactory.CreateSliderRow(transform, "Brake Sensitivity",
                1f, 3f, 0.01f, cal.BrakeSensitivity,
                v => _settingsManager.SetCalibrationSettings(c => c.BrakeSensitivity = v));

            // Outer deadzones (0.7–1, step 0.01)
            _steerOuterSlider = OptionsUIFactory.CreateSliderRow(transform, "Steer Outer Deadzone",
                0.7f, 1f, 0.01f, cal.SteerOuterDeadzone,
                v => _settingsManager.SetCalibrationSettings(c => c.SteerOuterDeadzone = v));
            _throttleOuterSlider = OptionsUIFactory.CreateSliderRow(transform, "Throttle Outer Deadzone",
                0.7f, 1f, 0.01f, cal.ThrottleOuterDeadzone,
                v => _settingsManager.SetCalibrationSettings(c => c.ThrottleOuterDeadzone = v));
            _brakeOuterSlider = OptionsUIFactory.CreateSliderRow(transform, "Brake Outer Deadzone",
                0.7f, 1f, 0.01f, cal.BrakeOuterDeadzone,
                v => _settingsManager.SetCalibrationSettings(c => c.BrakeOuterDeadzone = v));

            // Drift offsets (-0.15–0.15, step 0.001)
            _leftXOffsetSlider = OptionsUIFactory.CreateSliderRow(transform, "Left X Offset",
                -0.15f, 0.15f, 0.001f, cal.LeftXOffset,
                v => _settingsManager.SetCalibrationSettings(c => c.LeftXOffset = v));
            _leftYOffsetSlider = OptionsUIFactory.CreateSliderRow(transform, "Left Y Offset",
                -0.15f, 0.15f, 0.001f, cal.LeftYOffset,
                v => _settingsManager.SetCalibrationSettings(c => c.LeftYOffset = v));
            _rightXOffsetSlider = OptionsUIFactory.CreateSliderRow(transform, "Right X Offset",
                -0.15f, 0.15f, 0.001f, cal.RightXOffset,
                v => _settingsManager.SetCalibrationSettings(c => c.RightXOffset = v));
            _rightYOffsetSlider = OptionsUIFactory.CreateSliderRow(transform, "Right Y Offset",
                -0.15f, 0.15f, 0.001f, cal.RightYOffset,
                v => _settingsManager.SetCalibrationSettings(c => c.RightYOffset = v));
        }

        private void BuildActionButtons()
        {
            OptionsUIFactory.CreateSectionHeader(transform, "Calibration");

            OptionsUIFactory.CreateActionButton(
                transform, "CALIBRATE", OptionsUIFactory.STYLE_PRIMARY,
                () => CalibrationWizard.Show(OnWizardComplete, null));

            OptionsUIFactory.CreateActionButton(
                transform, "RESET TO DEFAULTS", OptionsUIFactory.STYLE_DANGER,
                () => ConfirmDialog.Show(
                    "Reset Calibration",
                    "This will reset all calibration settings to their defaults.",
                    "RESET",
                    true,
                    OnResetConfirmed));
        }

        private void Update()
        {
            if (!gameObject.activeSelf)
                return;

            var pad = Gamepad.current;
            if (pad == null)
                return;

            var ls = pad.leftStick.ReadValue();
            var rs = pad.rightStick.ReadValue();
            float lt = pad.leftTrigger.ReadValue();
            float rt = pad.rightTrigger.ReadValue();

            _leftStick?.UpdateInput(ls, ls);
            _rightStick?.UpdateInput(rs, rs);
            _leftTrigger?.UpdateValue(lt, lt);
            _rightTrigger?.UpdateValue(rt, rt);
        }

        private void OnWizardComplete(float[] neutralOffsets, float[,] axisRanges)
        {
            _settingsManager.SetCalibrationSettings(c => c.ApplyCalibrationData(neutralOffsets, axisRanges));
            RefreshSliders();
        }

        private void OnResetConfirmed()
        {
            _settingsManager.SetCalibrationSettings(c =>
            {
                c.InvertSteer           = false;
                c.InvertThrottle        = false;
                c.InvertBrake           = false;
                c.SteerSensitivity      = 1f;
                c.ThrottleSensitivity   = 1f;
                c.BrakeSensitivity      = 1f;
                c.SteerOuterDeadzone    = 1f;
                c.ThrottleOuterDeadzone = 1f;
                c.BrakeOuterDeadzone    = 1f;
                c.LeftXOffset   = 0f;
                c.LeftYOffset   = 0f;
                c.RightXOffset  = 0f;
                c.RightYOffset  = 0f;
            });
            RefreshSliders();
        }

        private void RefreshSliders()
        {
            var cal = _settingsManager.GetCalibrationSettings();
            if (_steerSensSlider)     _steerSensSlider.SetValueWithoutNotify(cal.SteerSensitivity);
            if (_throttleSensSlider)  _throttleSensSlider.SetValueWithoutNotify(cal.ThrottleSensitivity);
            if (_brakeSensSlider)     _brakeSensSlider.SetValueWithoutNotify(cal.BrakeSensitivity);
            if (_steerOuterSlider)    _steerOuterSlider.SetValueWithoutNotify(cal.SteerOuterDeadzone);
            if (_throttleOuterSlider) _throttleOuterSlider.SetValueWithoutNotify(cal.ThrottleOuterDeadzone);
            if (_brakeOuterSlider)    _brakeOuterSlider.SetValueWithoutNotify(cal.BrakeOuterDeadzone);
            if (_leftXOffsetSlider)   _leftXOffsetSlider.SetValueWithoutNotify(cal.LeftXOffset);
            if (_leftYOffsetSlider)   _leftYOffsetSlider.SetValueWithoutNotify(cal.LeftYOffset);
            if (_rightXOffsetSlider)  _rightXOffsetSlider.SetValueWithoutNotify(cal.RightXOffset);
            if (_rightYOffsetSlider)  _rightYOffsetSlider.SetValueWithoutNotify(cal.RightYOffset);
        }

        private StickVisualizer CreateStickVisualizer(Transform parent, string label)
        {
            var go = new GameObject(label.Replace(" ", "") + "Viz", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var le = go.AddComponent<LayoutElement>();
            le.minWidth = 140f; le.preferredWidth = 140f;
            le.minHeight = 140f; le.preferredHeight = 140f;
            var viz = go.AddComponent<StickVisualizer>();
            viz.Initialize(label, 0f);
            return viz;
        }

        private TriggerVisualizer CreateTriggerVisualizer(Transform parent, string label)
        {
            var go = new GameObject(label + "Viz", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var le = go.AddComponent<LayoutElement>();
            le.minWidth = 120f; le.preferredWidth = 120f;
            le.minHeight = 16f; le.preferredHeight = 16f;
            var viz = go.AddComponent<TriggerVisualizer>();
            viz.Initialize(label);
            return viz;
        }
    }
}
