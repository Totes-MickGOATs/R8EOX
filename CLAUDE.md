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
- **Boot.unity** (build index 0) — App entry point. Creates persistent `[AppRoot]` with `AppManager`, loads MainMenu
- **MainMenu.unity** (build index 1) — All menu screens on one Canvas. `MenuManager` drives navigation
- **OutpostTrack.unity** / **PhysicsTestTrack.unity** — Track scenes loaded async via `AppManager.LoadTrack()`
- Editor-play still works: open any track scene, press Play — `SessionBootstrapper` creates a default Practice session

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

- **Spec-to-Runtime Contract**: `BuggySpec` is the source of truth for all vehicle parameters. The pipeline is: BuggySpec → RCBuggyBuilder (serializes onto prefab) → VehicleManager (reads serialized values at runtime) → BuggySpecExporter (exports to viewer HTML). Runtime code must NEVER override builder-serialized values with hardcoded defaults — trust the prefab data.
- **Viewer Data**: `rc-buggy-viewer.html` contains inline JSON (`BUGGY_SPECS`) auto-generated by `R8EOX > Export Viewer Data`. Never edit the SPEC_DATA block manually. Run `R8EOX > Build All Buggies` to rebuild prefabs AND re-export in one step.
- **Domain Reload Timing**: After modifying editor scripts, Unity must recompile before menu items use the new code. If `R8EOX > Export Viewer Data` produces stale output, call `refresh_unity` first, then re-run the menu item.
- **Terrain.SampleHeight()**: Returns height relative to terrain origin — always add `terrain.transform.position.y` for world-space Y
- **FindAnyObjectByType vs FindObjectOfType**: Use `FindAnyObjectByType<T>()` (allowed in editor scripts). `FindObjectOfType` is banned project-wide
- **URP Shaders**: Always search project assets for actual shader names before referencing them — do NOT guess shader paths from training data
- **Domain Reload**: After script changes, Unity recompiles all C#. Wait for `isCompiling` to be false before using new types
- **Input System**: This project uses `com.unity.inputsystem` (1.19.0). The legacy Input Manager is NOT active
- **Camera.main**: Calls `FindGameObjectWithTag` internally — cache the result, never use in Update loops
- **Test Runner**: Use `mcp__UnityMCP__run_tests` or Window > General > Test Runner. Tests use NUnit (`com.unity.test-framework`)
- **PhysicsMaterial assets**: Use `.asset` extension, NOT `.physicsMaterial` — Unity 6 warns on `CreateAsset()` with the old extension
- **Test files and top-down lint**: `Assets/Tests/` is exempt from the cross-system `using R8EOX.X.Internal` lint rule. If tests still fail the hook, use fully-qualified type names instead of `using` directives
- **Friction circle**: `WheelForceSolver` applies `FrictionCircleMath` after computing lateral/longitudinal forces. Throttle reduces cornering grip naturally — no special 2WD/4WD code needed. Don't add drive-type-specific handling; it emerges from the friction ellipse.
- **Air physics scales**: `WheelInertiaConfig` defaults are tuned for 1/10th RC scale (MoI=0.006, GyroScale=1.5, ReactionScale=30). Don't revert to the old 80x/2500x values — those caused floaty air behavior.
- **Editor asmdef + URP types**: `R8EOX.Editor.asmdef` must reference `Unity.RenderPipelines.Core.Runtime` and `Unity.RenderPipelines.Universal.Runtime` for editor builders that use `VolumeProfile`, `Bloom`, etc. The Runtime asmdef already has these; the Editor one was missing them until the PostProcessBuilder was added
- **FindObjectsByType (Unity 6)**: `FindObjectsByType<T>(FindObjectsSortMode.None)` is deprecated in Unity 6. Use `FindObjectsByType<T>(FindObjectsInactive.Exclude)` instead — no sort mode parameter
- **EnvironmentBuilder auto-creates sun**: `SetupDirectionalLight` creates a Directional Light if none exists in the scene (rotation 50/-30/0). It also searches all `Light` components as a fallback before creating, so renamed lights are found too
- **TrackFolderData optionals**: `EnvironmentSettingsAsset`, `TrackConfigAsset`, and `SkyboxHdrPath` can all be null. Downstream builder calls must null-guard these — `PostProcessBuilder` does not handle a null settings parameter
- **Builders own their scenes**: Every builder (`BootSceneBuilder`, `MenuSceneBuilder`, `TrackBuilder`, `PhysicsTestTrackBuilder`) calls `EditorSceneManager.NewScene(EmptyScene)` at the start and `SaveScene()` at the end. They never modify the previously-active scene. If a new builder is added, it must follow this pattern — otherwise "Build All" will pollute whatever scene is open
- **MCP multi-instance**: When E2E test editor is running, two editors connect to MCP. All agents MUST call `set_active_instance` before MCP calls or they'll error. Read `mcpforunity://instances` to discover connected editors
- **Vehicle destroy order**: Always `SetActive(false)` before `Object.Destroy` on vehicles. AudioManager/VFXManager poll `GetTelemetry()` in LateUpdate and throw MissingReferenceException on destroyed Rigidbody
- **FindAssets("t:CustomSO") unreliable**: Unity's `FindAssets` with `t:` filter can miss custom ScriptableObjects during domain reload. TrackFolderScanner has name-based and direct-path fallbacks
- **Builder defaults must match SO defaults**: TrackBuilder/TerrainBuilder fallback constants must match TerrainSettings SO defaults (100x2x100). Drift between code defaults and SO values causes silent terrain size bugs
- **OnValidate fires during AddComponent**: Builder code can't set serialized fields before OnValidate runs. Don't put warnings in OnValidate for fields that builders wire post-creation
- **Test console = quality bar**: Treat ALL console warnings/errors during test runs as real bugs requiring root-cause fixes, not symptoms to suppress with LogAssert.ignoreFailingMessages
- **TMP resources in git**: `Assets/TextMesh Pro/` must be committed — worktrees and fresh clones need it. The folder is exempt from project lint rules in `is_unity_generated()`

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
| Quality gate | `unity-e2e-tester` | After any wave of build agents completes — before reporting "done" to user |

