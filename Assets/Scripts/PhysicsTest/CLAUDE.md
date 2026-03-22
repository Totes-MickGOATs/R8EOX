# PhysicsTest System

## Purpose
Physics test track system for observing and debugging vehicle physics in controlled, repeatable scenarios.

## Conventions
- Top-level: `PhysicsTestManager.cs` (namespace `R8EOX.PhysicsTest`)
- Internal components in `Internal/` (namespace `R8EOX.PhysicsTest.Internal`, `internal` access)

## Contents
- `PhysicsTestManager.cs` — Top-level coordinator: spawns/wires vehicle, PathFollower, PlaybackController, DebugOverlay; exposes `JumpToSegment(int)` public API
- `Internal/WaypointPath.cs` — Closed-loop Catmull-Rom spline path from child transforms
- `Internal/IWritableInput.cs` — Local writable input abstraction for PathFollower (avoids cross-system internal import)
- `Internal/InputAdapter.cs` — Bridges R8EOX.Input.Internal.ScriptedInput to IWritableInput without a cross-system using directive
- `Internal/PathFollower.cs` — Drives a vehicle along WaypointPath; writes throttle/brake/steer each FixedUpdate
- `Internal/PlaybackController.cs` — Wraps `Time.timeScale` for play/pause/speed/single-frame-step control
- `Internal/DebugOverlay.cs` — IMGUI debug HUD: playback controls, vehicle telemetry, per-wheel diagnostics
