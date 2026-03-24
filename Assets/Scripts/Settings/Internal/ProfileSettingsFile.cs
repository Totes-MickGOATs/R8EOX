using System;
using UnityEngine;

namespace R8EOX.Settings.Internal
{
    [Serializable]
    internal class ProfileSettingsFile
    {
        [SerializeField] private AudioSettings audio = new AudioSettings();
        [SerializeField] private ControlsSettings controls = new ControlsSettings();
        [SerializeField] private CalibrationSettings calibration = new CalibrationSettings();
        [SerializeField] private GameplaySettings gameplay = new GameplaySettings();

        public AudioSettings Audio { get => audio; set => audio = value; }
        public ControlsSettings Controls { get => controls; set => controls = value; }
        public CalibrationSettings Calibration { get => calibration; set => calibration = value; }
        public GameplaySettings Gameplay { get => gameplay; set => gameplay = value; }
    }
}
