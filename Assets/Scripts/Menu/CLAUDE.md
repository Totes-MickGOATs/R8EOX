# Menu

## Purpose
The Menu system controls all game UI screens and transitions. The top-level class (`MenuManager`,
when created) is the only public API — it uses `namespace R8EOX.Menu`.

## Architecture
Follows top-down system conventions. External systems call `MenuManager` only.
Internal classes live in `Internal/` with `namespace R8EOX.Menu.Internal` + `internal` access.

## Contents

| Path | Description |
|------|-------------|
| `Internal/` | Internal menu components — screens, buttons, animation utilities |

## Key Conventions
- All animations use `Time.unscaledDeltaTime` — menus must run while `Time.timeScale == 0`
- Screen lifecycle managed by `MenuScreen` base class (Show/Hide with CanvasGroup fade)
- Animation helpers in `MenuAnimator` (static, coroutine-based, no DOTween)
- Button appearance driven by `MenuButtonStyle` enum and configured by parent screen
