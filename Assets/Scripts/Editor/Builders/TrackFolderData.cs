#if UNITY_EDITOR
using System.Collections.Generic;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Immutable data produced by TrackFolderScanner describing
    /// all assets discovered in a convention-based track folder.
    /// </summary>
    internal sealed class TrackFolderData
    {
        internal string TrackName { get; }
        internal string HeightmapPath { get; }
        internal TerrainSettings TerrainSettingsAsset { get; }
        internal string BlendMaskPath { get; }
        internal IReadOnlyList<LayerData> Layers { get; }
        internal string SkyboxHdrPath { get; }
        internal EnvironmentSettings EnvironmentSettingsAsset { get; }
        internal string GeneratedFolder { get; }

        internal TrackFolderData(
            string trackName,
            string heightmapPath,
            TerrainSettings terrainSettings,
            string blendMaskPath,
            IReadOnlyList<LayerData> layers,
            string skyboxHdrPath,
            EnvironmentSettings environmentSettings,
            string generatedFolder)
        {
            TrackName = trackName;
            HeightmapPath = heightmapPath;
            TerrainSettingsAsset = terrainSettings;
            BlendMaskPath = blendMaskPath;
            Layers = layers;
            SkyboxHdrPath = skyboxHdrPath;
            EnvironmentSettingsAsset = environmentSettings;
            GeneratedFolder = generatedFolder;
        }
    }
}
#endif
