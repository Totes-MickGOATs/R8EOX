# ScriptableObjects

## Purpose
ScriptableObject class definitions. Asset instances created from these go in appropriate content folders.

## Conventions
- Always include `[CreateAssetMenu]` attribute
- Use `fileName = "New{TypeName}"` and `menuName = "R8EOX/{TypeName}"`
- Namespace: `R8EOX`
- ScriptableObjects are data containers — no MonoBehaviour lifecycle methods

## Contents
- `MotorPresetConfig.cs` — Predefined motor tuning parameters (engine force, max speed, brake force)
- `SuspensionConfig.cs` — Suspension spring strength, damping, rest distance
- `TractionConfig.cs` — Traction and grip curve configuration
- `WheelInertiaConfig.cs` — Wheel rotational inertia parameters
- `TrackConfig.cs` — Track configuration
- `RaceConfig.cs` — Race configuration
- `EnvironmentSettings.cs` — Skybox, fog, ambient light, and sun settings
- `TerrainSettings.cs` — Terrain dimensions and resolution settings
- `LayerSettings.cs` — Terrain layer tile size and surface properties
