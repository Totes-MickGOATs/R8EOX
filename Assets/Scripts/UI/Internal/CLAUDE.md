# UI Internal Components

## Purpose
Internal implementation for the UI system. Only `UIManager` should reference these.

## Conventions
- Namespace: `R8EOX.UI.Internal`
- Access: `internal class` (enforced by pre-commit hook)
- All are MonoBehaviours attached to UI GameObjects

## Contents
- `RaceHUD.cs` — Displays speed, position, lap counter, and current lap time during races
- `Leaderboard.cs` — Shows/hides race standings list
- `PauseMenu.cs` — Pause screen with resume/quit: toggles Time.timeScale between 0 and 1
- `VehiclePreviewRenderer.cs` — RenderTexture-based 3D vehicle preview with turntable, 3-point lighting, drag-to-rotate
- `VehicleStatsDisplay.cs` — Fill-bar display for vehicle stats (speed, acceleration, handling, weight) + description
- `VehicleListEntry.cs` — Single row in vehicle list: thumbnail, name, category badge, selection highlight
