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
- `TrackConfig.cs` — Track configuration (includes TrackType)
- `TrackType.cs` — Enum: Circuit, PointToPoint
- `SessionMode.cs` — Enum: Practice, Race, TimeTrial
- `SessionConfig.cs` — Session parameters (mode, track, vehicle, laps, AI, countdown, time limit)
- `SessionChannel.cs` — Runtime data channel for sharing active SessionConfig and VehicleRegistry between systems
- `RaceConfig.cs` — Race configuration
- `EnvironmentSettings.cs` — Skybox, fog, ambient light, and sun settings
- `TerrainSettings.cs` — Terrain dimensions and resolution settings
- `LayerSettings.cs` — Terrain layer tile size and surface properties
- `VehicleCategory.cs` — Enum: Buggy, Truck, Stadium, Custom
- `VehicleStats.cs` — Serializable struct: normalized stats for UI display (speed, acceleration, handling, weight)
- `VehicleDefinition.cs` — Vehicle metadata: name, description, thumbnail, prefab reference, category, stats
- `VehicleRegistry.cs` — Collection of VehicleDefinition refs + overlay prefab reference for vehicle selection UI
- `MenuThemeConfig.cs` — Menu visual theme data: colors (background, accents, danger, text), TMP font refs, animation timing
- `TrackDefinition.cs` — Track metadata: name, description, thumbnail, scene name, TrackType, supported SessionModes, locked state
- `TrackRegistry.cs` — Collection of TrackDefinition refs; GetAll, GetDefault, FindBySceneName, Count
