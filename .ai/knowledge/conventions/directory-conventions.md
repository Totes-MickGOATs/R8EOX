# Directory Conventions

## Asset Structure

```
Assets/
├── Scenes/              # Scene files (.unity) — PascalCase names
├── Scripts/             # C# source code
│   ├── {System}/        # One folder per game system
│   │   ├── {System}Manager.cs     # Top-level class
│   │   └── Internal/              # Internal implementation
│   ├── Editor/          # Editor-only scripts (R8EOX.Editor namespace)
│   └── ScriptableObjects/  # ScriptableObject definitions
├── Prefabs/             # Reusable prefab assets
├── Materials/           # Materials and shaders
├── Art/                 # Textures, models, sprites
│   ├── Textures/
│   ├── Models/
│   └── Sprites/
├── Audio/               # Sound effects and music
│   ├── SFX/
│   └── Music/
├── Settings/            # URP pipeline assets, volume profiles (DO NOT edit directly)
└── Plugins/             # Third-party plugins, Roslyn analyzers
```

## Naming Rules

| Asset Type | Convention | Example |
|-----------|-----------|---------|
| Scenes | PascalCase | `MainMenu.unity`, `Level1.unity` |
| Scripts | PascalCase, matches class name | `PlayerController.cs` |
| Prefabs | PascalCase | `EnemySpawner.prefab` |
| Materials | PascalCase | `PlayerSkin.mat` |
| Textures | PascalCase with suffix | `PlayerSkin_Albedo.png`, `Ground_Normal.png` |
| Audio | PascalCase | `Explosion_Large.wav` |

## System Folder Organization

Each game system (Combat, Movement, UI, etc.) gets its own folder under `Assets/Scripts/`:
- The top-level class (manager/controller) sits directly in the system folder
- Internal implementation classes go in an `Internal/` subfolder
- Use `R8EOX.{System}` namespace for the top-level class
- Use `R8EOX.{System}.Internal` namespace + `internal` access for sub-components

## Path Conventions

- All paths in code are relative to `Assets/` unless stated otherwise
- Use forward slashes `/` for cross-platform compatibility
- Unity `.meta` files are auto-generated — never manually create or edit them

## CLAUDE.md Files

Every non-dot folder should have a `CLAUDE.md` describing its purpose and conventions. Update it when adding or significantly changing files in the folder.
