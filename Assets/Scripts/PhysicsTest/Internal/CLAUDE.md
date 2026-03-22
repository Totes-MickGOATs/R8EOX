# PhysicsTest Internal Components

## Purpose
Internal implementation for the PhysicsTest system. Only PhysicsTestManager references these.

## Conventions
- Namespace: `R8EOX.PhysicsTest.Internal`
- Access: `internal class` (enforced by pre-commit hook)

## Contents
- `WaypointPath.cs` — Closed-loop path with Catmull-Rom interpolation and editor gizmos
