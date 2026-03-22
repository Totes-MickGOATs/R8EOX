using UnityEngine;

namespace R8EOX.VFX.Internal
{
    internal class ScreenEffects : MonoBehaviour
    {
        [SerializeField] private float maxBlurIntensity = 0.5f;

        private float currentSpeedBlur;
        private float currentDamageVignette;

        internal void SetSpeedBlur(float normalizedSpeed)
        {
            currentSpeedBlur = normalizedSpeed * maxBlurIntensity;
            // TODO: Apply to post-processing volume or shader
        }

        internal void SetDamageVignette(float damagePercent)
        {
            currentDamageVignette = damagePercent;
            // TODO: Apply vignette intensity to post-processing
        }

        internal void ClearAll()
        {
            currentSpeedBlur = 0f;
            currentDamageVignette = 0f;
            // TODO: Reset post-processing effects
        }
    }
}
