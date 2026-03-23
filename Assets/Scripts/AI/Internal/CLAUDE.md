# AI Internal Components

## Purpose
Internal implementation for the AI system. Only `AIManager` should reference these.

## Conventions
- Namespace: `R8EOX.AI.Internal`
- Access: `internal class` (enforced by pre-commit hook)
- AI drives vehicles through AIManager -> VehicleManager, never directly

## Contents
- `AIDriver.cs` — Per-vehicle AI: speed-scaled lookahead, curvature-based throttle/brake, dot-product steering; writes to ScriptedInput each Tick
- `RacingLine.cs` — Centerline wrapper around TrackManager: lookahead points with lateral offset, curvature queries, distance projection
- `AIBehavior.cs` — Difficulty-driven modifiers: aggression (0-1), consistency, max speed factor, brake sensitivity, overtake/draft decisions
