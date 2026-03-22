# Camera System

## Purpose
Camera control — chase cam, cinematic views, replay, and screen effects.

## Conventions
- Top-level: `CameraManager.cs` (namespace `R8EOX.Camera`)
- Internal components in `Internal/` (namespace `R8EOX.Camera.Internal`, `internal` access)
- Camera follows vehicles through CameraManager — gets target from RaceManager
- Camera updates in LateUpdate (after physics)

## Contents
- `CameraManager.cs` — Top-level API: switch modes, set target
- `Internal/FollowCamera.cs` — Chase/follow camera with smoothing
- `Internal/CinematicCamera.cs` — Replay, flyover, spectator views
- `Internal/CameraShake.cs` — Impact/collision/speed shake effects
