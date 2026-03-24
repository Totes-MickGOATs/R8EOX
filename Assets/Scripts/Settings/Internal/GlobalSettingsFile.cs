using System;

namespace R8EOX.Settings.Internal
{
    [Serializable]
    internal class GlobalSettingsFile
    {
        [SerializeField] private VideoSettings video = new VideoSettings();
        [SerializeField] private ProfileSettings profiles = new ProfileSettings();

        public VideoSettings Video { get => video; set => video = value; }
        public ProfileSettings Profiles { get => profiles; set => profiles = value; }
    }
}
