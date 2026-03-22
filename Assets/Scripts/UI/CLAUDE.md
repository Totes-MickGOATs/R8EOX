# UI System

## Purpose
HUD, menus, and all user interface elements.

## Conventions
- Top-level: `UIManager.cs` (namespace `R8EOX.UI`)
- Internal components in `Internal/` (namespace `R8EOX.UI.Internal`, `internal` access)
- UI receives data from other systems via UIManager — never queries systems directly
- Use Unity UI Toolkit or uGUI (project decision TBD)

## Contents
- `UIManager.cs` — Top-level API: show/hide UI, update displays
- `Internal/RaceHUD.cs` — Speed, position, lap count during race
- `Internal/Leaderboard.cs` — Race standings display
- `Internal/PauseMenu.cs` — Pause screen, settings access
- `Internal/VehiclePreviewRenderer.cs` — RenderTexture-based 3D vehicle preview with turntable and drag-to-rotate
- `Internal/VehicleStatsDisplay.cs` — Fill-bar display for vehicle stats and description text
- `Internal/VehicleListEntry.cs` — Single row in vehicle selection list with thumbnail, name, and category
