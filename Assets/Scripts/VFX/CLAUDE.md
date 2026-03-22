# VFX System

## Purpose
Visual effects — tire marks, exhaust, sparks, and screen-space effects.

## Conventions
- Top-level: `VFXManager.cs` (namespace `R8EOX.VFX`)
- Internal components in `Internal/` (namespace `R8EOX.VFX.Internal`, `internal` access)
- VFX receives events from other systems via VFXManager — never polls
- Use object pooling for particle systems

## Contents
- `VFXManager.cs` — Top-level API: trigger effects, manage pools
- `Internal/TireMarks.cs` — Skid marks rendered on track surface
- `Internal/ExhaustEffect.cs` — Exhaust particles, backfire visuals
- `Internal/SparkEffect.cs` — Collision spark particles
- `Internal/ScreenEffects.cs` — Speed blur, damage vignette, rain drops
