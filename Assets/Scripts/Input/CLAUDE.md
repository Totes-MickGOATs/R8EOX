# Input System

## Purpose
Input handling — routes player input to the vehicle system using Unity's new Input System.

## Conventions
- Top-level: `InputManager.cs` (namespace `R8EOX.Input`)
- Internal components in `Internal/` (namespace `R8EOX.Input.Internal`, `internal` access)
- Uses `UnityEngine.InputSystem` — legacy Input API is banned
- Input actions defined in `.inputactions` asset files

## Contents
- `InputManager.cs` — Top-level API: read input, route to vehicle
- `Internal/InputMapper.cs` — Maps Input System actions to vehicle commands
