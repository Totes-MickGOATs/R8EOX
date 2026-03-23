using UnityEngine;

namespace R8EOX.Settings.Internal
{
    internal static class AudioApplier
    {
        internal static void Apply(AudioSettings settings)
        {
            // For now, use AudioListener.volume for master volume.
            // In the future this will route through AudioMixer groups.

            // SFX and Music volumes are stored here for AudioManager to read.
            // AudioManager polls these via LastSfxVolume / LastMusicVolume
            // rather than holding a direct dependency on SettingsManager.

            AudioListener.volume = settings.MasterVolume;

            LastSfxVolume = settings.SfxVolume;
            LastMusicVolume = settings.MusicVolume;
        }

        // Static cache for other systems to read.
        internal static float LastSfxVolume { get; private set; } = 1f;
        internal static float LastMusicVolume { get; private set; } = 0.7f;

        // Convert linear 0-1 to dB scale (-80 to 0).
        internal static float LinearToDecibels(float linear)
        {
            if (linear <= 0f) return -80f;
            return 20f * Mathf.Log10(linear);
        }
    }
}
