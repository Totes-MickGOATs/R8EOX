using UnityEngine;

namespace R8EOX.AI.Internal
{
    /// <summary>
    /// Difficulty-driven modifiers for AI driving behavior.
    /// Controls speed, throttle consistency, and overtake aggression.
    /// </summary>
    internal class AIBehavior
    {
        private float aggression;
        private float consistency;

        /// <summary>Aggression factor (0..1) derived from difficulty.</summary>
        internal float Aggression => aggression;

        internal void Initialize(int difficulty)
        {
            aggression = Mathf.Clamp01(difficulty / 10f);
            consistency = Mathf.Clamp01(0.5f + difficulty / 20f);
        }

        /// <summary>
        /// Apply consistency jitter to base throttle.
        /// Lower difficulty = more variation, less reliable throttle.
        /// </summary>
        internal float ModifyThrottle(float baseThrottle)
        {
            return baseThrottle * consistency;
        }

        /// <summary>
        /// Maximum speed factor based on difficulty.
        /// Lower difficulty = slower top speed (0.6 at diff 0, 1.0 at diff 10).
        /// </summary>
        internal float GetMaxSpeedFactor()
        {
            return 0.6f + aggression * 0.4f;
        }

        /// <summary>
        /// Curvature sensitivity — lower difficulty drivers brake earlier/harder.
        /// Returns a multiplier on the curvature brake threshold.
        /// </summary>
        internal float GetBrakeSensitivity()
        {
            return 1.5f - aggression * 0.8f;
        }

        internal bool ShouldAttemptOvertake(float distanceToCarAhead)
        {
            return distanceToCarAhead < 10f && aggression > 0.5f;
        }

        internal bool ShouldDraft(float distanceToCarAhead)
        {
            return distanceToCarAhead < 20f && distanceToCarAhead > 5f;
        }
    }
}
