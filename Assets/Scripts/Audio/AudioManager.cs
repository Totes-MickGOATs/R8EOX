using UnityEngine;

namespace R8EOX.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public void PlayEngineSound(float rpm, float throttle)
        {
            // TODO: Route to EngineSound component
        }

        public void PlayTireSound(float slipAmount, string surfaceType)
        {
            // TODO: Route to TireSound component
        }

        public void PlayCollisionSound(float impactForce)
        {
            // TODO: Play collision SFX based on impact force
        }

        public void SetMusicTrack(string trackName)
        {
            // TODO: Route to MusicPlayer
        }

        public void SetMusicVolume(float volume)
        {
            // TODO: Adjust music volume
        }
    }
}
