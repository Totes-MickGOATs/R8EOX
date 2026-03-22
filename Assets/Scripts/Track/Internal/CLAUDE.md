# Track Internal Components

## Purpose
Internal implementation classes for the Track system. Only `TrackManager` should reference these.

## Conventions
- Namespace: `R8EOX.Track.Internal`
- Access: `internal class` (enforced by pre-commit hook)
- Checkpoint and TrackBoundary are MonoBehaviours (they use triggers/collisions)

## Contents
- `Centerline.cs` — Spline-based track center path: point sampling, nearest point projection, total length
- `Checkpoint.cs` — MonoBehaviour with trigger collider: detects vehicle passage, notifies TrackManager via Action
- `TrackBoundary.cs` — MonoBehaviour with collision: applies bounce force when vehicles hit walls
- `TrackSurface.cs` — MonoBehaviour with SurfaceType enum (Asphalt/Dirt/Grass/Gravel): returns grip multiplier
