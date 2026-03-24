using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    /// <summary>
    /// Physics-first airborne torques for RC buggy.
    /// Two gyroscopic equations produce emergent behavior:
    ///   1. Precession: yaw-pitch coupling from spinning wheels
    ///   2. Reaction: throttle pitches nose up, brake pitches nose down
    /// No direct input-to-torque mapping. All air control comes from wheel physics.
    /// </summary>
    internal class RCAirPhysics : MonoBehaviour
    {
        // ---- Constants ----

        const float k_RpmToRadPerSec = Mathf.PI / 30f;


        // ---- Serialized Fields ----

        [Header("Gyroscopic Configuration")]
        [Tooltip("Enable gyroscopic precession and reaction torques while airborne. Disable to let PhysX handle airborne tumble naturally.")]
        [SerializeField] private bool _enableGyroscopicTorques = true;
        [Tooltip("Wheel inertia and scale factors. Create via Assets > Create > R8EOX > Wheel Inertia Config")]
        [SerializeField] private WheelInertiaConfig _inertiaConfig;


        // ---- Fallback Defaults (used when no config asset assigned) ----

        const float k_DefaultWheelMoI = 0.006f;
        const float k_DefaultGyroScale = 1.5f;
        const float k_DefaultReactionScale = 30.0f;


        // ---- Private Fields ----

        private Rigidbody _rb;
        private RaycastWheel[] _wheels;
        private float[] _prevWheelSpinRates;
        private Vector3[] _prevWheelSpinAxes;


        // ---- Properties ----

        /// <summary>Wheel moment of inertia from config or default.</summary>
        private float WheelMoI => _inertiaConfig != null ? _inertiaConfig.WheelMoI : k_DefaultWheelMoI;

        /// <summary>Gyroscopic scale from config or default.</summary>
        private float GyroScale => _inertiaConfig != null ? _inertiaConfig.GyroScale : k_DefaultGyroScale;

        /// <summary>Reaction scale from config or default.</summary>
        private float ReactionScale => _inertiaConfig != null ? _inertiaConfig.ReactionScale : k_DefaultReactionScale;


        // ---- Unity Lifecycle ----

        void Start()
        {
            _rb = GetComponentInParent<Rigidbody>();
            if (_rb == null)
            {
                Debug.LogWarning("[RCAirPhysics] No Rigidbody found in parent hierarchy. Disabling component.", this);
                enabled = false;
                return;
            }

            _wheels = GetComponentsInChildren<RaycastWheel>();
            if (_wheels.Length == 0 && transform.parent != null)
                _wheels = transform.parent.GetComponentsInChildren<RaycastWheel>();

            _prevWheelSpinRates = new float[_wheels.Length];
            _prevWheelSpinAxes = new Vector3[_wheels.Length];
            for (int i = 0; i < _wheels.Length; i++)
                _prevWheelSpinAxes[i] = _wheels[i].transform.right;
        }


        // ---- Public API ----

        /// <summary>
        /// Apply gyroscopic torques. Called by VehicleManager when airborne.
        /// Throttle/brake/steer parameters are unused -- all torque comes from
        /// wheel spin physics, not direct input mapping.
        /// </summary>
        public void Apply(float dt, float throttle, float brake, float steer)
        {
            if (!_enableGyroscopicTorques) return;
            if (_wheels == null || _wheels.Length == 0 || dt <= 0f) return;

            Vector3 bodyOmega = _rb.angularVelocity;
            Vector3 totalGyroTorque = Vector3.zero;
            Vector3 totalReactionTorque = Vector3.zero;
            float wheelMoI = WheelMoI;

            for (int i = 0; i < _wheels.Length; i++)
            {
                Vector3 spinAxis = _wheels[i].transform.right;
                float currentSpinRate = _wheels[i].WheelRpm * k_RpmToRadPerSec;

                // Gyroscopic precession: tau = omega_body x (I * omega_spin * axis)
                totalGyroTorque += GyroscopicMath.ComputeGyroscopicTorque(
                    bodyOmega, spinAxis, wheelMoI, currentSpinRate);

                // Reaction torque: tau = -axis * I * d(omega)/dt
                if (_wheels[i].IsMotor)
                {
                    totalReactionTorque += GyroscopicMath.ComputeReactionTorque(
                        spinAxis, wheelMoI, currentSpinRate, _prevWheelSpinRates[i], dt);
                }

                // Steering precession: axis change from steering produces yaw torque
                totalGyroTorque += GyroscopicMath.ComputeSteeringPrecessionTorque(
                    spinAxis, _prevWheelSpinAxes[i], wheelMoI, currentSpinRate, dt);

                _prevWheelSpinAxes[i] = spinAxis;
                _prevWheelSpinRates[i] = currentSpinRate;
            }

            Vector3 totalTorque = (totalGyroTorque * GyroScale)
                                + (totalReactionTorque * ReactionScale);

            _rb.AddTorque(totalTorque);
        }
    }
}
