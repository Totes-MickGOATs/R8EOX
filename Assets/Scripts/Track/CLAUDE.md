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
- `SpawnPointData.cs` — Public struct (DTO) exposing spawn point data across system boundaries
- `TrackManager.cs` — Top-level API: track queries, checkpoint tracking; discovers SpawnGrid (preferred) or individual SpawnPoints
- `Internal/Centerline.cs` — MonoBehaviour wrapping SplineContainer (com.unity.splines): evaluates position, tangent, curvature, nearest point, distance along track
- `Internal/Checkpoint.cs` — Lap tracking, position detection triggers
- `Internal/SpawnPoint.cs` — Vehicle spawn location markers with index and player flag (fallback when no SpawnGrid)
- `Internal/SpawnGrid.cs` — Configurable grid-based spawn layout: dimensions, spacing, fill order, stagger, grouping; live editor gizmos
- `Internal/SpawnGridMath.cs` — Pure static math: computes grid positions from parameters, validates groupings
- `Internal/FillDirection.cs` — Enum: LeftToRight, RightToLeft (grid fill order within rows)
- `Internal/RowOrder.cs` — Enum: FrontToBack, BackToFront (grid row iteration order)
- `Internal/StaggerMode.cs` — Enum: Alternating, Cumulative (row stagger behavior)
- `Internal/GridGrouping.cs` — Serializable struct: group sizes + gap between groups (for column/row grouping)
- `Internal/TrackBoundary.cs` — Walls, barriers, out-of-bounds detection
- `Internal/TrackSurface.cs` — Surface types (asphalt, dirt, grass) and grip modifiers
