using UnityEngine;

namespace R8EOX.Audio.Internal
{
    internal class TireSound : MonoBehaviour
    {
        [SerializeField] private AudioSource tireSource;
        [SerializeField] private float slipThreshold = 0.15f;
        [SerializeField] private float maxVolume = 0.8f;
        [SerializeField] private float minPitch = 0.8f;
        [SerializeField] private float maxPitch = 1.2f;
        [SerializeField] private float volumeSmoothing = 10f;
        [SerializeField] private float pitchSmoothing = 8f;

        private float currentVolume;
        private float currentPitch;

        private void Start()
        {
            if (tireSource == null) return;

            tireSource.loop = true;
            tireSource.playOnAwake = false;
            tireSource.volume = 0f;
            currentPitch = minPitch;
        }

        internal void UpdateTireAudio(float slipAmount)
        {
            if (tireSource == null) return;

            float dt = Time.deltaTime;

            if (slipAmount < slipThreshold)
            {
                currentVolume = Mathf.MoveTowards(currentVolume, 0f, volumeSmoothing * dt);
                tireSource.volume = currentVolume;

                if (currentVolume <= 0.01f && tireSource.isPlaying)
                    tireSource.Stop();

                return;
            }

            float effectiveSlip = Mathf.InverseLerp(slipThreshold, 1f, slipAmount);
            float targetVolume = effectiveSlip * maxVolume;
            float targetPitch = Mathf.Lerp(minPitch, maxPitch, effectiveSlip);

            currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, volumeSmoothing * dt);
            currentPitch = Mathf.MoveTowards(currentPitch, targetPitch, pitchSmoothing * dt);

            tireSource.volume = currentVolume;
            tireSource.pitch = currentPitch;

            if (!tireSource.isPlaying)
                tireSource.Play();
        }

        internal void SetSurface(string surfaceType)
        {
            // Future: swap tire AudioClip based on surface type (asphalt, gravel, dirt, grass).
            // Requires a surface-to-clip mapping, likely via a serialized dictionary or SO.
        }
    }
}
