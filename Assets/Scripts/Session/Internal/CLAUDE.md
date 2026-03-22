# Session Internal Components

## Purpose
Internal implementation classes for the Session system. Only `SessionManager` should reference these.

## Conventions
- Namespace: `R8EOX.Session.Internal`
- Access: `internal class` (enforced by pre-commit hook)

## Contents
- `SessionPhase.cs` — Enum: Idle/Loading/VehicleSelect/Spawning/Ready/Teardown session lifecycle phases
- `TrackReadiness.cs` — Struct: flags for spawn points, checkpoints, centerline, finish trigger; readiness queries per mode + track type; missing-component report
- `SessionState.cs` — FSM with validated phase transitions; supports VehicleSelect phase (Loading→VehicleSelect→Spawning) and mid-session car swap (Ready→VehicleSelect)
- `SessionBootstrapper.cs` — Scene-resident MonoBehaviour: detects editor-play vs menu flow via SessionChannel; creates default Practice session in editor-play mode
- `VehicleSpawner.cs` — Instantiates vehicle prefabs at SpawnPointData positions with terrain safety
- `SpawnSafety.cs` — Pure static class: raycast terrain height, compute safe spawn Y
- `TrackValidator.cs` — Validates track components against mode requirements (TODO)
