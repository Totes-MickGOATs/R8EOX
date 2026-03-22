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

## Convention-Over-Configuration

Prefer folder structure as config. Builders and tools discover files by **naming convention** rather than requiring manual references in ScriptableObjects or code.

- Builders scan folders and find files by known names (e.g., `diffuse.jpg`, `normal.png`, `arm.jpg`)
- Folder names encode metadata (e.g., `0_DirtBase/` — numeric prefix = ordering)
- Only settings that **cannot** be inferred from files use lightweight config SOs (tile size, fog density, dimensions)
- Config SOs have sensible defaults — if the `.asset` file is missing, the builder uses defaults
- Generated output goes in a `Generated/` subfolder to separate source from build artifacts
- New content = duplicate a folder + replace files. No code changes needed.

### Track Structure

```
Assets/Tracks/{TrackName}/
├── Environment/                    # Background (skybox, atmosphere)
│   ├── skybox.hdr                  # .hdr file = skybox HDRI
│   └── EnvironmentSettings.asset   # Fog, ambient, sun tuning
├── Terrain/                        # Ground geometry + textures
│   ├── heightmap.raw               # Raw heightmap file
│   ├── TerrainSettings.asset       # Dimensions, resolutions
│   ├── Layers/
│   │   ├── 0_{LayerName}/          # Base layer (no blend mask needed)
│   │   │   ├── diffuse.*           # PBR textures by convention
│   │   │   ├── normal.*
│   │   │   ├── arm.*
│   │   │   └── LayerSettings.asset # Optional: tile size, metallic, smoothness
│   │   └── 1_{LayerName}/
│   │       ├── diffuse.*, normal.*, arm.*
│   │       ├── blend-mask.*        # Optional: white=visible, black=transparent
│   │       └── LayerSettings.asset
│   └── Macro/
│       ├── normal-map.*
│       └── specular-map.*
└── Generated/                      # Builder output (not hand-edited)
```

## Path Conventions

- All paths in code are relative to `Assets/` unless stated otherwise
- Use forward slashes `/` for cross-platform compatibility
- Unity `.meta` files are auto-generated — never manually create or edit them

## CLAUDE.md Files

Every non-dot folder should have a `CLAUDE.md` describing its purpose and conventions. Update it when adding or significantly changing files in the folder.
