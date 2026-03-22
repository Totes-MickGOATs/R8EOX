# Editor/Builders/

## Purpose
Editor-only builder scripts for constructing vehicles, terrain, and track scenes programmatically.

## Files

| File | Purpose |
|------|---------|
| `AssemblyInfo.cs` | InternalsVisibleTo declarations for the Editor assembly |
| `AssetHelper.cs` | Utility methods for loading and creating assets in editor scripts |
| `EnvironmentBuilder.cs` | Constructs environment elements (lighting, skybox, post-processing) |
| `LayerData.cs` | Data struct for terrain layer configuration |
| `RCBuggyBuilder.cs` | Builds the RCBuggy hierarchy: chassis, wheels, colliders, visuals, components |
| `TerrainBuilder.cs` | Creates and configures terrain objects with surface layers and height maps |
| `TerrainLayerBuilder.cs` | Builds terrain layer assets (texture, normal map, tiling) |
| `TerrainSplatmapBuilder.cs` | Composites splatmap blend masks onto terrain alphamap |
| `TrackBuilder.cs` | Assembles complete track scenes from terrain, environment, and vehicle |
| `TrackFolderData.cs` | Data struct for scanned track folder contents |
| `TrackFolderScanner.cs` | Scans track asset folders by naming convention |

## Conventions

- Namespace: `R8EOX.Editor.Builders`
- All builders are `internal static` classes
- Materials use URP Lit shader (`Universal Render Pipeline/Lit`), never Standard
- Vehicle internal types accessed via `InternalsVisibleTo("R8EOX.Editor")`
