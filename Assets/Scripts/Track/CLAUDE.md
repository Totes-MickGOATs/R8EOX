# Track System

## Purpose
Track/circuit definition — path, checkpoints, boundaries, and surface types.

## Conventions
- Top-level: `TrackManager.cs` (namespace `R8EOX.Track`)
- Internal components in `Internal/` (namespace `R8EOX.Track.Internal`, `internal` access)
- Track is agnostic to Vehicle — never import `R8EOX.Vehicle.Internal`
- Track data is scene-specific; reusable elements use prefabs
- Configuration via `TrackConfig` ScriptableObject

## Contents
- `TrackManager.cs` — Top-level API: track queries, checkpoint tracking
- `Internal/Centerline.cs` — Spline/path defining track center
- `Internal/Checkpoint.cs` — Lap tracking, position detection triggers
- `Internal/TrackBoundary.cs` — Walls, barriers, out-of-bounds detection
- `Internal/TrackSurface.cs` — Surface types (asphalt, dirt, grass) and grip modifiers
