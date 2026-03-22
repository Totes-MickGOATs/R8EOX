# Top-Down Systems Architecture

## Core Principle

Every call path must be traceable. Only top-level system components communicate with each other. Lower-level code has its calls marshalled by the parent system.

## Directory Structure

```
Assets/Scripts/
├── {System}/                    # One folder per system
│   ├── {System}Manager.cs       # TOP-LEVEL: the system's public API
│   └── Internal/                # INTERNAL: only used by the manager
│       ├── SubComponentA.cs
│       └── SubComponentB.cs
```

## Namespace Convention

```csharp
// Top-level — can reference other R8EOX.{System} namespaces
namespace R8EOX.Combat
{
    public class CombatManager : MonoBehaviour { }
}

// Internal — ONLY referenced within R8EOX.Combat
namespace R8EOX.Combat.Internal
{
    internal class DamageCalculator { }
}
```

The `Internal` sub-namespace + `internal` access modifier prevents cross-system access at the C# compiler level.

## Rules

1. Each system folder has exactly ONE top-level class (the manager/controller)
2. Internal classes are only called by their own system's top-level class
3. Cross-system communication happens ONLY between top-level classes
4. The top-level class marshals ALL external calls for its internals

## Approved Patterns (from Game Programming Patterns — Robert Nystrom)

| Pattern | Usage | Why |
|---------|-------|-----|
| **Command** | Input/AI produces Command objects → top-level coordinator dispatches | Parent holds decision-making; calls flow up then down |
| **Component (container-mediated)** | Components read/write shared state on container; container controls update order | No sibling coupling |
| **Update Method** | Top-level calls `Tick()` on sub-components explicitly (not auto-Update) | System controls when, order, pause/skip |
| **Subclass Sandbox** | Base class provides constrained protected operations | Parent acts as gatekeeper |
| **State (FSMs)** | Top-level owns the state machine; transitions are explicit | Traceable and auditable |

## Banned Patterns

| Pattern | Why Banned |
|---------|-----------|
| **Singleton (.Instance)** | Bypasses parent hierarchy — any code accesses anything |
| **SendMessage / BroadcastMessage** | String-based, invisible at compile time, untraceable |
| **FindObjectOfType** | Global reach — grabs objects without going through parent |
| **Observer (peer-to-peer)** | Any object subscribes to any other — parent doesn't know |
| **Global Event Queue** | Any system posts/listens — sender doesn't know receivers |

## Unity-Specific Adaptations

- Prefer explicit `Tick()` over auto-`Update()` — system managers call sub-components directly
- `GetComponent<T>()` only in the coordinator — leaf components receive references via injection
- ScriptableObjects for shared config — data flows down, not sideways
- No `static` events on MonoBehaviours — use Command pattern or direct calls through parent

## Enforcement

- **Pre-commit hook**: Blocks cross-system `.Internal` namespace imports, SendMessage, FindObjectOfType, .Instance
- **Code review agent**: Checks architectural compliance during review
- **C# compiler**: `internal` access modifier prevents cross-assembly access
