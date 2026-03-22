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
- `SpawnPoint.cs` — MonoBehaviour marking vehicle spawn locations: serializes index, isPlayerSpawn flag; provides SpawnPointData DTO via ToData(); draws Gizmo in editor (fallback when no SpawnGrid)
- `SpawnGrid.cs` — MonoBehaviour: configurable grid-based spawn layout with live editor gizmos; single draggable object generates all spawn points from grid dimensions, spacing, fill order, stagger, and column/row grouping
- `SpawnGridMath.cs` — Pure static class: computes SpawnPointData[] from grid parameters; validates grouping sums; no MonoBehaviour dependencies (testable)
- `FillDirection.cs` — Enum: LeftToRight, RightToLeft (lateral fill order within grid rows)
- `RowOrder.cs` — Enum: FrontToBack, BackToFront (whether row 0 is front or back of grid)
- `StaggerMode.cs` — Enum: Alternating (odd rows offset), Cumulative (progressive offset per row)
- `GridGrouping.cs` — Serializable struct: int[] groupSizes + float gapBetweenGroups for splitting columns or rows into groups with extra spacing
- `TrackSurface.cs` — MonoBehaviour with SurfaceType enum (Asphalt/Dirt/Grass/Gravel): returns grip multiplier
