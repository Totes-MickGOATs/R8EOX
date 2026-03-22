# Agent Dispatch Guide

## Available Agents

| Agent | Domain | When to Use |
|-------|--------|-------------|
| `unity-scene-builder` | Scenes, GameObjects, hierarchy, prefabs, lighting, cameras, UI | Creating or modifying scene content |
| `unity-script-writer` | C# scripts, components, MonoBehaviours, ScriptableObjects | Writing or modifying code |
| `unity-reviewer` | Code review, best practices, bug detection | After code is written, needs review |
| `unity-researcher` | API lookup, documentation, feasibility | Need to verify how something works |
| `unity-tester` | Unit tests, play mode tests, test execution | Writing or running tests |

## Dispatch Rules

1. **Research first** — if there are unknowns, dispatch `unity-researcher` before building
2. **Build in parallel** — launch `unity-script-writer` and `unity-scene-builder` simultaneously when independent
3. **Wait for compilation** — scripts must compile before scene work can reference new components
4. **Test and review** — dispatch `unity-tester` and `unity-reviewer` after code is written
5. **Check console** — always `read_console` after all agents complete

## Maximum Parallelism

- Launch as many independent agents as possible in a single message
- Break work into granular, specific tasks
- Each agent prompt must be self-contained with all context

## Pre-Read & Inject Pattern

Before dispatching agents, the orchestrator should:
1. Read shared context (CLAUDE.md, folder CLAUDE.md, relevant .ai/knowledge/ docs) ONCE
2. Paste relevant content directly into each agent prompt
3. Include exact file paths and content expectations
4. Agents start immediately — no file discovery needed

## Agent Prompt Template

```
## Context (pre-loaded — do NOT re-read these files)
### Project Conventions
{namespace, code style, banned patterns, line limit}
### Folder Rules
{from local CLAUDE.md}
### Reference
{from relevant .ai/knowledge/ doc}

## Your Task
{exact files to create/modify with full paths}
{content expectations or diffs}
{what to verify after completion}
```

## Dependency Ordering

- Research → Scripts (research informs code decisions)
- Scripts → Scene (new components must compile first)
- Scripts → Tests (need code to test against)
- Independent scripts can run in parallel
- Scene work independent of new scripts can run alongside script writing
