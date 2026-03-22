# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

R8EOX is a fresh Unity 6 project (version 6000.4.0f1) using the **Universal Render Pipeline (URP)**.

## Unity MCP Integration

This project includes the `com.coplaydev.unity-mcp` package. Use the UnityMCP tools (prefixed `mcp__UnityMCP__`) to interact with the Unity Editor directly — creating GameObjects, editing scripts, managing scenes, reading console output, etc. Always check `read_console` after script changes to verify compilation.

## Build & Run

- **Open in Unity Hub** — Unity 6000.4.0f1 (LTS)
- **Build target:** Standalone macOS (configurable in Build Settings)
- **Run tests:** Use `mcp__UnityMCP__run_tests` or Unity Test Runner (Window > General > Test Runner)

## Architecture

### Rendering
- URP with separate render pipeline assets for **Mobile** (`Assets/Settings/Mobile_RPAsset.asset`) and **PC** (`Assets/Settings/PC_RPAsset.asset`)
- Volume profiles in `Assets/Settings/`

### Key Packages
- `com.unity.render-pipelines.universal` (17.4.0) — URP rendering
- `com.unity.inputsystem` (1.19.0) — new Input System
- `com.unity.ai.navigation` (2.0.12) — NavMesh/AI navigation
- `com.unity.timeline` (1.8.12) — Timeline animation
- `com.unity.test-framework` (1.6.0) — NUnit-based testing
- `com.coplaydev.unity-mcp` — MCP for Unity integration

### Scenes
- None yet — create scenes as needed in `Assets/Scenes/`

## Conventions

- All asset paths are relative to `Assets/` unless stated otherwise
- Use forward slashes in paths for cross-platform compatibility
- Editor-only scripts go in `Editor/` folders (excluded from builds)
- After creating or modifying C# scripts, wait for domain reload and check console for compilation errors before proceeding

## Documentation

- Deep reference docs live in `.ai/knowledge/` — read `README.md` for the index, then load only relevant docs
- Every non-dot folder has a `CLAUDE.md` describing its contents — update it when adding/changing files
- See `.ai/knowledge/tooling/linting-rules.md` for the full linting rule reference

## Architecture — Top-Down Systems

This project enforces a **top-down architecture**. Every call path must be traceable.

- `Assets/Scripts/` is organized by **system** (e.g., `Combat/`, `Movement/`, `UI/`)
- Each system has ONE top-level class (manager/controller) — its public API
- Internal classes use `namespace R8EOX.{System}.Internal` + `internal` access modifier
- Cross-system communication happens ONLY between top-level classes
- Lower-level components NEVER directly call into other systems — their parent marshals all external calls
- Enforced by: namespace conventions, `internal` access, pre-commit hook, code review agent
- **Banned**: Singleton `.Instance`, `SendMessage`, `BroadcastMessage`, `FindObjectOfType`
- **Approved patterns**: Command, Component (container-mediated), Update Method (explicit Tick), Subclass Sandbox, State
- See `.ai/knowledge/architecture/top-down-systems.md` for full details and examples

## File Size Limit

**STRICT 300 line maximum** per `.cs` file. No exceptions for project code.
- Does NOT apply to: Unity-generated files (`Library/`, `Packages/`, `ProjectSettings/`, `Plugins/Roslyn/`)
- Enforced by: pre-commit hook (blocking) + code review agent
- If a file approaches 300 lines, refactor into smaller, focused classes

## Linting

- **Pre-commit hook**: Runs automatically on `git commit`. Checks staged `.cs` files for line count (300 max), namespace, legacy input, public fields, one-type-per-file, top-down violations, banned patterns (SendMessage, FindObjectOfType, Singleton). Checks CLAUDE.md and agent/skill docs for required structure.
- **Setup**: Run `bash tools/install-hooks.sh` after cloning (symlinks hook to `.git/hooks/pre-commit`)
- **Bypass**: `git commit --no-verify` (emergency only)
- **.editorconfig**: Enforces formatting and naming conventions in Rider/VS — PascalCase for public members, camelCase for privates/locals, I-prefix for interfaces

## Code Style

- **Namespace**: All scripts use `R8EOX` (editor scripts use `R8EOX.Editor`)
- **Naming**: PascalCase for classes/methods/properties, camelCase for local variables and parameters
- **Fields**: Prefer `[SerializeField] private` over `public` for Inspector-exposed fields
- **One class per file** — class name must match file name
- **Input**: Use the new Input System (`UnityEngine.InputSystem`), NOT legacy `Input.GetKey`/`Input.GetAxis`

