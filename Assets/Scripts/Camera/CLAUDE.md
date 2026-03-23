# Camera System

## Purpose
Camera control — chase cam, cinematic views, replay, and screen effects.

## Conventions
- Top-level: `CameraManager.cs` (namespace `R8EOX.Camera`)
- Internal components in `Internal/` (namespace `R8EOX.Camera.Internal`, `internal` access)
- Camera follows vehicles through CameraManager — gets target from SessionManager via `SetTarget()`
- Camera updates in LateUpdate (after physics)
- Mode switching via `SwitchToFollowMode()` / `SwitchToCinematicMode()`

## Contents
- `CameraManager.cs` — Top-level API: switch modes, set target, trigger shake, configurable follow settings
- `CameraMode.cs` — Public enum: Follow, Cinematic
- `Internal/FollowCamera.cs` — Chase/follow camera with configurable offset, smooth damping, and look-ahead
- `Internal/CinematicCamera.cs` — Replay, flyover, spectator views (skeleton)
- `Internal/CameraShake.cs` — Impact/collision/speed shake effects with intensity decay
