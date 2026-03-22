# Race System

## Purpose
Race coordination — bridges Vehicle and Track systems, manages race state and standings.

## Conventions
- Top-level: `RaceManager.cs` (namespace `R8EOX.Race`)
- Internal components in `Internal/` (namespace `R8EOX.Race.Internal`, `internal` access)
- RaceManager is the ONLY system that coordinates between Vehicle and Track
- Uses FSM for race state (countdown → racing → finished)
- Configuration via `RaceConfig` ScriptableObject

## Contents
- `RaceManager.cs` — Top-level API: start/end race, coordinate systems
- `Internal/RaceState.cs` — State machine: countdown, racing, finished
- `Internal/Standings.cs` — Position tracking, lap counts per vehicle
- `Internal/RaceTimer.cs` — Lap times, best times, split tracking