## Directory Structure

```
Assets/
├── Scenes/          # All scene files
├── Scripts/         # Runtime C# scripts
│   ├── Editor/      # Editor-only scripts (excluded from builds)
│   └── ScriptableObjects/  # ScriptableObject definitions
├── Prefabs/         # Reusable prefab assets
├── Materials/       # Materials and shaders
├── Art/             # Textures, models, sprites
├── Audio/           # Sound effects and music
├── Settings/        # URP pipeline assets and volume profiles
└── Plugins/         # Third-party plugins (Roslyn, etc.)
```

## Gotchas

- **URP Shaders**: Always search project assets for actual shader names before referencing them — do NOT guess shader paths from training data
- **Domain Reload**: After script changes, Unity recompiles all C#. Wait for `isCompiling` to be false before using new types
- **Input System**: This project uses `com.unity.inputsystem` (1.19.0). The legacy Input Manager is NOT active
- **Camera.main**: Calls `FindGameObjectWithTag` internally — cache the result, never use in Update loops
- **Test Runner**: Use `mcp__UnityMCP__run_tests` or Window > General > Test Runner. Tests use NUnit (`com.unity.test-framework`)

## Orchestration

For any non-trivial task, use the specialized subagents in `.claude/agents/` instead of doing everything in the main session. This enables parallel execution and keeps work focused.

### Agent Dispatch Rules

| Task Type | Agent | When to Use |
|-----------|-------|-------------|
| Scene construction | `unity-scene-builder` | Creating/modifying GameObjects, hierarchy, prefabs, lighting, cameras, UI |
| Script writing | `unity-script-writer` | Creating/modifying C# scripts, components, ScriptableObjects |
| Code review | `unity-reviewer` | After writing code — catches null refs, performance issues, convention violations |
| Research | `unity-researcher` | Before building — verify APIs, look up docs, check feasibility |
| Testing | `unity-tester` | Writing or running NUnit tests |

### Execution Pattern

1. **Research first** — if there are unknowns, dispatch `unity-researcher` before building
2. **Build in parallel** — launch `unity-script-writer` and `unity-scene-builder` simultaneously when independent
3. **Integrate after compilation** — wire new components into scenes only after scripts compile cleanly
4. **Test and review** — dispatch `unity-tester` and `unity-reviewer` after code is written
5. **Always consolidate** — check `read_console` and summarize results after all agents complete

### Key Rules

- Launch independent agents in **parallel** (multiple Agent calls in one message)
- Each agent prompt must be **self-contained** — include all context it needs
- For complex tasks, use `/do` which automates the wave-based dispatch pattern
- The main session coordinates — agents implement
- The orchestrator pre-reads shared context (CLAUDE.md, folder CLAUDE.md, `.ai/knowledge/` docs) ONCE, then injects relevant content directly into each agent prompt — agents start immediately without re-reading files
- Break tasks into the most granular pieces possible and launch maximum parallel agents

### Subagent Commits (MANDATORY)

**Every subagent MUST commit its own work before finishing.** This is not optional. Uncommitted work from subagents gets lost or causes conflicts.

- Stage ONLY the files you created or modified — list them explicitly by path
- **NEVER** use `git add -A`, `git add .`, or `git add --all`
- Commit with: `git commit -m "feat: {what you did}"`
- If the commit fails, report the error — do not silently skip
- The orchestrator must include this instruction in every agent prompt

### File Conflict Prevention

**NEVER dispatch parallel agents that may write to the same file.** Before launching a wave of agents, the orchestrator MUST:

1. **Map each agent's write set** — list every file each agent will create or modify (scripts, scenes, CLAUDE.md files, .meta files, etc.)
2. **Check for overlaps** — if two agents share ANY file in their write sets, they CANNOT run in the same wave
3. **Serialize conflicting agents** — run them in sequential waves instead, passing the first agent's output as context to the next

Common conflict scenarios to watch for:
- Two `unity-script-writer` agents editing the same `.cs` file (e.g., adding methods to the same manager)
- A `unity-script-writer` and `unity-scene-builder` both updating the same folder's `CLAUDE.md`
- Multiple agents staging and committing at the same time — each agent commits ONLY its own files by name, never `git add -A` or `git add .`

When in doubt, **serialize rather than parallelize** — a slightly slower build is better than merge conflicts or lost work.
