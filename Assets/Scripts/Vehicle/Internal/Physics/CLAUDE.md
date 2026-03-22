# Vehicle Physics Math

## Purpose
Pure static math classes extracted from MonoBehaviours for testability. No Unity dependencies beyond Vector3/Quaternion.

## Conventions
- Namespace: `R8EOX.Vehicle.Internal`
- Access: `internal static class`
- All methods are pure functions (input → output, no side effects)
- Called by MonoBehaviours in parent `Internal/` directory

## Contents
- `SuspensionMath.cs` — Spring force, damping, compression calculations
- `GripMath.cs` — Lateral and longitudinal grip from curve-sampled model
- `DrivetrainMath.cs` — Differential force distribution math
- `WheelForceSolver.cs` — Combines suspension + grip + drive into net wheel force
- `WheelForceInput.cs` — Input struct for WheelForceSolver
- `WheelForceResult.cs` — Result struct from WheelForceSolver
- `AirPhysicsMath.cs` — Airborne torque and gyroscopic precession
- `GyroscopicMath.cs` — Gyroscopic effect calculations
- `TumbleMath.cs` — Tumble/crash detection thresholds
- `ESCMath.cs` — Electronic stability control calculations
