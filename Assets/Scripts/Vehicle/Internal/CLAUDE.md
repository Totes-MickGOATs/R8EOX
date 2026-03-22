# Vehicle Internal Components

## Purpose
Internal implementation classes for the Vehicle system. Only `VehicleManager` should reference these — never other systems.

## Conventions
- Namespace: `R8EOX.Vehicle.Internal`
- Access: `internal class` (enforced by pre-commit hook)
- These classes receive calls from VehicleManager, never from Track, Race, etc.

## Contents
- `Wheel.cs` — Per-wheel physics: grip, suspension compression, steering, braking, drive torque
- `Chassis.cs` — Body properties: mass, center of mass, aerodynamic drag and downforce
- `Motor.cs` — Engine simulation: RPM, torque curve sampling, throttle response
- `Transmission.cs` — Gear ratios, shifting logic, final drive ratio calculation
