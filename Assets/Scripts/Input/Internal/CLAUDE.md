# Input Internal Components

## Purpose
Internal implementation for the Input system. Only top-level Input classes should reference these.

## Conventions
- Namespace: `R8EOX.Input.Internal`
- Access: `internal static class` (enforced by pre-commit hook)

## Contents
- `InputMath.cs` — Pure math for input processing: deadzone remapping, steering curves, input merging
- `ScriptedInput.cs` — MonoBehaviour implementing IVehicleInput with externally settable properties; used by autopilot/path-following systems
