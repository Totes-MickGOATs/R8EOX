# Assets

## Purpose
Root Unity asset folder. All game content, scripts, and resources live here.

## Conventions
- All paths in code are relative to this folder (e.g., `Scenes/MainMenu`)
- Use forward slashes `/` for cross-platform compatibility
- Unity `.meta` files are auto-generated — never manually edit them
- Every subfolder should have its own CLAUDE.md describing its contents

## Contents
- `Scenes/` — Scene files (.unity)
- `Scripts/` — C# source code organized by system
- `Prefabs/` — Reusable prefab assets
- `Materials/` — Materials and shaders
- `Art/` — Textures, models, sprites
- `Audio/` — Sound effects and music
- `Settings/` — URP pipeline assets and volume profiles (DO NOT edit directly)
- `Plugins/` — Third-party plugins and Roslyn analyzers
