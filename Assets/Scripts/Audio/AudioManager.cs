using UnityEngine;
using R8EOX.Audio.Internal;

namespace R8EOX.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private EngineSound engineSound;
        [SerializeField] private TireSound tireSound;
        [SerializeField] private MusicPlayer musicPlayer;

        [Header("Collision")]
        [SerializeField] private AudioSource collisionSource;
        [SerializeField] private AudioClip collisionClip;
        [SerializeField] private float minCollisionForce = 50f;
        [SerializeField] private float maxCollisionForce = 500f;
        [SerializeField] private float maxCollisionVolume = 1f;

        private R8EOX.Vehicle.VehicleManager vehicleManager;
        private bool hasTarget;

        public void SetTarget(GameObject vehicle)
        {
            if (vehicle != null)
                vehicleManager = vehicle.GetComponent<R8EOX.Vehicle.VehicleManager>();
            else
                vehicleManager = null;

            hasTarget = vehicleManager != null;
        }

        private void LateUpdate()
        {
            if (!hasTarget) return;

            var telemetry = vehicleManager.GetTelemetry();
            UpdateEngineAudio(telemetry);
            UpdateTireAudio(telemetry);
            UpdateCollisionAudio(telemetry);
        }

        private void UpdateEngineAudio(R8EOX.Vehicle.VehicleTelemetry telemetry)
        {
            if (engineSound == null) return;
            engineSound.UpdateEngine(telemetry.EngineRpm, telemetry.ThrottleAmount);
        }

        private void UpdateTireAudio(R8EOX.Vehicle.VehicleTelemetry telemetry)
        {
            if (tireSound == null) return;
            if (telemetry.Wheels == null || telemetry.Wheels.Length == 0) return;

            float slipSum = 0f;
            int groundedCount = 0;

            for (int i = 0; i < telemetry.Wheels.Length; i++)
            {
                if (!telemetry.Wheels[i].IsOnGround) continue;
                slipSum += telemetry.Wheels[i].SlipRatio;
                groundedCount++;
            }

            float averageSlip = groundedCount > 0 ? slipSum / groundedCount : 0f;
            tireSound.UpdateTireAudio(averageSlip);
        }

        private void UpdateCollisionAudio(R8EOX.Vehicle.VehicleTelemetry telemetry)
        {
            if (collisionSource == null || collisionClip == null) return;
            if (telemetry.LastCollisionImpulse <= minCollisionForce) return;

            float normalized = Mathf.InverseLerp(minCollisionForce, maxCollisionForce,
                telemetry.LastCollisionImpulse);
            float volume = normalized * maxCollisionVolume;
            collisionSource.PlayOneShot(collisionClip, volume);
        }

        // --- Manual override API (callable by other systems directly) ---

        public void PlayEngineSound(float rpm, float throttle)
        {
            if (engineSound != null)
                engineSound.UpdateEngine(rpm, throttle);
        }

        public void PlayTireSound(float slipAmount, string surfaceType)
        {
            if (tireSound == null) return;
            tireSound.UpdateTireAudio(slipAmount);
            tireSound.SetSurface(surfaceType);
        }

        public void PlayCollisionSound(float impactForce)
        {
            if (collisionSource == null || collisionClip == null) return;
            if (impactForce <= minCollisionForce) return;

            float normalized = Mathf.InverseLerp(minCollisionForce, maxCollisionForce,
                impactForce);
            float volume = normalized * maxCollisionVolume;
            collisionSource.PlayOneShot(collisionClip, volume);
        }

        public void SetMusicTrack(string trackName)
        {
            // Music track lookup by name would require an audio library SO.
            // For now this is a placeholder for future resource loading.
            if (musicPlayer != null)
                Debug.LogWarning($"[AudioManager] SetMusicTrack by name not yet implemented: {trackName}");
        }

        public void SetMusicVolume(float volume)
        {
            if (musicPlayer != null)
                musicPlayer.SetVolume(volume);
        }

        public void StopMusic()
        {
            if (musicPlayer != null)
                musicPlayer.Stop();
        }
    }
}
