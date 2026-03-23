using System;

namespace R8EOX.Settings.Internal
{
    /// <summary>
    /// Serializable data container for controller calibration settings.
    /// Populated by the calibration wizard and applied at runtime via the Input system.
    /// </summary>
    [Serializable]
    internal class CalibrationSettings
    {
        // ── Serialized fields ─────────────────────────────────────────────

        [UnityEngine.SerializeField] private string preferredController = "";
        [UnityEngine.SerializeField] private bool invertSteer = false;
        [UnityEngine.SerializeField] private bool invertThrottle = false;
        [UnityEngine.SerializeField] private bool invertBrake = false;

        /// <summary>Multiplier on steer axis output. Range: 1.0–3.0.</summary>
        [UnityEngine.SerializeField] private float steerSensitivity = 1f;

        [UnityEngine.SerializeField] private float throttleSensitivity = 1f;
        [UnityEngine.SerializeField] private float brakeSensitivity = 1f;

        /// <summary>Axis value at which full deflection is registered. Range: 0.7–1.0.</summary>
        [UnityEngine.SerializeField] private float steerOuterDeadzone = 1f;

        [UnityEngine.SerializeField] private float throttleOuterDeadzone = 1f;
        [UnityEngine.SerializeField] private float brakeOuterDeadzone = 1f;

        /// <summary>Neutral-point offset for left stick X axis. Range: -0.15–0.15.</summary>
        [UnityEngine.SerializeField] private float leftXOffset = 0f;

        /// <summary>Neutral-point offset for left stick Y axis. Range: -0.15–0.15.</summary>
        [UnityEngine.SerializeField] private float leftYOffset = 0f;

        /// <summary>Neutral-point offset for right stick X axis. Range: -0.15–0.15.</summary>
        [UnityEngine.SerializeField] private float rightXOffset = 0f;

        /// <summary>Neutral-point offset for right stick Y axis. Range: -0.15–0.15.</summary>
        [UnityEngine.SerializeField] private float rightYOffset = 0f;

        // ── Properties ────────────────────────────────────────────────────

        public string PreferredController { get => preferredController; set => preferredController = value ?? ""; }
        public bool InvertSteer { get => invertSteer; set => invertSteer = value; }
        public bool InvertThrottle { get => invertThrottle; set => invertThrottle = value; }
        public bool InvertBrake { get => invertBrake; set => invertBrake = value; }

        /// <summary>Clamped 1.0–3.0.</summary>
        public float SteerSensitivity { get => steerSensitivity; set => steerSensitivity = UnityEngine.Mathf.Clamp(value, 1f, 3f); }

        public float ThrottleSensitivity { get => throttleSensitivity; set => throttleSensitivity = UnityEngine.Mathf.Max(value, 0.1f); }
        public float BrakeSensitivity { get => brakeSensitivity; set => brakeSensitivity = UnityEngine.Mathf.Max(value, 0.1f); }

        /// <summary>Clamped 0.7–1.0.</summary>
        public float SteerOuterDeadzone { get => steerOuterDeadzone; set => steerOuterDeadzone = UnityEngine.Mathf.Clamp(value, 0.7f, 1f); }

        public float ThrottleOuterDeadzone { get => throttleOuterDeadzone; set => throttleOuterDeadzone = UnityEngine.Mathf.Clamp(value, 0.7f, 1f); }
        public float BrakeOuterDeadzone { get => brakeOuterDeadzone; set => brakeOuterDeadzone = UnityEngine.Mathf.Clamp(value, 0.7f, 1f); }

        /// <summary>Clamped -0.15–0.15.</summary>
        public float LeftXOffset { get => leftXOffset; set => leftXOffset = UnityEngine.Mathf.Clamp(value, -0.15f, 0.15f); }

        /// <summary>Clamped -0.15–0.15.</summary>
        public float LeftYOffset { get => leftYOffset; set => leftYOffset = UnityEngine.Mathf.Clamp(value, -0.15f, 0.15f); }

        /// <summary>Clamped -0.15–0.15.</summary>
        public float RightXOffset { get => rightXOffset; set => rightXOffset = UnityEngine.Mathf.Clamp(value, -0.15f, 0.15f); }

        /// <summary>Clamped -0.15–0.15.</summary>
        public float RightYOffset { get => rightYOffset; set => rightYOffset = UnityEngine.Mathf.Clamp(value, -0.15f, 0.15f); }

        // ── Factory ───────────────────────────────────────────────────────

        /// <summary>Returns a new instance populated with default values.</summary>
        public static CalibrationSettings CreateDefault()
        {
            return new CalibrationSettings();
        }

        // ── Utilities ─────────────────────────────────────────────────────

        /// <summary>Returns a deep copy of this instance.</summary>
        public CalibrationSettings Clone()
        {
            return new CalibrationSettings
            {
                preferredController   = preferredController,
                invertSteer           = invertSteer,
                invertThrottle        = invertThrottle,
                invertBrake           = invertBrake,
                steerSensitivity      = steerSensitivity,
                throttleSensitivity   = throttleSensitivity,
                brakeSensitivity      = brakeSensitivity,
                steerOuterDeadzone    = steerOuterDeadzone,
                throttleOuterDeadzone = throttleOuterDeadzone,
                brakeOuterDeadzone    = brakeOuterDeadzone,
                leftXOffset           = leftXOffset,
                leftYOffset           = leftYOffset,
                rightXOffset          = rightXOffset,
                rightYOffset          = rightYOffset
            };
        }

        /// <summary>
        /// Applies wizard calibration results. neutralOffsets is [leftX, leftY, rightX, rightY].
        /// axisRanges is [axis, 0=min/1=max] — used to derive outer deadzone per axis.
        /// </summary>
        public void ApplyCalibrationData(float[] neutralOffsets, float[,] axisRanges)
        {
            if (neutralOffsets != null && neutralOffsets.Length >= 4)
            {
                LeftXOffset  = neutralOffsets[0];
                LeftYOffset  = neutralOffsets[1];
                RightXOffset = neutralOffsets[2];
                RightYOffset = neutralOffsets[3];
            }

            if (axisRanges == null || axisRanges.GetLength(0) < 1)
                return;

            // Derive outer deadzone from the smallest absolute max-deflection across axes.
            // This ensures the full range is reachable without requiring impossible inputs.
            float minRange = 1f;
            int axisCount = axisRanges.GetLength(0);
            for (int i = 0; i < axisCount; i++)
            {
                float lo = axisRanges[i, 0];
                float hi = axisRanges[i, 1];
                float range = UnityEngine.Mathf.Min(UnityEngine.Mathf.Abs(lo), UnityEngine.Mathf.Abs(hi));
                minRange = UnityEngine.Mathf.Min(minRange, range);
            }

            SteerOuterDeadzone    = minRange;
            ThrottleOuterDeadzone = minRange;
            BrakeOuterDeadzone    = minRange;
        }
    }
}
