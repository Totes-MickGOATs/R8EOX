namespace R8EOX.Vehicle.Internal
{
    /// <summary>Motor turn rating presets matching real RC motor specifications.</summary>
    internal static class MotorPresetRegistry
    {
        public static readonly MotorData[] Presets =
        {
            new MotorData(15.5f, 13.2f,  8.5f, 2.0f, 13f, 3.0f),  // 21.5T
            new MotorData(18.0f, 15.3f,  9.9f, 2.5f, 20f, 4.0f),  // 17.5T
            new MotorData(26.0f, 22.1f, 14.3f, 3.0f, 27f, 5.5f),  // 13.5T
            new MotorData(34.0f, 28.9f, 18.7f, 3.5f, 34f, 7.0f),  // 9.5T
            new MotorData(44.0f, 37.4f, 24.2f, 4.0f, 44f, 9.0f),  // 5.5T
            new MotorData(56.0f, 47.6f, 30.8f, 5.0f, 56f, 12.0f), // 1.5T
        };

        /// <summary>Returns preset data for the given preset, or null for Custom.</summary>
        public static MotorData? Get(MotorPreset preset)
        {
            int idx = (int)preset;
            if (preset == MotorPreset.Custom || idx < 0 || idx >= Presets.Length)
                return null;
            return Presets[idx];
        }
    }
}
