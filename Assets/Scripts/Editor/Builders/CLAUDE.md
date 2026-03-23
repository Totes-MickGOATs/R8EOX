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
| `BuggySpec.cs` | Readonly data struct holding all per-variant vehicle parameters |
| `BuggySpecCatalog.cs` | Static catalog with `Get2WD()` and `Get4WD()` spec factory methods |
| `BuggyMotorKind.cs` | Builder-local enum mirroring `MotorPreset` (avoids cross-system import) |
| `BuggyDriveLayout.cs` | Builder-local enum: RWD, AWD |
| `BuggyDiffType.cs` | Builder-local enum: Open, BallDiff, Spool |
| `RCBuggyBuilder.cs` | Builds buggy prefabs from BuggySpec; menu items for 2WD/4WD/All variants |
| `TerrainBuilder.cs` | Creates and configures terrain objects with surface layers and height maps |
| `TerrainLayerBuilder.cs` | Builds terrain layer assets (texture, normal map, tiling) |
| `TerrainSplatmapBuilder.cs` | Composites splatmap blend masks onto terrain alphamap |
| `TrackBuilder.cs` | Assembles complete track scenes from terrain, environment, and vehicle |
| `TrackFolderData.cs` | Data struct for scanned track folder contents (includes TrackConfig) |
| `TrackFolderScanner.cs` | Scans track asset folders by naming convention (discovers TrackConfig at track root) |
| `PhysicsTestTrackBuilder.cs` | Builds the PhysicsTestTrack scene: ground, obstacles, waypoints, lighting, camera, PhysicsTestManager |
| `BuggySpecExporter.cs` | Exports buggy specs, motor presets, and builder geometry to JSON for the viewer playground |
| `SceneSetupBuilder.cs` | Places all manager GameObjects into the active track scene and wires SessionBootstrapper refs; available as menu item and called by TrackBuilder |
| `VehicleSelectLayoutData.cs` | Shared color constants and RectTransform/ColorBlock helpers used by all Vehicle Select UI builders |
| `VehicleSelectLayoutBuilder.cs` | `R8EOX > Build Vehicle Select Layout` menu item: configures VehicleSelectOverlay prefab (Canvas, CanvasScaler, all child RectTransforms/colors/styles) and delegates to companion builders |
| `OverlayScrollViewBuilder.cs` | Configures the ScrollView, Viewport, Content, and Scrollbar Vertical inside ListPanel |
| `OverlayPreviewPanelBuilder.cs` | Configures VehicleName, PreviewImage, StatsDisplay (stat bars + labels + bg objects), and DescriptionText inside PreviewPanel |
| `VehicleListEntryBuilder.cs` | Configures VehicleListEntry prefab: LayoutElement height, Highlight/Thumbnail/NameText/CategoryText layout and styles |
| `MenuSceneBuilder.cs` | `R8EOX > Build Menu Scene` menu item: creates MainMenu.unity scene with Camera, Canvas, MenuManager, and all five screen panels (Splash, MainMenu, ModeSelect, TrackSelect, Loading) fully wired |

## Conventions

- Namespace: `R8EOX.Editor.Builders`
- All builders are `internal static` classes
- Materials use URP Lit shader (`Universal Render Pipeline/Lit`), never Standard
- Vehicle internal types accessed via `InternalsVisibleTo("R8EOX.Editor")`
- `BuggySpecSerializer.BuildBodyMeshes()` excludes control arms (name contains "Arm") — the viewer builds arms separately from `armY`/`armThickness`/`armDepth` fields
- `BuggySpecExporter` writes inline into `rc-buggy-viewer.html` between SPEC_DATA markers — there is no standalone JSON file
- When adding new constants to the shared JSON, add static accessors on the owning runtime class (e.g., `Drivetrain.DiffStiffnessConst`) rather than hardcoding values in the exporter
