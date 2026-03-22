using UnityEngine;

namespace R8EOX.Audio.Internal
{
    internal class EngineSound : MonoBehaviour
    {
        [SerializeField] private AudioSource engineSource;
        [SerializeField] private float minPitch = 0.5f;
        [SerializeField] private float maxPitch = 2.0f;

        internal void UpdateEngine(float rpm, float maxRPM, float throttle)
        {
            // TODO: Adjust pitch and volume based on RPM and throttle
            float normalizedRPM = Mathf.Clamp01(rpm / maxRPM);
            if (engineSource != null)
            {
                engineSource.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedRPM);
            }
        }
    }
}
