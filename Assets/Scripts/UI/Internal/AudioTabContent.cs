using UnityEngine;
using UnityEngine.UI;
using AudioSettingsData = R8EOX.Settings.Internal.AudioSettings;

namespace R8EOX.UI.Internal
{
    internal class AudioTabContent : MonoBehaviour
    {
        private Settings.SettingsManager settingsManager;

        internal void Initialize(Settings.SettingsManager settings)
        {
            settingsManager = settings;
            BuildUI();
        }

        private void BuildUI()
        {
            var vg = gameObject.AddComponent<VerticalLayoutGroup>();
            vg.childForceExpandWidth = true;
            vg.childForceExpandHeight = false;
            vg.spacing = 8f;
            vg.padding = new RectOffset(0, 0, 0, 0);

            AudioSettingsData audio = settingsManager.GetAudioSettings();

            OptionsUIFactory.CreateSectionHeader(transform, "Volume");
            OptionsUIFactory.CreateSliderRow(transform, "Master", 0f, 1f, 0.01f,
                audio.MasterVolume, v => OnVolumeChanged("master", v));
            OptionsUIFactory.CreateSliderRow(transform, "SFX", 0f, 1f, 0.01f,
                audio.SfxVolume, v => OnVolumeChanged("sfx", v));
            OptionsUIFactory.CreateSliderRow(transform, "Music", 0f, 1f, 0.01f,
                audio.MusicVolume, v => OnVolumeChanged("music", v));
        }

        private void OnVolumeChanged(string channel, float value)
        {
            settingsManager.SetAudioSettings(a =>
            {
                switch (channel)
                {
                    case "master": a.MasterVolume = value; break;
                    case "sfx":    a.SfxVolume    = value; break;
                    case "music":  a.MusicVolume  = value; break;
                }
            });
        }
    }
}
