# UI System

## Purpose
HUD, menus, and all user interface elements.

## Conventions
- Top-level: `UIManager.cs` (namespace `R8EOX.UI`)
- Internal components in `Internal/` (namespace `R8EOX.UI.Internal`, `internal` access)
- UI receives data from other systems via UIManager — never queries systems directly
- UIManager polls RaceManager each frame for HUD data (push not needed)
- Uses TextMeshPro (TMPro) for all UI text elements
- Uses new InputSystem for pause toggle (Escape key)

## Contents
- `UIManager.cs` — Top-level API: show/hide UI, update displays, vehicle select overlay lifecycle, route vehicle swap requests, Escape-key pause toggle, pull-based HUD update from RaceManager
- `Internal/RaceHUD.cs` — TMPro-based HUD: speed (km/h), position (ordinal), lap counter, race time, lap time, best lap time, countdown display
- `Internal/Leaderboard.cs` — Race standings display (skeleton)
- `Internal/PauseMenu.cs` — Pause screen with resume/quit and "Change Vehicle" swap trigger; Time.timeScale pause
- `Internal/VehicleSelectOverlay.cs` — Full-screen vehicle selection overlay with list, preview, and confirm/back
- `Internal/VehicleListPanel.cs` — Scrollable vehicle list that spawns VehicleListEntry rows
- `Internal/VehiclePreviewPanel.cs` — Container for VehiclePreviewRenderer, VehicleStatsDisplay, and vehicle name
- `Internal/VehiclePreviewRenderer.cs` — RenderTexture-based 3D vehicle preview with turntable and drag-to-rotate
- `Internal/VehicleStatsDisplay.cs` — Fill-bar display for vehicle stats and description text
- `Internal/VehicleListEntry.cs` — Single row in vehicle selection list with thumbnail, name, and category
