using UnityEngine;

namespace R8EOX.Audio.Internal
{
    internal class EngineSound : MonoBehaviour
    {
        [SerializeField] private AudioSource engineSource;
        [SerializeField] private float maxRPM = 10000f;
        [SerializeField] private float minPitch = 0.5f;
        [SerializeField] private float maxPitch = 2.0f;
        [SerializeField] private float idleVolume = 0.3f;
        [SerializeField] private float fullVolume = 1.0f;
        [SerializeField] private float volumeSmoothing = 5f;
        [SerializeField] private float pitchSmoothing = 8f;

        private float currentVolume;
        private float currentPitch;

        private void Start()
        {
            if (engineSource == null) return;

            engineSource.loop = true;
            engineSource.playOnAwake = false;

            if (!engineSource.isPlaying)
                engineSource.Play();

            currentVolume = idleVolume;
            currentPitch = minPitch;
        }

        internal void UpdateEngine(float rpm, float throttle)
        {
            if (engineSource == null) return;

            float normalizedRPM = Mathf.Clamp01(rpm / maxRPM);
            float targetPitch = Mathf.Lerp(minPitch, maxPitch, normalizedRPM);
            float targetVolume = Mathf.Lerp(idleVolume, fullVolume, throttle);

            float dt = Time.deltaTime;
            currentPitch = Mathf.MoveTowards(currentPitch, targetPitch, pitchSmoothing * dt);
            currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, volumeSmoothing * dt);

            engineSource.pitch = currentPitch;
            engineSource.volume = currentVolume;

            if (!engineSource.isPlaying)
                engineSource.Play();
        }
    }
}
