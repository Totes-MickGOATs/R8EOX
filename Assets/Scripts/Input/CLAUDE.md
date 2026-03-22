# Input System

## Purpose
Input handling — routes player input to the vehicle system using Unity's new Input System.

## Conventions
- Top-level: `InputManager.cs` (namespace `R8EOX.Input`)
- Internal components in `Internal/` (namespace `R8EOX.Input.Internal`, `internal` access)
- Uses `UnityEngine.InputSystem` — legacy Input API is banned
- Input actions defined in `R8EOXInputActions.inputactions` asset

## Contents
- `InputManager.cs` — Top-level coordinator: manages enable/disable, resolves active input provider
- `IVehicleInput.cs` — Interface for swappable input providers (player, AI, replay, network)
- `RCInput.cs` — MonoBehaviour implementing `IVehicleInput` via R8EOXInputActions (keyboard + gamepad)
- `R8EOXInputActions.inputactions` — Unity Input System actions asset
- `R8EOXInputActions.cs` — Auto-generated C# wrapper for the input actions asset (do not edit)
- `Internal/InputMath.cs` — Pure math: deadzone remapping and steering curve application

## Architecture
- `RCInput` reads actions from `R8EOXInputActions` each frame and applies steering curve via `InputMath`
- `InputManager` is the system's top-level class; manages enable/disable for pause, cutscenes, etc.
- `VehicleManager` resolves `IVehicleInput` via `GetComponent` on its own GameObject
- Pure math in `InputMath` has no Unity lifecycle dependency for testability
