# Track Contract

Defines what every track gets automatically, what each track must provide, and what's optional.

R8EOX is an RC simulator / vehicle physics sandbox with racing. This contract is generic — not locked to any racing sub-genre.

## Tier 1 — Fully Automatic (zero work per track)

Systems that work on any track scene with no manual setup, powered by shared SOs and runtime spawning:

- Session bootstrap (SessionBootstrapper + SessionChannel)
- Vehicle spawning at spawn points (SessionManager + VehicleSpawner)
- Vehicle selection overlay (SessionChannel)
- Input routing (global InputSystem)
- Chase camera (shared SO config)
- Race HUD, leaderboard, pause menu (reads RaceManager data)
- Engine/collision audio (reads vehicle data)
- Tire marks, surface particles, screen effects (reads vehicle + surface data)

**Rule:** If it applies to every track identically, it lives on a shared SO or spawns at runtime. Never a per-scene serialized field.

## Tier 2 — Required Per-Track

Without these, racing gameplay breaks. Every track must provide them.

| Infrastructure | Discovery | Why Required |
|---|---|---|
| Spawn infrastructure | SpawnGrid or SpawnPoint components | No spawn → no vehicles |
| Centerline spline | SplineContainer on Centerline component | No centerline → no positions, no AI, no wrong-way, no minimap, no resets |
| Checkpoints (4-6 min) | Checkpoint trigger colliders, ordered by index | No checkpoints → no laps, no shortcut prevention. Checkpoint[0] = finish line |
| TrackConfig SO | Referenced by SessionConfig | Metadata for race logic |

## Tier 3 — Recommended (graceful degradation if absent)

| Infrastructure | Default If Absent |
|---|---|
| Track boundaries | No OOB detection; vehicles drive anywhere |
| Surface zones | Grip = 1.0 everywhere |
| Track width data | Fixed default or derived via raycasts |
| Respawn/recovery points | Nearest centerline point + up offset |

## Tier 4 — Optional (declare if present, ignored if absent)

Sector timing markers, penalty zones, start gate animation, jump metadata, pit lane spline, weather override, ambient audio zones.

## Tier 5 — Auto-Derived (computed, no authoring)

Lap counting, race positions, wrong-way detection, race state machine, AI pathing, AI braking zones, minimap, surface-dependent audio/VFX, reset destinations, track length.

## Centerline

Uses `com.unity.splines` package (v2.8.x). SplineContainer component with Auto Smooth knots. Author in Unity scene view or import from Blender (mesh vertices → FitSplineToPoints).

The centerline is a **logic reference**, not a racing line. Position tracking projects each vehicle onto it for distance-along-track. Lateral offset doesn't affect position ranking.
