# VFX System

## Purpose
Visual effects — tire marks, exhaust, sparks, and screen-space effects driven by vehicle telemetry.

## Conventions
- Top-level: `VFXManager.cs` (namespace `R8EOX.VFX`)
- Internal components in `Internal/` (namespace `R8EOX.VFX.Internal`, `internal` access)
- VFXManager receives a vehicle target via `SetTarget(GameObject)`, caches VehicleManager, polls `GetTelemetry()` each LateUpdate
- Internal components never poll other systems — VFXManager routes all data
- Use object pooling for particle systems

## Contents
- `VFXManager.cs` — Top-level API: telemetry-driven VFX routing, manual override methods for external callers
- `Internal/TireMarks.cs` — TrailRenderer-based skid marks, one trail per wheel, slip-threshold activated
- `Internal/ExhaustEffect.cs` — Throttle-driven exhaust particles with backfire burst
- `Internal/SparkEffect.cs` — Collision spark particles positioned at impact point
- `Internal/ScreenEffects.cs` — Speed blur and damage vignette intensity (post-processing integration deferred)
