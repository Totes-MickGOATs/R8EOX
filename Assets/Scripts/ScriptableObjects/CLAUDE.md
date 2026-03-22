# ScriptableObjects

## Purpose
ScriptableObject class definitions. Asset instances created from these go in appropriate content folders.

## Conventions
- Always include `[CreateAssetMenu]` attribute
- Use `fileName = "New{TypeName}"` and `menuName = "R8EOX/{TypeName}"`
- Namespace: `R8EOX`
- ScriptableObjects are data containers — no MonoBehaviour lifecycle methods

## Contents
- `VehicleConfig.cs` — Vehicle tuning: engine, transmission, chassis, wheels, handling
- `TrackConfig.cs` — Track configuration
- `RaceConfig.cs` — Race configuration
- `EnvironmentSettings.cs` — Skybox, fog, ambient light, and sun settings
- `TerrainSettings.cs` — Terrain dimensions and resolution settings
- `LayerSettings.cs` — Terrain layer tile size and surface properties
