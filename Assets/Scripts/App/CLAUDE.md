# App System

## Purpose
Application-level orchestration — owns the persistent root that survives all scene loads, drives the top-level state machine (Boot, Menu, Loading, InGame), and coordinates the session lifecycle from the menu layer down to the Session system.

## Conventions
- Top-level: `AppManager.cs` (namespace `R8EOX.App`)
- Internal components in `Internal/` (namespace `R8EOX.App.Internal`, `internal` access)
- AppManager is the ONLY cross-system entry point for the App system; MenuManager and UIManager call it directly

## Contents
- `AppManager.cs` — Top-level API: `DontDestroyOnLoad` root, `LoadTrack(TrackDefinition, SessionMode)`, `ReturnToMenu()`, progress/complete/error events; creates and owns the `[SessionManager]` child GO for each session
- `Internal/AppState.cs` — Enum: Boot, Menu, Loading, InGame
- `Internal/SceneLoader.cs` — Async and synchronous scene loading with timeout and progress callbacks

## Flow

```
AppManager.LoadTrack(trackDef, mode)
  └── SessionConfig.CreateRuntime(...)
  └── sessionChannel.SetSession(config)
  └── new [SessionManager] child GO
        └── sessionManager.SetSessionChannel(channel)
        └── sessionManager.BeginSession(config)
  └── SceneLoader.LoadSceneAsync(trackDef.SceneName, ...)
        └── Scene loads → SessionBootstrapper.Awake()
              └── finds [SessionManager] → calls sessionManager.OnSceneReady(...)
                    └── activeConfig already set → enters VehicleSelect or Spawn
```
