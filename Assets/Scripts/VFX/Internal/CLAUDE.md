# VFX Internal Components

## Purpose
Internal implementation for the VFX system. Only `VFXManager` should reference these.

## Conventions
- Namespace: `R8EOX.VFX.Internal`
- Access: `internal class` (enforced by pre-commit hook)
- All are MonoBehaviours using particle systems or mesh rendering

## Contents
- `TireMarks.cs` — Renders skid mark decals on track surface with configurable width and material
- `ExhaustEffect.cs` — Throttle-driven exhaust particles with backfire burst on gear shifts
- `SparkEffect.cs` — Collision spark particles: positions at impact point, emits based on intensity
- `ScreenEffects.cs` — Post-processing effects: speed blur and damage vignette intensity control
