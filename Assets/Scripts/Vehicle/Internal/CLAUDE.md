# Vehicle Internal Components

## Purpose
Internal implementation classes for the Vehicle system. Only `VehicleManager` references these — never other systems.

## Conventions
- Namespace: `R8EOX.Vehicle.Internal`
- Access: `internal class` (enforced by pre-commit hook)
- MonoBehaviours where Unity requires (Transform/Physics access): RaycastWheel, Drivetrain, RCAirPhysics
- Plain classes/structs for helpers: SteeringRamp, TumbleController, AirborneDetector, MotorPresetRegistry
- These classes receive calls from VehicleManager, never from Track, Race, etc.
- Pure math delegated to static classes in `Physics/` subdirectory

## Contents
- `RaycastWheel.cs` — Per-wheel physics: SphereCast suspension, grip, surface detection (MonoBehaviour on each wheel pivot)
- `Drivetrain.cs` — Differential force distribution across wheels (open, ball, spool, AWD)
- `RCAirPhysics.cs` — Airborne gyroscopic precession and reaction torque
- `WheelManager.cs` — Coordinates the wheel array for VehicleManager
- `WheelVisuals.cs` — Rotates visual wheel meshes based on angular velocity
- `SteeringRamp.cs` — Speed-dependent steering angle interpolation
- `TumbleController.cs` — Tilt detection and tumble factor for telemetry (no material blending)
- `AirborneDetector.cs` — Tracks whether vehicle is airborne
- `MotorPresetRegistry.cs` — Predefined motor tuning presets (21.5T through 1.5T)
- `MotorPreset.cs` — Motor preset enum (extracted from VehicleManager for one-type-per-file)
- `Physics/` — Pure static math classes (see Physics/CLAUDE.md)
- `Pipeline/` — Sequential physics pipeline stages (see Pipeline/CLAUDE.md)
