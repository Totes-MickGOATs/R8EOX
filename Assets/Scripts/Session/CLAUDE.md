# Session System

## Purpose
Session orchestration — manages the lifecycle of play sessions (practice, race, time trial). Bridges the gap between "player wants to play" and "systems are running." Handles scene loading, vehicle spawning, track validation, and mode setup.

## Conventions
- Top-level: `SessionManager.cs` (namespace `R8EOX.Session`)
- Internal components in `Internal/` (namespace `R8EOX.Session.Internal`, `internal` access)
- SessionManager is the ONLY system that orchestrates cross-system setup (Track, Race, Vehicle, Camera, AI)
- Uses SessionChannel SO for editor-play vs full-flow detection
- Configuration via `SessionConfig` ScriptableObject

## Contents
- `SessionManager.cs` — Top-level API: begin/end session, coordinate systems (TODO)
- `Internal/SessionBootstrapper.cs` — Scene-resident: detects editor-play vs session flow (TODO)
- `Internal/SessionPhase.cs` — Enum: Idle, Loading, Spawning, Ready, Teardown
- `Internal/SessionState.cs` — FSM with validated phase transitions (TODO)
- `Internal/VehicleSpawner.cs` — Instantiates vehicle prefabs at spawn points with terrain safety (TODO)
- `Internal/SpawnSafety.cs` — Pure static: terrain-safe spawn height correction (TODO)
- `Internal/TrackValidator.cs` — Checks track readiness per mode and track type (TODO)
- `Internal/TrackReadiness.cs` — Struct: readiness flags and missing-component report
