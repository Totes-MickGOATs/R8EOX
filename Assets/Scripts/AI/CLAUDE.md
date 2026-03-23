# AI System

## Purpose
AI-controlled opponents — pathfinding, decision making, and racing behavior.

## Conventions
- Top-level: `AIManager.cs` (namespace `R8EOX.AI`)
- Internal components in `Internal/` (namespace `R8EOX.AI.Internal`, `internal` access)
- AI drives vehicles through VehicleManager — never controls physics directly
- AI uses TrackManager for racing line data — never accesses Track internals

## Architecture
AI vehicles use the same prefab as the player. AIManager swaps RCInput for ScriptedInput, then each AIDriver reads the centerline via TrackManager, calculates throttle/brake/steer based on curvature and lookahead, and writes to ScriptedInput. VehicleManager reads ScriptedInput via the IVehicleInput interface — it never knows the vehicle is AI-controlled.

## Contents
- `AIManager.cs` — Top-level API: register AI drivers (swaps input), tick all drivers, difficulty scaling (1-10)
- `Internal/AIDriver.cs` — Per-vehicle AI: speed-scaled lookahead, curvature-based throttle/brake, dot-product steering
- `Internal/RacingLine.cs` — Centerline wrapper: lookahead points via TrackManager, lateral offset for line variation, curvature queries
- `Internal/AIBehavior.cs` — Difficulty-driven modifiers: aggression, consistency, max speed factor, brake sensitivity, overtake/draft decisions
