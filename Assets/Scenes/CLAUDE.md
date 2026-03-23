# Scenes

## Purpose
All Unity scene files (.unity).

## Conventions
- Scene names use PascalCase (e.g., `MainMenu.unity`, `Level1.unity`)
- Every scene must contain: Main Camera (tagged MainCamera), Directional Light, Global Volume
- Use the `/create-scene` skill for proper URP scene setup
- Save scenes after modifications: `mcp__UnityMCP__manage_scene(action="save")`

## Contents
- `OutpostTrack.unity` — Outdoor RC track with terrain, environment, SpawnGrid, all managers wired via SceneSetupBuilder
- `PhysicsTestTrack.unity` — Flat test track with waypoint path, obstacles, and PhysicsTestManager for vehicle physics debugging
