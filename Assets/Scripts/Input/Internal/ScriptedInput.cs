using UnityEngine;

namespace R8EOX.Input.Internal
{
    /// <summary>
    /// Implements IVehicleInput with externally settable properties.
    /// Used by autopilot and path-following systems to drive a vehicle
    /// without reading player hardware input.
    /// </summary>
    internal class ScriptedInput : MonoBehaviour, IVehicleInput
    {
        // ── IVehicleInput — continuous axes ─────────────────────────────────

        public float Throttle { get; set; }
        public float Brake    { get; set; }
        public float Steer    { get; set; }

        // ── IVehicleInput — one-frame buttons ───────────────────────────────

        public bool ResetPressed        { get; set; }
        public bool DebugTogglePressed  { get; set; }
        public bool CameraCyclePressed  { get; set; }
        public bool PausePressed        { get; set; }

        // ── Unity lifecycle ──────────────────────────────────────────────────

        private void LateUpdate()
        {
            ClearFrameInputs();
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Resets all one-frame button states to false.
        /// Called automatically in LateUpdate; call manually when you need
        /// to flush mid-frame (e.g. after the vehicle has consumed them).
        /// </summary>
        public void ClearFrameInputs()
        {
            ResetPressed       = false;
            DebugTogglePressed = false;
            CameraCyclePressed = false;
            PausePressed       = false;
        }
    }
}
