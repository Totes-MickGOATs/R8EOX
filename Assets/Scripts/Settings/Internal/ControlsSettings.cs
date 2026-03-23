using System;

namespace R8EOX.Settings.Internal
{
    /// <summary>
    /// Serializable data container for input/controls settings.
    /// Serialized via JsonUtility; all fields are plain value types or strings.
    /// </summary>
    [Serializable]
    internal class ControlsSettings
    {
        // ── Serialized fields ─────────────────────────────────────────────

        [UnityEngine.SerializeField] private string profileName = "Default";

        /// <summary>Deadzone applied to the steering axis. Range: 0–0.3.</summary>
        [UnityEngine.SerializeField] private float steerDeadzone = 0.1f;

        /// <summary>Deadzone applied to the throttle axis. Range: 0–0.3.</summary>
        [UnityEngine.SerializeField] private float throttleDeadzone = 0.05f;

        /// <summary>Exponent applied to input curve remapping. Range: 1.0–3.0.</summary>
        [UnityEngine.SerializeField] private float curveExponent = 1.5f;

        /// <summary>JSON from InputActionRebindingExtensions.SaveBindingOverridesAsJson().</summary>
        [UnityEngine.SerializeField] private string bindingOverridesJson = "";

        // ── Properties ────────────────────────────────────────────────────

        public string ProfileName { get => profileName; set => profileName = value ?? "Default"; }

        /// <summary>Clamped 0–0.3.</summary>
        public float SteerDeadzone { get => steerDeadzone; set => steerDeadzone = UnityEngine.Mathf.Clamp(value, 0f, 0.3f); }

        /// <summary>Clamped 0–0.3.</summary>
        public float ThrottleDeadzone { get => throttleDeadzone; set => throttleDeadzone = UnityEngine.Mathf.Clamp(value, 0f, 0.3f); }

        /// <summary>Clamped 1.0–3.0.</summary>
        public float CurveExponent { get => curveExponent; set => curveExponent = UnityEngine.Mathf.Clamp(value, 1f, 3f); }

        public string BindingOverridesJson { get => bindingOverridesJson; set => bindingOverridesJson = value ?? ""; }

        // ── Factory ───────────────────────────────────────────────────────

        /// <summary>Returns a new instance populated with default values.</summary>
        public static ControlsSettings CreateDefault()
        {
            return new ControlsSettings
            {
                profileName          = "Default",
                steerDeadzone        = 0.1f,
                throttleDeadzone     = 0.05f,
                curveExponent        = 1.5f,
                bindingOverridesJson = ""
            };
        }

        // ── Utilities ─────────────────────────────────────────────────────

        /// <summary>Returns a deep copy of this instance.</summary>
        public ControlsSettings Clone()
        {
            return new ControlsSettings
            {
                profileName          = profileName,
                steerDeadzone        = steerDeadzone,
                throttleDeadzone     = throttleDeadzone,
                curveExponent        = curveExponent,
                bindingOverridesJson = bindingOverridesJson
            };
        }
    }
}
