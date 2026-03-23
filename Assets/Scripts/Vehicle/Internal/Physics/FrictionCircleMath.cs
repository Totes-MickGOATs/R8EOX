using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    /// <summary>
    /// Friction ellipse model for combined lateral/longitudinal slip.
    /// When combined demand exceeds the friction circle, both forces
    /// scale down proportionally to stay on the circle boundary.
    /// </summary>
    internal static class FrictionCircleMath
    {
        /// <summary>
        /// Compute scale factors for lateral and longitudinal forces
        /// to keep them within the friction circle.
        /// </summary>
        /// <param name="latForce">Lateral force magnitude (N)</param>
        /// <param name="lonForce">Longitudinal force magnitude (N)</param>
        /// <param name="maxGripForce">Maximum grip force = gripCoeff * gripLoad (N)</param>
        /// <returns>(latScale, lonScale) both in [0, 1]. When combined demand
        /// is within the circle, both are 1.0. When exceeding, both scale
        /// proportionally so the resultant sits on the circle boundary.</returns>
        public static (float latScale, float lonScale) ComputeCombinedSlipScale(
            float latForce, float lonForce, float maxGripForce)
        {
            if (maxGripForce <= 0f)
                return (0f, 0f);

            float latAbs = Mathf.Abs(latForce);
            float lonAbs = Mathf.Abs(lonForce);

            // Friction ellipse: (Flat/Fmax)^2 + (Flon/Fmax)^2
            float latNorm = latAbs / maxGripForce;
            float lonNorm = lonAbs / maxGripForce;
            float combinedSq = latNorm * latNorm + lonNorm * lonNorm;

            if (combinedSq <= 1f)
                return (1f, 1f);

            // Scale both proportionally to sit on the circle boundary
            float scale = 1f / Mathf.Sqrt(combinedSq);
            return (scale, scale);
        }
    }
}
