#if UNITY_EDITOR
namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Immutable data describing a single terrain layer discovered
    /// inside a track's Terrain/Layers/ folder.
    /// </summary>
    internal sealed class LayerData
    {
        internal int Index { get; }
        internal string Name { get; }
        internal string DiffusePath { get; }
        internal string NormalPath { get; }
        internal string ArmPath { get; }
        internal LayerSettings LayerSettingsAsset { get; }
        internal string BlendMaskPath { get; }

        internal LayerData(
            int index,
            string name,
            string diffusePath,
            string normalPath,
            string armPath,
            LayerSettings layerSettings,
            string blendMaskPath)
        {
            Index = index;
            Name = name;
            DiffusePath = diffusePath;
            NormalPath = normalPath;
            ArmPath = armPath;
            LayerSettingsAsset = layerSettings;
            BlendMaskPath = blendMaskPath;
        }
    }
}
#endif
