using UnityEngine;

namespace R8EOX.Audio.Internal
{
    internal class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource;

        internal void PlayTrack(AudioClip clip)
        {
            // TODO: Crossfade to new track
            if (musicSource != null)
            {
                musicSource.clip = clip;
                musicSource.Play();
            }
        }

        internal void SetVolume(float volume)
        {
            if (musicSource != null)
            {
                musicSource.volume = volume;
            }
        }

        internal void Stop()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }
    }
}
