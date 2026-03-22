# Vehicle System

## Purpose
Car/vehicle behavior — physics, drivetrain, and handling. This is the core driving simulation.

## Conventions
- Top-level: `VehicleManager.cs` (namespace `R8EOX.Vehicle`)
- Internal components in `Internal/` (namespace `R8EOX.Vehicle.Internal`, `internal` access)
- Vehicle is agnostic to Track — never import `R8EOX.Track.Internal`
- Configuration via `VehicleConfig` ScriptableObject — no hardcoded tuning values
- Use explicit `Tick(float deltaTime)` — manager drives update order

## Contents
- `VehicleManager.cs` — Top-level API: spawn, configure, drive vehicles
- `Internal/Wheel.cs` — Per-wheel physics, grip, suspension
- `Internal/Chassis.cs` — Body, weight distribution, aerodynamics
- `Internal/Motor.cs` — Engine power, RPM, torque curves
- `Internal/Transmission.cs` — Gear ratios, shifting logic
