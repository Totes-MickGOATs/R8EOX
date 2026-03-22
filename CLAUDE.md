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
