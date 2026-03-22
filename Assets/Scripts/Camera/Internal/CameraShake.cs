using UnityEngine;

namespace R8EOX.Camera.Internal
{
    internal class CameraShake
    {
        private float shakeIntensity;
        private float shakeDuration;
        private float shakeTimer;

        internal void StartShake(float intensity, float duration)
        {
            shakeIntensity = intensity;
            shakeDuration = duration;
            shakeTimer = duration;
        }

        internal Vector3 GetShakeOffset()
        {
            if (shakeTimer <= 0f)
            {
                return Vector3.zero;
            }

            float decay = shakeTimer / shakeDuration;
            return Random.insideUnitSphere * shakeIntensity * decay;
        }

        internal void Tick(float deltaTime)
        {
            if (shakeTimer > 0f)
            {
                shakeTimer -= deltaTime;
            }
        }

        internal bool IsShaking()
        {
            return shakeTimer > 0f;
        }
    }
}
