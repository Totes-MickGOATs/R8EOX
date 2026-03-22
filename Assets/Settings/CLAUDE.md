# Settings

## Purpose
URP render pipeline assets and volume profiles that control rendering quality and post-processing.

## Conventions
- NEVER edit these files directly via text — use Unity Editor or MCP tools
- The PreToolUse hook blocks direct edits to pipeline assets
- To change render settings, use `mcp__UnityMCP__manage_graphics`
- When creating new scenes, reference `DefaultVolumeProfile.asset` for the Global Volume

## Contents
- `Mobile_RPAsset.asset` / `Mobile_Renderer.asset` — Mobile quality tier
- `PC_RPAsset.asset` / `PC_Renderer.asset` — PC/desktop quality tier
- `DefaultVolumeProfile.asset` — Post-processing profile for Global Volumes
- `UniversalRenderPipelineGlobalSettings.asset` — URP global config
