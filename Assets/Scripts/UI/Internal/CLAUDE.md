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
- `ConfirmDialog.cs` — Self-contained reusable modal for destructive confirmations. Static `Show()` creates its own Canvas (sortOrder=200, ScreenSpaceOverlay) so it renders above all other overlays. Supports danger (red border) and primary (cyan border) styling, full-screen backdrop that blocks clicks, Escape-to-cancel, and a 0.15s CanvasGroup fade-in via unscaledDeltaTime. Entirely code-built — no prefab required.
- `StickVisualizer.cs` — Gamepad stick visualizer (140x140px): background, crosshair, deadzone circle, and cyan position dot built programmatically; Initialize/UpdateInput/SetDeadzoneRadius API
- `TriggerVisualizer.cs` — Gamepad trigger fill bar (120x16px): dark track, cyan fill proportional to value, label (LT/RT) and percentage text built programmatically; Initialize/UpdateValue API
- `OptionsUIFactory.cs` — Internal static factory: creates themed UI controls (section headers, slider rows, dropdown rows, checkbox rows, tier button groups, action buttons) in the R8EOX cyan/gold/dark palette; style constants STYLE_PRIMARY/SECONDARY/DANGER; no MonoBehaviour required
