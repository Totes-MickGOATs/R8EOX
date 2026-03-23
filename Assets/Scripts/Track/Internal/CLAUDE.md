# Track Internal Components

## Purpose
Internal implementation classes for the Track system. Only `TrackManager` should reference these.

## Conventions
- Namespace: `R8EOX.Track.Internal`
- Access: `internal class` (enforced by pre-commit hook)
- Checkpoint and TrackBoundary are MonoBehaviours (they use triggers/collisions)

## Contents
- `Centerline.cs` — MonoBehaviour [RequireComponent(SplineContainer)]: wraps com.unity.splines for centerline queries — position at distance, nearest point projection, distance along track, direction, curvature. Discovered by TrackManager via GetComponentInChildren
- `Checkpoint.cs` — MonoBehaviour with trigger collider: detects vehicle passage, notifies TrackManager via Action
- `TrackBoundary.cs` — MonoBehaviour with collision: applies bounce force when vehicles hit walls
- `SpawnPoint.cs` — MonoBehaviour marking vehicle spawn locations: serializes index, isPlayerSpawn flag; provides SpawnPointData DTO via ToData(); terrain-aware gizmos (red >2m below, orange >0.5m below terrain, with displacement line)
- `SpawnGrid.cs` — MonoBehaviour: configurable grid-based spawn layout with live editor gizmos; terrain-aware per-point coloring (red >2m, orange >0.5m below terrain) with displacement lines; single draggable object generates all spawn points from grid dimensions, spacing, fill order, stagger, and column/row grouping
- `SpawnGridMath.cs` — Pure static class: computes SpawnPointData[] from grid parameters; validates grouping sums; no MonoBehaviour dependencies (testable)
- `FillDirection.cs` — Enum: LeftToRight, RightToLeft (lateral fill order within grid rows)
- `RowOrder.cs` — Enum: FrontToBack, BackToFront (whether row 0 is front or back of grid)
- `StaggerMode.cs` — Enum: Alternating (odd rows offset), Cumulative (progressive offset per row)
- `GridGrouping.cs` — Serializable struct: int[] groupSizes + float gapBetweenGroups for splitting columns or rows into groups with extra spacing
- `TrackSurface.cs` — MonoBehaviour with SurfaceType enum (Asphalt/Dirt/Grass/Gravel): returns grip multiplier
