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
No scripts yet — organized by system as they are created.
