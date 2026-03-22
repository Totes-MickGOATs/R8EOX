# PhysicsTest Internal Components

## Purpose
Internal implementation for the PhysicsTest system. Only PhysicsTestManager references these.

## Conventions
- Namespace: `R8EOX.PhysicsTest.Internal`
- Access: `internal class` (enforced by pre-commit hook)

## Contents
- `WaypointPath.cs` — Closed-loop path with Catmull-Rom interpolation and editor gizmos
- `IWritableInput.cs` — Local writable input abstraction; allows PathFollower to command throttle/brake/steer without a cross-system internal dependency
- `PathFollower.cs` — Drives a vehicle along a WaypointPath; computes steering and throttle each FixedUpdate and writes to an IWritableInput
