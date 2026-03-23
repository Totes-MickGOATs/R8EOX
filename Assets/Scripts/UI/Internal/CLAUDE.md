# UI Internal Components

## Purpose
Internal implementation for the UI system. Only `UIManager` should reference these.

## Conventions
- Namespace: `R8EOX.UI.Internal`
- Access: `internal class` (enforced by pre-commit hook)
- All are MonoBehaviours attached to UI GameObjects
- TMPro text fields use `[SerializeField] private TextMeshProUGUI`

## Contents
- `RaceHUD.cs` — TMPro-based race HUD: speed (km/h), position (ordinal), lap counter, race time, current lap time, best lap time, countdown overlay; includes FormatTime and GetOrdinal helpers
- `Leaderboard.cs` — Shows/hides race standings list (skeleton)
- `PauseMenu.cs` — Pause screen with resume/quit and "Change Vehicle" trigger; routes swap through UIManager; controls Time.timeScale
- `VehicleSelectOverlay.cs` — Full-screen vehicle selection overlay: wires VehicleListPanel + VehiclePreviewPanel, confirm/back buttons, persists last selection via PlayerPrefs
- `VehicleListPanel.cs` — Scrollable vehicle list: spawns VehicleListEntry rows from VehicleDefinition[], reports selection changes
- `VehiclePreviewPanel.cs` — Container for VehiclePreviewRenderer + VehicleStatsDisplay + vehicle name label
- `VehiclePreviewRenderer.cs` — RenderTexture-based 3D vehicle preview with turntable, 3-point lighting, drag-to-rotate
- `VehicleStatsDisplay.cs` — Fill-bar display for vehicle stats (speed, acceleration, handling, weight) + description
- `VehicleListEntry.cs` — Single row in vehicle list: thumbnail, name, category badge, selection highlight
