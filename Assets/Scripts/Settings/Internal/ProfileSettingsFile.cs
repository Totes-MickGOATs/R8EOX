using System;

namespace R8EOX.Settings.Internal
{
    [Serializable]
    internal class ProfileSettingsFile
    {
        public AudioSettings audio = new AudioSettings();
        public ControlsSettings controls = new ControlsSettings();
        public CalibrationSettings calibration = new CalibrationSettings();
        public GameplaySettings gameplay = new GameplaySettings();
    }
}
