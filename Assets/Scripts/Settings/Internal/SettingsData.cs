using System;

namespace R8EOX.Settings.Internal
{
    [Serializable]
    internal class SettingsData
    {
        // Fields are public because this is a plain [Serializable] data bag used as a
        // runtime aggregate container — not a MonoBehaviour or ScriptableObject — and
        // SettingsManager assigns sub-settings directly (e.g. currentData.audio = ...).
        // These fields are never exposed to the Inspector.
        public VideoSettings video;
        public AudioSettings audio;
        public ControlsSettings controls;
        public CalibrationSettings calibration;
        public GameplaySettings gameplay;
        public ProfileSettings profiles;

        internal SettingsData()
        {
            video = new VideoSettings();
            audio = new AudioSettings();
            controls = new ControlsSettings();
            calibration = new CalibrationSettings();
            gameplay = new GameplaySettings();
            profiles = new ProfileSettings();
        }

        internal static SettingsData CreateDefault()
        {
            return new SettingsData();
        }

        internal SettingsData Clone()
        {
            var clone = new SettingsData();
            clone.video = video.Clone();
            clone.audio = audio.Clone();
            clone.controls = controls.Clone();
            clone.calibration = calibration.Clone();
            clone.gameplay = gameplay.Clone();
            clone.profiles = profiles.Clone();
            return clone;
        }
    }
}
