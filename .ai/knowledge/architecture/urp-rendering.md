# URP Rendering Architecture

## Pipeline Setup

This project uses Universal Render Pipeline (URP) 17.4.0 with separate quality tiers:

- **PC**: `Assets/Settings/PC_RPAsset.asset` + `Assets/Settings/PC_Renderer.asset`
- **Mobile**: `Assets/Settings/Mobile_RPAsset.asset` + `Assets/Settings/Mobile_Renderer.asset`
- **Global settings**: `Assets/Settings/UniversalRenderPipelineGlobalSettings.asset`

## Volume Profiles

- `Assets/Settings/DefaultVolumeProfile.asset` — default post-processing profile
- Every scene should have a Global Volume referencing this profile

## Scene Requirements

Every scene must include:
1. **Main Camera** — tagged `MainCamera`, with URP camera component
2. **Directional Light** — rotation `(50, -30, 0)` for default sun angle
3. **Global Volume** — referencing `DefaultVolumeProfile.asset`

## Shader Rules

- ALWAYS search project assets for actual shader names before referencing them
- Do NOT guess shader paths from training data — use `unity_reflect` or `manage_asset(action="search", filter_type="Shader")`
- URP shaders live under `Universal Render Pipeline/` prefix

## Important

- NEVER directly edit pipeline assets via text — use Unity Editor or MCP tools
- The PreToolUse hook blocks direct edits to `*_RPAsset.asset`, `*_Renderer.asset`, and `UniversalRenderPipelineGlobalSettings`
- To change render settings, use `mcp__UnityMCP__manage_graphics`
