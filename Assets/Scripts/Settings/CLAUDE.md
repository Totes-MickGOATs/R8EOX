# Settings

## Purpose
Settings system — persists and applies all user preferences across sessions and profiles.

## Top-Level Class
- **SettingsManager.cs** (`R8EOX.Settings`) — MonoBehaviour on [AppRoot]. The sole public API for all settings reads, writes, and profile management.

## Internal Classes (`Assets/Scripts/Settings/Internal/`)
Data containers and I/O helpers. All use `namespace R8EOX.Settings.Internal` and `internal` access.

| File | Role |
|------|------|
| `SettingsData.cs` | Aggregate container for all sub-settings |
| `SettingsIO.cs` | Load/save from `Application.persistentDataPath` via `JsonUtility` |
| `VideoSettings.cs` | Display settings (resolution, vsync, quality tier, upscaling) |
| `AudioSettings.cs` | Volume levels (master, sfx, music) |
| `ControlsSettings.cs` | Input mappings, deadzones, input curve |
| `CalibrationSettings.cs` | Controller axis calibration (offsets, sensitivity, outer deadzone) |
| `GameplaySettings.cs` | HUD and debug overlay toggles |
| `ProfileSettings.cs` | Profile list and active profile name |
| `GlobalSettingsFile.cs` | JSON wrapper for global (cross-profile) settings |
| `ProfileSettingsFile.cs` | JSON wrapper for per-profile settings |
| `QualityTier.cs` | Enum: Ultra, High, Balanced, Performance |
| `UpscalingMode.cs` | Enum: None, FSR2 |
| `WindowMode.cs` | Enum for window/fullscreen modes |
| `VideoApplier.cs` | Applies VideoSettings to Unity engine (QualitySettings, Screen, etc.) |
| `AudioApplier.cs` | Applies AudioSettings to Unity mixer |

## Data Flow
`SettingsManager.Load()` → `SettingsIO.LoadAll(profile)` → `SettingsData`
`SettingsManager.Set*Settings(action)` → mutates data → `Save()` + `ApplyAll()` + `OnSettingsChanged`

## Profile Storage
- Global (video + profile list): `{persistentDataPath}/settings_global.json`
- Per-profile (audio, controls, calibration, gameplay): `{persistentDataPath}/profiles/{name}/settings.json`

## Conventions
- Other systems access settings ONLY through `SettingsManager` — never reference internal types directly
- `Set*Settings` takes an `Action<T>` modifier delegate — mutate in place, the manager handles save/apply
- `Get*Settings()` returns a `.Clone()` — callers cannot accidentally mutate live data
