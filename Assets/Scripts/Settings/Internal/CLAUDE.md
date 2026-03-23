# Settings/Internal

## Purpose
Internal implementation types for the Settings system.
All use `namespace R8EOX.Settings.Internal` and `internal` access.
Nothing here is accessible outside the Settings system.

## Files

### Data Containers
| File | Role |
|------|------|
| `SettingsData.cs` | Aggregate container for all sub-settings |
| `VideoSettings.cs` | Display settings (resolution, vsync, quality tier, render scale, upscaling) |
| `AudioSettings.cs` | Volume levels (master, sfx, music) |
| `ControlsSettings.cs` | Input mappings, deadzones, input curve |
| `CalibrationSettings.cs` | Controller axis calibration (offsets, sensitivity, outer deadzone) |
| `GameplaySettings.cs` | HUD and debug overlay toggles |
| `ProfileSettings.cs` | Profile list and active profile name |

### Enums
| File | Values |
|------|--------|
| `QualityTier.cs` | Ultra, High, Balanced, Performance |
| `WindowMode.cs` | Fullscreen, BorderlessFullscreen, Windowed |
| `UpscalingMode.cs` | None, FSR2 |

### I/O
| File | Role |
|------|------|
| `GlobalSettingsFile.cs` | JSON wrapper for global (cross-profile) settings |
| `ProfileSettingsFile.cs` | JSON wrapper for per-profile settings |
| `SettingsIO.cs` | Load/save from `Application.persistentDataPath` via `JsonUtility` |

### Appliers
| File | Role |
|------|------|
| `VideoApplier.cs` | Applies `VideoSettings` to URP asset and `Screen`/`QualitySettings`/`Application`. Caller must inject a `Volume` reference for motion blur toggling. SSAO toggled via `ScriptableRendererData.TryGetRendererFeature<ScreenSpaceAmbientOcclusion>()`. |
| `AudioApplier.cs` | Applies `AudioSettings` to `AudioListener` volume; caches sfx/music volumes for `AudioManager` to poll. |

## Conventions
- All types are `internal` — never referenced by systems outside `Settings/`
- Appliers are static — no state, called from `SettingsManager`
- `VideoApplier.Apply(settings, volume)` requires a caller-injected `Volume` for motion blur; pass `null` to skip gracefully
