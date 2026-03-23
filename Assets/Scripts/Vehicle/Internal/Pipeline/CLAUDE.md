# Vehicle Physics Pipeline

## Purpose
Sequential pipeline stages that decompose VehicleManager.FixedUpdate into discrete, testable steps. Each stage reads/writes a shared `VehicleFrame` struct passed by `ref`.

## Conventions
- Namespace: `R8EOX.Vehicle.Internal`
- All stages are `internal static class` with a single `public static void Execute(ref VehicleFrame, ...)`
- Stages are stateless — all persistent state lives in VehicleManager or its sub-components
- Pipeline order is fixed: Input → Airborne → GroundDrive → Drivetrain → Steering → AirPhysics → WheelSolve

## Contents
- `VehicleFrame.cs` — Mutable struct: pipeline's shared scratchpad (time, input, airborne, drive, steering fields)
- `InputStage.cs` — Reads IVehicleInput, smooths throttle ramp, computes forward speed
- `AirborneStage.cs` — AirborneDetector update + tumble tilt calculation
- `GroundDriveStage.cs` — ESCMath engine/brake/coast logic, coast drag force application
- `DrivetrainStage.cs` — Delegates to Drivetrain.Distribute() for differential force split
- `SteeringStage.cs` — Speed-sensitive steering via SteeringRamp.Step()
- `AirPhysicsStage.cs` — Delegates to RCAirPhysics.Apply() for gyroscopic torques
- `WheelSolveStage.cs` — Per-wheel ApplyWheelPhysics + steering rotation
