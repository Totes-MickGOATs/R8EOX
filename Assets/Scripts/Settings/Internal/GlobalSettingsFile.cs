using System;

namespace R8EOX.Settings.Internal
{
    [Serializable]
    internal class GlobalSettingsFile
    {
        public VideoSettings video = new VideoSettings();
        public ProfileSettings profiles = new ProfileSettings();
    }
}
