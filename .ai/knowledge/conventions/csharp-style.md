# C# Code Style Conventions

## Namespace

- All runtime scripts: `namespace R8EOX`
- Editor scripts: `namespace R8EOX.Editor`
- Test scripts: `namespace R8EOX.Tests`
- System internal classes: `namespace R8EOX.{System}.Internal`

## Naming

| Element | Convention | Example |
|---------|-----------|---------|
| Classes, structs, enums | PascalCase | `PlayerController`, `DamageType` |
| Interfaces | IPascalCase | `IDamageable`, `IInteractable` |
| Methods, properties | PascalCase | `TakeDamage()`, `MaxHealth` |
| Events | PascalCase | `OnDeath`, `OnHealthChanged` |
| Private fields | camelCase | `maxHealth`, `moveSpeed` |
| Local variables | camelCase | `currentTarget`, `hitCount` |
| Parameters | camelCase | `amount`, `targetPosition` |
| Constants | PascalCase | `MaxPlayers`, `DefaultSpeed` |

## Fields

- Prefer `[SerializeField] private` over `public` for Inspector-exposed fields
- Always use explicit access modifiers (`private`, `public`, `protected`)
- Use `[Header("Section")]` and `[Tooltip("Description")]` for Inspector organization
- Pre-commit hook blocks public fields of common types (int, float, string, bool, Vector, GameObject, etc.)

```csharp
// CORRECT
[Header("Health")]
[SerializeField] private int maxHealth = 100;
[SerializeField] private float regenRate = 1f;

// WRONG — blocked by pre-commit hook
public int maxHealth = 100;
public float regenRate = 1f;
```

## File Organization

- **One class per file** — class name must match file name exactly
- **300 line maximum** per file — enforced by pre-commit hook
- If approaching 300 lines, refactor into smaller, focused classes

## Component Dependencies

- Use `[RequireComponent(typeof(T))]` when a script depends on another component
- Cache `GetComponent<T>()` results in `Awake()` or `Start()` — never in Update loops

## Access Modifiers

Always explicit — never rely on C# defaults:
```csharp
// CORRECT
private void HandleInput() { }
public void TakeDamage(int amount) { }

// WRONG — implicit private
void HandleInput() { }
```

## Banned Patterns

- Singleton `.Instance` — use dependency injection through parent system
- `SendMessage()` / `BroadcastMessage()` — use direct method calls
- `FindObjectOfType()` — pass references through parent system
- `Camera.main` in Update loops — cache in Awake/Start
- Legacy `Input.GetKey()` / `Input.GetAxis()` — use new Input System
