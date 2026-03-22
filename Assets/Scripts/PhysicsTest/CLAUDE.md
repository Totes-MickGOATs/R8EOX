# PhysicsTest System

## Purpose
Physics test track system for observing and debugging vehicle physics in controlled, repeatable scenarios.

## Conventions
- Top-level: `PhysicsTestManager.cs` (namespace `R8EOX.PhysicsTest`)
- Internal components in `Internal/` (namespace `R8EOX.PhysicsTest.Internal`, `internal` access)

## Contents
- `Internal/WaypointPath.cs` — Closed-loop Catmull-Rom spline path from child transforms
- `Internal/PlaybackController.cs` — Wraps `Time.timeScale` for play/pause/speed/single-frame-step control
