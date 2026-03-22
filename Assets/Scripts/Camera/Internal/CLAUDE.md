# Camera Internal Components

## Purpose
Internal implementation for the Camera system. Only `CameraManager` should reference these.

## Conventions
- Namespace: `R8EOX.Camera.Internal`
- Access: `internal class` (enforced by pre-commit hook)

## Contents
- `FollowCamera.cs` — Chase cam with configurable offset, smooth damping, and look-ahead
- `CinematicCamera.cs` — Scripted camera positions with smooth transitions, always looks at target
- `CameraShake.cs` — Intensity/duration-based shake: returns random offset with decay over time
