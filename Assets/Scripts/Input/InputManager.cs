using UnityEngine;

namespace R8EOX.Input
{
    /// <summary>
    /// Top-level coordinator for the Input system.
    /// Manages enabling/disabling input and provides the active input provider.
    /// VehicleManager resolves IVehicleInput via GetComponent on its own GameObject;
    /// this manager handles system-level concerns like pausing input.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The input provider component (e.g., RCInput). Auto-resolved if null.")]
        private MonoBehaviour inputProviderOverride;

        /// <summary>Current active input provider.</summary>
        public IVehicleInput ActiveInput { get; private set; }

        /// <summary>Whether input is currently enabled.</summary>
        public bool InputEnabled { get; private set; } = true;

        private void Awake()
        {
            ResolveInputProvider();
        }

        /// <summary>
        /// Enable all input processing.
        /// </summary>
        public void EnableInput()
        {
            InputEnabled = true;
            SetProviderEnabled(true);
        }

        /// <summary>
        /// Disable all input processing (e.g., during pause or cutscene).
        /// </summary>
        public void DisableInput()
        {
            InputEnabled = false;
            SetProviderEnabled(false);
        }

        private void ResolveInputProvider()
        {
            if (inputProviderOverride != null
                && inputProviderOverride is IVehicleInput overrideInput)
            {
                ActiveInput = overrideInput;
                return;
            }

            ActiveInput = GetComponentInChildren<IVehicleInput>();
        }

        private void SetProviderEnabled(bool enabled)
        {
            if (ActiveInput is MonoBehaviour mb)
            {
                mb.enabled = enabled;
            }
        }
    }
}
