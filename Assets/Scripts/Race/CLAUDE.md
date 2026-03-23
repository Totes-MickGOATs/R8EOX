# Race System

## Purpose
Race coordination — bridges Vehicle and Track systems, manages race state and standings.

## Conventions
- Top-level: `RaceManager.cs` (namespace `R8EOX.Race`)
- Internal components in `Internal/` (namespace `R8EOX.Race.Internal`, `internal` access)
- RaceManager is the ONLY system that coordinates between Vehicle and Track
- Uses FSM for race state (countdown -> racing -> finished)
- Configuration via `RaceConfig` ScriptableObject
- SessionManager calls `Initialize(trackManager)`, `RegisterVehicle()`, `StartRace()`, `Tick()`, `EndRace()`

## Contents
- `RacePhase.cs` — Public enum: PreRace, Countdown, Racing, Finished (shared across systems for UI)
- `RaceManager.cs` — Top-level API: start/end race, coordinate systems, checkpoint handling, standings queries, timing queries; fires OnPhaseChanged and OnLapCompleted events
- `Internal/RaceState.cs` — State machine: countdown timer, phase transitions (PreRace/Countdown/Racing/Finished)
- `Internal/Standings.cs` — Position tracking: lap counts, checkpoint progress, distance-based position recalculation; fixes lap counting via previous-checkpoint validation
- `Internal/RaceTimer.cs` — Lap times: elapsed race time, per-vehicle current/last/best lap times
