# Input System Architecture

## Package

`com.unity.inputsystem` version 1.19.0

## Rules

- This project uses the **new Input System** exclusively
- The legacy Input Manager (`Input.GetKey`, `Input.GetAxis`, `Input.GetButton`) is NOT active
- The pre-commit hook blocks any usage of legacy Input APIs

## Patterns

### Action Maps
- Define input actions in `.inputactions` asset files
- Reference actions by name or direct reference, never by raw key codes
- Use `PlayerInput` component for automatic action map management

### Reading Input
```csharp
// CORRECT — new Input System
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    public void Tick() // Called by parent system, not auto-Update
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
    }
}
```

```csharp
// WRONG — legacy Input (blocked by pre-commit hook)
float h = Input.GetAxis("Horizontal"); // BANNED
bool jump = Input.GetKey(KeyCode.Space); // BANNED
```

## Verification

Before using Input System APIs, verify with `unity_reflect`:
- `unity_reflect search "PlayerInput"`
- `unity_reflect get_type "UnityEngine.InputSystem.PlayerInput"`
