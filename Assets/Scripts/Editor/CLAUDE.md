# Editor

## Purpose
Editor-only scripts excluded from runtime builds by Unity's convention.

## Conventions
- Namespace: `R8EOX.Editor` (NOT `R8EOX`)
- Must use `using UnityEditor;`
- Never create circular dependencies between editor and runtime scripts
- Place custom inspectors, editor windows, property drawers, and build tools here
- See `Builders/` subfolder for programmatic scene/prefab construction

## Contents
- `Builders/` — Editor builder scripts for constructing vehicles, terrain, and scenes programmatically
- `AddBuggyMaterials.cs` — Menu item to create persistent URP Lit material assets and assign them to RCBuggy prefab meshes
- `OutpostTrackSetup.cs` — Menu item to create/configure the OutpostTrack scene
- `TrackBuilder.cs` — Assembles complete track scenes from terrain, environment, and vehicle
- `BuildSettingsValidator.cs` — InitializeOnLoad + menu items: validates runtime scenes are in Build Settings, removes ghost entries, auto-fixes
- `SpawnPointValidator.cs` — Menu item (R8EOX > Validate Spawn Points): checks all spawn points against terrain height; reports errors (>2m below) and warnings (>0.5m below) to console
