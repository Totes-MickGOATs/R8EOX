# Unity 6 and URP Gotchas

## Unity 6 (6000.4.0f1)

### Deprecated APIs
- `Object.FindObjectOfType<T>()` — use `Object.FindFirstObjectByType<T>()` (but both are banned in this project — pass references through parent)
- `Object.FindObjectsOfType<T>()` — use `Object.FindObjectsByType<T>()` (also banned)
- Always verify APIs with `unity_reflect` before using them

### Assembly Definitions
- Unity 6 requires explicit assembly definitions for test assemblies
- EditMode tests: `Assets/Tests/EditMode/` with `.asmdef`
- PlayMode tests: `Assets/Tests/PlayMode/` with `.asmdef`

## URP 17.4.0

### Shader Paths
- NEVER guess shader names from training data
- Always search project assets: `manage_asset(action="search", filter_type="Shader")`
- Or verify with: `unity_reflect search "shader name"`
- URP shaders use `Universal Render Pipeline/` prefix (e.g., `Universal Render Pipeline/Lit`)

### Materials
- URP materials use URP-specific shaders — Built-in RP shaders will render pink/magenta
- When creating materials, verify the shader exists in the project first

### Pipeline Assets
- NEVER directly edit `*_RPAsset.asset`, `*_Renderer.asset`, or `UniversalRenderPipelineGlobalSettings`
- Use Unity Editor or `mcp__UnityMCP__manage_graphics` instead
- The PreToolUse hook blocks direct text edits to these files

### Post-Processing
- URP uses Volume components with Volume Profiles
- Default profile: `Assets/Settings/DefaultVolumeProfile.asset`
- Every scene should have a Global Volume referencing this profile

## Domain Reload

- After creating or modifying C# scripts, Unity recompiles ALL C# code
- Wait for `isCompiling` to be `false` before using new types
- Check `read_console` for compilation errors after any script change
- New components/types cannot be used until compilation succeeds

## Input System (1.19.0)

- Legacy Input Manager is NOT active in this project
- Pre-commit hook blocks `Input.GetKey`, `Input.GetAxis`, `Input.GetButton`
- Use `UnityEngine.InputSystem` namespace for all input
