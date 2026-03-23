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
- `PauseMenu.cs` — Pause screen. Full button set: RESUME (primary), OPTIONS (secondary), RESTART, CHANGE VEHICLE, RETURN TO MENU, QUIT TO DESKTOP. Destructive actions (Restart, Return to Menu, Quit to Desktop) require ConfirmDialog confirmation. UI built programmatically on first Show() via lazy BuildUI(). Initialize(optionsCallback, restartCallback) wires caller-supplied handlers. Controls Time.timeScale (0 on Show, 1 on Hide). Routes vehicle swap and quit-to-menu through UIManager.
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
- `ProfileTabContent.cs` — Profile management tab: dropdown of all profiles, RENAME/NEW/DELETE action buttons (RENAME is a placeholder toast; NEW auto-names; DELETE confirms via ConfirmDialog), and muted info labels explaining per-profile vs shared settings. Initialize(SettingsManager, ToastManager) wires dependencies.
- `ControlsTabContent.cs` — Options screen Controls tab. Three sections: Controller Profile (dropdown + Save As New / Delete profile buttons with ConfirmDialog), Input Settings (Steer Deadzone 0-0.3, Throttle DZ 0-0.3, Curve Exponent 1-3 sliders writing via SetControlsSettings), and Key/Button Bindings (ScrollRect skeleton with placeholder label for future RebindEntry rows). Reset to Defaults (STYLE_DANGER) resets via ControlsSettings.CreateDefault(). Initialize(SettingsManager).
- `RebindEntry.cs` — Single row skeleton for the future key/button bindings list. Exposes Initialize(actionName, currentBinding), StartRebind() (sets label to "Press key..." and disables button), and SetBinding(displayText). Hook point for InputActionRebindingExtensions.PerformInteractiveRebinding() is marked with a TODO. Not yet wired into ControlsTabContent.
- `VideoTabContent.cs` — Options screen Video tab. Four sections: Graphics Quality (tier button group: Ultra/High/Balanced/Perf, maps to QualityTier via int cast), Display (Window Mode and Resolution dropdowns; resolution list built from Screen.resolutions with WxH deduplication), Sync (V-Sync and FPS Cap dropdowns; FPS cap values: 0/30/60/120/144/240), Rendering (Render Scale slider 0.5–1.0, step 0.05). Adds VerticalLayoutGroup + ContentSizeFitter on Initialize. Reads current settings via GetVideoSettings(); writes via SetVideoSettings() lambdas and SetQualityTier(). Silent setters (SetValueWithoutNotify) prevent callback loops during PopulateFromSettings. Initialize(SettingsManager).
- `CalibrationTabContent.cs` — Calibration settings tab: preferred-controller dropdown, live axis-test area (2 StickVisualizers + 2 TriggerVisualizers updated each frame via Gamepad.current), per-axis inversions/sensitivities/outer-deadzones/drift-offsets, CALIBRATE button (opens CalibrationWizard) and RESET button (DANGER ConfirmDialog). Initialize(SettingsManager) wires all dependencies.
- `CalibrationWizard.cs` — Modal calibration wizard (Canvas sortOrder=250). FSM with 4 states via int constants: Release (idle prompt + OK), SampleNeutral (60-frame averaging of 6 axes), MoveExtremes (10s min/max tracking), Results (span validation + SAVE/CANCEL). SAVE calls onComplete(float[4] neutralOffsets, float[6,2] axisRanges); CANCEL destroys the GO. Gracefully skips frames when Gamepad.current is null.