### Execution Pattern

1. **Research first** — if there are unknowns, dispatch `unity-researcher` before building
2. **Build in parallel** — launch `unity-script-writer` and `unity-scene-builder` simultaneously when independent
3. **Integrate after compilation** — wire new components into scenes only after scripts compile cleanly
4. **E2E quality check** — dispatch `unity-e2e-tester` against test editor to catch issues early
5. **Test and review** — dispatch `unity-tester` and `unity-reviewer` after code is written
6. **Always consolidate** — check `read_console` and summarize results after all agents complete

### MCP Instance Routing

When the E2E test editor is running, **two Unity editors are connected to MCP**. All agents MUST pin to the correct instance or MCP calls will error.

- **Before dispatching any MCP-using agent**, read `mcpforunity://instances` to get the current dev editor's instance ID
- **Include the instance ID in every agent prompt**: `Pin to instance R8EOX@{hash} before any MCP calls`
- The `unity-e2e-tester` handles its own instance pinning (it pins to the test editor, not the dev editor)
- If only one instance is connected, pinning is optional but harmless

### Key Rules

- Launch independent agents in **parallel** (multiple Agent calls in one message)
- Each agent prompt must be **self-contained** — include all context it needs
- For complex tasks, use `/do` which automates the wave-based dispatch pattern
- The main session coordinates — agents implement
- The orchestrator pre-reads shared context (CLAUDE.md, folder CLAUDE.md, `.ai/knowledge/` docs) ONCE, then injects relevant content directly into each agent prompt — agents start immediately without re-reading files
- Break tasks into the most granular pieces possible and launch maximum parallel agents

### IMMEDIATE COMMITS — MANDATORY FOR ALL CLAUDE ACTIVITY

> **BLOCKING RULE — DO NOT SKIP — DO NOT DEFER — DO NOT BATCH**
>
> **"Agent" means ANY Claude activity: main session, orchestrator, subagent, dispatched agent — anything Claude does.** If Claude edited a file, Claude commits it immediately. No role is exempt. No distinction matters. If you changed a file, commit it now.
>
> Every agent MUST `git commit` **immediately after each file is written, modified, or updated.** Violation = lost work.

#### The Rule

**One file changed = one immediate commit.** Not "commit when done." Not "commit at the end." IMMEDIATELY after EACH file change.

```bash
# After writing/modifying a file:
git status  # ← ALWAYS check for companion .meta files!
git add path/to/ExactFile.cs
git add path/to/ExactFile.cs.meta   # ← REQUIRED for new files (Unity auto-generates these)
git commit -m "feat: add ExactFile with purpose description"

# Then continue to the next file. Repeat for EVERY file.
```

#### ⚠️ UNITY .meta FILES — THE #1 MISSED ITEM

Unity generates a `.meta` file for **every** new file and folder. If you create `Foo.cs`, Unity creates `Foo.cs.meta`. If you create folder `Bar/`, Unity creates `Bar.meta`. **These must be committed alongside their parent file.** Without `.meta` files, Unity loses GUID references — causing magenta materials, missing scripts, and broken prefab links. A `.cs` commit without its `.meta` is an incomplete commit.

#### What This Means — COMMIT BEFORE PROCEEDING

**You are BLOCKED from starting work on the next file until the current file is committed.** The workflow is strictly sequential per file:

1. Write or edit `Foo.cs`
2. `git status` — check for `Foo.cs.meta` (will exist if `Foo.cs` is new)
3. `git add Foo.cs Foo.cs.meta && git commit -m "feat: ..."` — **STOP HERE until commit succeeds**
4. Only NOW may you proceed to `Bar.cs`
5. `git add Bar.cs Bar.cs.meta && git commit -m "feat: ..."` — **STOP HERE until commit succeeds**
6. Only NOW may you proceed to the next file

This applies to ALL file types: `.cs`, `.unity`, `.asset`, `.meta`, `CLAUDE.md`, everything.

**If a commit fails, you MUST fix the issue and retry. You CANNOT skip the commit and move on.**

#### Banned

- `git add -A` / `git add .` / `git add --all` — **BANNED** (stages other agents' work)
- Batching multiple files into one commit — **BANNED**
- Deferring commits to "after all work is done" — **BANNED**
- Silently skipping a failed commit — **BANNED** (report the error instead)
- Using `--no-verify` — **BANNED** (pre-commit hook must validate every commit)

#### No Exemptions — This Means YOU

Do not mentally categorize your own edits as "just config" or "just docs." If you used Edit, Write, or Bash to change a file, you commit it before touching another file. There is no role, context, or rationale that exempts any Claude activity from this rule.

When dispatching agents, the orchestrator MUST ALSO:
1. Include this commit rule **verbatim** in every agent prompt
2. After each agent completes, verify commits happened via `git log`
3. After each agent completes, run `git status` to check for orphaned `.meta` files the agent missed — commit them immediately
4. If an agent returned without committing, flag it as a failure

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
