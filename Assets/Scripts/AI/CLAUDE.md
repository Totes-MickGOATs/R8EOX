# AI System

## Purpose
AI-controlled opponents — pathfinding, decision making, and racing behavior.

## Conventions
- Top-level: `AIManager.cs` (namespace `R8EOX.AI`)
- Internal components in `Internal/` (namespace `R8EOX.AI.Internal`, `internal` access)
- AI drives vehicles through VehicleManager — never controls physics directly
- AI uses TrackManager for racing line data — never accesses Track internals

## Contents
- `AIManager.cs` — Top-level API: manage AI drivers, difficulty
- `Internal/AIDriver.cs` — Per-car AI decision making and input generation
- `Internal/RacingLine.cs` — Optimal path calculation from track data
- `Internal/AIBehavior.cs` — Aggression, drafting, overtaking strategies
