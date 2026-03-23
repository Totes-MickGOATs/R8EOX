# Vehicle System

## Purpose
RC car vehicle physics — suspension, drivetrain, grip, and handling. This is the core driving simulation, migrated from the legacy project's proven RCBuggy implementation.

## Conventions
- Top-level: `VehicleManager.cs` (namespace `R8EOX.Vehicle`) — renamed from legacy `RCCar`
- Internal components in `Internal/` (namespace `R8EOX.Vehicle.Internal`, `internal` access)
- Internal MonoBehaviours are used where Unity requires it (Transform for SphereCast, Rigidbody access)
- `InternalsVisibleTo("R8EOX.Editor")` in AssemblyInfo.cs allows Editor builders to add internal components
- Vehicle is agnostic to Track — never import `R8EOX.Track.Internal`
- Configuration via ScriptableObjects in `ScriptableObjects/` (MotorPresetConfig, SuspensionConfig, TractionConfig, WheelInertiaConfig)
- VehicleManager owns `FixedUpdate` internally, calls explicit methods on sub-components in controlled order
- Public API: `ApplyInput()`, `GetSpeed()`, `SetPaused()` — no external `Tick()`
- Pure static math classes in `Internal/Physics/` for testability

## Contents
- `VehicleManager.cs` — Top-level API: RC car controller, reads input, applies forces, exposes tuning
- `WheelTelemetry.cs` — Public readonly struct: per-wheel data snapshot (contact, slip, grip, RPM, suspension)
- `VehicleTelemetry.cs` — Public readonly struct: full vehicle state snapshot (speed, RPM, inputs, airborne, wheels)
- `Internal/` — Internal MonoBehaviours and helper classes
- `Internal/Physics/` — Pure static math classes (suspension, grip, drivetrain, tumble, friction circle)
- `Internal/Pipeline/` — Sequential physics pipeline stages (see Pipeline/CLAUDE.md)

## Physics Pipeline
VehicleManager.FixedUpdate delegates to sequential stages in `Internal/Pipeline/`:
`InputStage → AirborneStage → GroundDriveStage → DrivetrainStage → SteeringStage → AirPhysicsStage → WheelSolveStage`
Each stage is a static class with `Execute(ref VehicleFrame, ...)`. VehicleFrame is a mutable struct created fresh each FixedUpdate. Adding a feature = create one stage file + add one line to FixedUpdate.

## Telemetry Contracts
`WheelTelemetry` and `VehicleTelemetry` are public DTOs in `R8EOX.Vehicle` — other systems (Audio, VFX, UI) read them without touching Vehicle internals. `VehicleTelemetry` is returned by `VehicleManager.GetTelemetry()`.

## Spec Contract
VehicleManager's serialized fields are set by `RCBuggyBuilder.ConfigureVehicleManager()` at build time. `Start()` must NOT re-assign Rigidbody properties (mass, damping, interpolation, collision mode) — these come from the prefab. Only `centerOfMass` is applied in `Start()` because it reads from the serialized `_comGround` field. Motor params are applied via `ApplyMotorPreset()` from the serialized `_motorPreset` enum.

## Static Accessors for Exporter
- `VehicleManager.FlipHeightOffset` — exposes `k_FlipHeightOffset` for `BuggySpecExporter`
- `Drivetrain.DiffStiffnessConst` — exposes `k_DiffStiffness` for `BuggySpecExporter`
