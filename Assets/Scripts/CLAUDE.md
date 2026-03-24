# Scripts

## Purpose
All runtime C# scripts for the game, organized by system following a strict top-down architecture.

## Conventions
- **Namespace**: `R8EOX` for runtime, `R8EOX.Editor` for editor, `R8EOX.Tests` for tests
- **One class per file** — class name matches file name exactly
- **300 line maximum** per file — enforced by pre-commit hook
- **[SerializeField] private** over public fields
- Use `/create-script` skill for proper scaffolding

## Top-Down Architecture

Each game system gets its own folder. Only top-level classes communicate across systems.

```
Scripts/
├── {System}/                    # One folder per system
│   ├── {System}Manager.cs       # TOP-LEVEL: the system's public API (R8EOX.{System})
│   └── Internal/                # Internal implementation
│       └── SubComponent.cs      # R8EOX.{System}.Internal + internal access modifier
```

### Rules
1. Each system has ONE top-level class — the manager/controller
2. Internal classes use `namespace R8EOX.{System}.Internal` + `internal` access
3. Cross-system communication: ONLY between top-level classes
4. Lower-level components NEVER call into other systems directly

### Banned
- Singleton `.Instance` — use dependency injection through parent
- `SendMessage` / `BroadcastMessage` — use direct method calls
- `FindObjectOfType` — pass references through parent system
- Peer-to-peer Observer — only top-level classes subscribe to events

### Approved Patterns
- Command (input → coordinator → actor)
- Component with container-mediated state
- Explicit `Tick()` called by parent (not auto-Update)
- Subclass Sandbox (constrained protected operations)
- State machines owned by top-level class

## Contents
- `App/` — Application lifecycle: persistent root (AppManager), scene loading, DontDestroyOnLoad management
- `Menu/` — Menu system: screen navigation, splash, main menu, mode select, track select, loading
- `Vehicle/` — RC car physics: suspension, drivetrain, grip, handling (complete)
- `Input/` — Player input routing via new Input System, IVehicleInput interface (complete)
- `Track/` — Track definition: spawn points, checkpoints, centerline, boundaries, surfaces
- `Race/` — Race coordination: FSM, standings, timer (skeleton)
- `Session/` — Session orchestration: editor-play detection, vehicle spawning, track validation, mode degradation
- `Settings/` — User preferences: persistence, quality tiers, audio volumes, input calibration, profiles
- `Camera/` — Camera control: chase cam, cinematic views, screen effects (skeleton)
- `UI/` — HUD, menus, user interface, pause menu, vehicle select overlay
- `Audio/` — Engine, tire, ambient, music (skeleton)
- `AI/` — AI opponents: centerline-following drivers with difficulty scaling, wired via SessionManager
- `VFX/` — Tire marks, exhaust, sparks, screen effects (skeleton)
- `ScriptableObjects/` — Data containers: SessionConfig, TrackConfig, RaceConfig, TrackDefinition, TrackRegistry, MenuThemeConfig, motor/suspension/traction configs
- `Diagnostics/` — Runtime diagnostics: flow tracing, event logging, deferred destroy verification, IMGUI overlay (fully stripped in release builds)
- `Editor/` — Builder tools: TrackBuilder, RCBuggyBuilder, MenuSceneBuilder, BootSceneBuilder, TrackRegistryBuilder
