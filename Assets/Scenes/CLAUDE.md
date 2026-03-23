# Scenes

## Purpose
All Unity scene files (.unity).

## Conventions
- Scene names use PascalCase (e.g., `MainMenu.unity`, `Level1.unity`)
- Every scene must contain: Main Camera (tagged MainCamera), Directional Light, Global Volume
- Use the `/create-scene` skill for proper URP scene setup
- Save scenes after modifications: `mcp__UnityMCP__manage_scene(action="save")`

## Build Settings Order
0. `Boot.unity` — App entry point, creates persistent [AppRoot] with AppManager
1. `MainMenu.unity` — All menu screens (splash, main, mode select, track select, loading)
2+ Track scenes loaded dynamically via AppManager.LoadTrack()

## Contents
- `Boot.unity` — Build index 0. Single [AppRoot] GO with AppManager + DontDestroyOnLoad. Loads MainMenu on Start
- `MainMenu.unity` — Menu canvas with all screen panels (Splash, MainMenu, ModeSelect, TrackSelect, Loading). MenuManager coordinates navigation
- `OutpostTrack.unity` — Outdoor RC track with terrain, environment, SpawnGrid, all managers wired via SceneSetupBuilder
- `PhysicsTestTrack.unity` — Flat test track with waypoint path, obstacles, and PhysicsTestManager for vehicle physics debugging
