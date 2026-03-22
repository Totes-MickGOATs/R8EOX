# Input Internal Components

## Purpose
Internal implementation for the Input system. Only `InputManager` should reference these.

## Conventions
- Namespace: `R8EOX.Input.Internal`
- Access: `internal class` (enforced by pre-commit hook)

## Contents
- `InputMapper.cs` — Normalizes raw InputAction values to throttle (0-1), brake (0-1), and steering (-1 to 1)
