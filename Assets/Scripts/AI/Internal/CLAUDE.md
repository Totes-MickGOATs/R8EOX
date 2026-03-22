# AI Internal Components

## Purpose
Internal implementation for the AI system. Only `AIManager` should reference these.

## Conventions
- Namespace: `R8EOX.AI.Internal`
- Access: `internal class` (enforced by pre-commit hook)
- AI drives vehicles through AIManager -> VehicleManager, never directly

## Contents
- `AIDriver.cs` — Per-vehicle AI: calculates throttle/brake/steering from racing line and behavior
- `RacingLine.cs` — Waypoint-based optimal path: tracks current target, advances on arrival
- `AIBehavior.cs` — Difficulty-driven modifiers: aggression, consistency, overtake/draft decisions
