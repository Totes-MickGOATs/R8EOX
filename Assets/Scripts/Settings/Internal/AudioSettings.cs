using System;

namespace R8EOX.Settings.Internal
{
    [Serializable]
    internal class AudioSettings
    {
        [UnityEngine.SerializeField] private float masterVolume = 1.0f;
        [UnityEngine.SerializeField] private float sfxVolume = 1.0f;
        [UnityEngine.SerializeField] private float musicVolume = 0.7f;

        public float MasterVolume { get => masterVolume; set => masterVolume = value; }
        public float SfxVolume { get => sfxVolume; set => sfxVolume = value; }
        public float MusicVolume { get => musicVolume; set => musicVolume = value; }

        public static AudioSettings CreateDefault()
        {
            return new AudioSettings();
        }

        public AudioSettings Clone()
        {
            return new AudioSettings
            {
                masterVolume = masterVolume,
                sfxVolume = sfxVolume,
                musicVolume = musicVolume
            };
        }
    }
}
