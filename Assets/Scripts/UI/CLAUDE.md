# UI System

## Purpose
HUD, menus, and all user interface elements.

## Conventions
- Top-level: `UIManager.cs` (namespace `R8EOX.UI`)
- Internal components in `Internal/` (namespace `R8EOX.UI.Internal`, `internal` access)
- UI receives data from other systems via UIManager — never queries systems directly
- Use Unity UI Toolkit or uGUI (project decision TBD)

## Contents
- `UIManager.cs` — Top-level API: show/hide UI, update displays, vehicle select overlay lifecycle, route vehicle swap requests
- `Internal/RaceHUD.cs` — Speed, position, lap count during race
- `Internal/Leaderboard.cs` — Race standings display
- `Internal/PauseMenu.cs` — Pause screen with resume/quit and "Change Vehicle" swap trigger
- `Internal/VehicleSelectOverlay.cs` — Full-screen vehicle selection overlay with list, preview, and confirm/back
- `Internal/VehicleListPanel.cs` — Scrollable vehicle list that spawns VehicleListEntry rows
- `Internal/VehiclePreviewPanel.cs` — Container for VehiclePreviewRenderer, VehicleStatsDisplay, and vehicle name
- `Internal/VehiclePreviewRenderer.cs` — RenderTexture-based 3D vehicle preview with turntable and drag-to-rotate
- `Internal/VehicleStatsDisplay.cs` — Fill-bar display for vehicle stats and description text
- `Internal/VehicleListEntry.cs` — Single row in vehicle selection list with thumbnail, name, and category
