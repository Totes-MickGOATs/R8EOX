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
- `Internal/` — Internal MonoBehaviours and helper classes
- `Internal/Physics/` — Pure static math classes (suspension, grip, drivetrain, tumble)
