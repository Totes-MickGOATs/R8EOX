# Materials

## Purpose
Materials and shader assets for rendering.

## Conventions
- PascalCase names (e.g., `PlayerSkin.mat`, `GroundTerrain.mat`)
- Always verify shader exists in project before assigning — use `manage_asset(action="search", filter_type="Shader")`
- URP shaders use `Universal Render Pipeline/` prefix
- Never use Built-in RP shaders — they render pink/magenta in URP

## Contents
- `Vehicle/` — RC Buggy vehicle materials (6 URP Lit color-only materials: DarkGrey, MediumGrey, BlueSolid, BlueSemi, BlackTire, WhiteHub)
