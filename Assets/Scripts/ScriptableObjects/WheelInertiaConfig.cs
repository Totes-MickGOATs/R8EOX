using UnityEngine;

namespace R8EOX
{
    /// <summary>
    /// Wheel inertia and gyroscopic tuning parameters.
    /// Controls the strength of gyroscopic precession and reaction torque
    /// during airborne flight. Game-feel multipliers allow boosting the
    /// physically accurate values for better gameplay.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWheelInertiaConfig", menuName = "R8EOX/WheelInertiaConfig")]
    public class WheelInertiaConfig : ScriptableObject
    {
        [Tooltip("Moment of inertia per wheel (kg*m^2). Typical 1/10th RC: 0.006")]
        [SerializeField] private float _wheelMoI = 0.006f;

        [Tooltip("Game-feel multiplier for gyroscopic effect. 1.0 = physically accurate.")]
        [SerializeField] private float _gyroScale = 1.5f;

        [Tooltip("Game-feel multiplier for reaction torque (pitch control). 1.0 = physically accurate.")]
        [SerializeField] private float _reactionScale = 30.0f;

        /// <summary>Moment of inertia per wheel in kg*m^2.</summary>
        public float WheelMoI => _wheelMoI;

        /// <summary>Game-feel multiplier for gyroscopic precession torque.</summary>
        public float GyroScale => _gyroScale;

        /// <summary>Game-feel multiplier for reaction torque (throttle/brake pitch).</summary>
        public float ReactionScale => _reactionScale;
    }
}
