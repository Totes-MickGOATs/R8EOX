# Session System

## Purpose
Session orchestration — manages the lifecycle of play sessions (practice, race, time trial). Bridges the gap between "player wants to play" and "systems are running." Handles scene loading, vehicle spawning, track validation, and mode setup.

## Conventions
- Top-level: `SessionManager.cs` (namespace `R8EOX.Session`)
- Internal components in `Internal/` (namespace `R8EOX.Session.Internal`, `internal` access)
- SessionManager is the ONLY system that orchestrates cross-system setup (Track, Race, Vehicle, Camera, Audio, VFX, AI)
- Uses SessionChannel SO for editor-play vs full-flow detection
- Configuration via `SessionConfig` ScriptableObject

## Contents
- `SessionManager.cs` — Top-level API: begin/end session, vehicle selection overlay, mid-session vehicle swap, coordinate systems; wires AudioManager and VFXManager via SetTarget on player vehicle
- `Internal/SessionBootstrapper.cs` — Scene-resident: detects editor-play vs menu flow via SessionChannel; creates default Practice session in editor-play mode
- `Internal/SessionPhase.cs` — Enum: Idle, Loading, VehicleSelect, Spawning, Ready, Teardown
- `Internal/SessionState.cs` — FSM with validated phase transitions; supports VehicleSelect phase (Loading→VehicleSelect→Spawning) and mid-session car swap (Ready→VehicleSelect)
- `Internal/VehicleSpawner.cs` — Instantiates vehicle prefabs at spawn points with terrain safety
- `Internal/SpawnSafety.cs` — Pure static: terrain-safe spawn height correction
- `Internal/TrackValidator.cs` — Checks track readiness per mode and track type (TODO)
- `Internal/TrackReadiness.cs` — Struct: readiness flags and missing-component report
