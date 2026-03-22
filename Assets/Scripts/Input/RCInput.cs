using UnityEngine;
using UnityEngine.InputSystem;
using R8EOX.Input.Internal;

namespace R8EOX.Input
{
    /// <summary>
    /// Player input provider for the RC buggy using Unity's Input System package.
    /// Supports keyboard (WASD) and gamepad (triggers + stick) via the R8EOXInputActions asset.
    /// Implements IVehicleInput for swappable input sources.
    /// </summary>
    public class RCInput : MonoBehaviour, IVehicleInput
    {
        // ---- Serialized Fields ----

        [Header("Steering")]
        [Tooltip("Exponent applied to steering input for non-linear response (1.0=linear, 1.5=default)")]
        [SerializeField] private float steerCurveExponent = 1.5f;


        // ---- IVehicleInput Properties ----

        /// <inheritdoc/>
        public float Throttle { get; private set; }
        /// <inheritdoc/>
        public float Brake { get; private set; }
        /// <inheritdoc/>
        public float Steer { get; private set; }
        /// <inheritdoc/>
        public bool ResetPressed { get; private set; }
        /// <inheritdoc/>
        public bool DebugTogglePressed { get; private set; }
        /// <inheritdoc/>
        public bool CameraCyclePressed { get; private set; }
        /// <inheritdoc/>
        public bool PausePressed { get; private set; }


        // ---- Private Fields ----

        private R8EOXInputActions actions;


        // ---- Unity Lifecycle ----

        private void Awake()
        {
            actions = new R8EOXInputActions();
        }

        private void OnEnable()
        {
            actions.Gameplay.Enable();
        }

        private void OnDisable()
        {
            actions.Gameplay.Disable();
        }

        private void OnDestroy()
        {
            actions?.Dispose();
        }

        private void Update()
        {
            Throttle = actions.Gameplay.Throttle.ReadValue<float>();
            Brake = actions.Gameplay.Brake.ReadValue<float>();

            float rawSteer = actions.Gameplay.Steer.ReadValue<float>();
            Steer = InputMath.ApplySteeringCurve(rawSteer, steerCurveExponent);

            ResetPressed = actions.Gameplay.Reset.WasPressedThisFrame();
            DebugTogglePressed = actions.Gameplay.DebugToggle.WasPressedThisFrame();
            CameraCyclePressed = actions.Gameplay.CameraCycle.WasPressedThisFrame();
            PausePressed = actions.Gameplay.Pause.WasPressedThisFrame();
        }
    }
}
