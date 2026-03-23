# Menu/Internal

## Purpose
Internal implementation classes for the Menu system. All types use `namespace R8EOX.Menu.Internal`
with `internal` access modifiers — never exposed outside the Menu system.

## Conventions
- Namespace: `R8EOX.Menu.Internal`
- Access: `internal` (classes, methods, enums)
- No MonoBehaviour singletons — MenuScreen subclasses are managed by the top-level MenuManager
- All coroutine animations use `Time.unscaledDeltaTime` (menus run while timeScale=0)

## Contents

| File | Type | Purpose |
|------|------|---------|
| `MenuAnimator.cs` | `internal static class` | Coroutine-based animation utilities: stagger fade in/out, scale-in with ease-out, alpha pulse, fill-bar lerp |
| `MenuButton.cs` | `internal class : MonoBehaviour` | UI button component — configures color, hover state, locked state, and label text |
| `MenuButtonStyle.cs` | `internal enum` | Button style variants: Primary, Secondary, Danger, Locked, Ghost |
| `MenuScreen.cs` | `internal abstract class : MonoBehaviour` | Base class for all menu screens — owns CanvasGroup fade in/out, Show/Hide/ShowImmediate/HideImmediate lifecycle |
| `ModeSelectScreen.cs` | `internal class : MenuScreen` | Mode selection screen — Testing Session (Practice), Race (locked), Multiplayer (locked); initialized with mode and back callbacks |
| `TrackSelectScreen.cs` | `internal class : MenuScreen` | Track selection screen — coordinates TrackListPanel (left) and TrackPreviewPanel (right) with START/BACK buttons; initialized with TrackRegistry, SessionMode, and confirm/back callbacks |

## Usage Pattern

`MenuScreen` subclasses call `MenuAnimator` coroutines via `StartCoroutine` to animate child elements.
`MenuButton` is configured by the parent screen — never self-configure in Awake/Start.
