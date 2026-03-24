# Diagnostics Internal

## Purpose
Internal implementation for the Diagnostics system. All types use `internal` access and namespace `R8EOX.Diagnostics.Internal`. Every file is wrapped in `#if UNITY_EDITOR || DEVELOPMENT_BUILD`.

## Contents

### Data Structures
- `RingBuffer.cs` — Generic fixed-size circular buffer with IEnumerable support
- `EventEntry.cs` — Readonly struct: timestamped event with channel and severity
- `EventSeverity.cs` — Enum: Info, Warning, Error
- `EventLog.cs` — Channel-aware event log built on RingBuffer; Add, GetAll, GetByChannel, GetRecent
- `FlowStep.cs` — Readonly struct: one expected step in a flow (name, optional flag, timeout)
- `FlowStatus.cs` — Enum: Active, Completed, Failed, TimedOut
- `FlowDefinition.cs` — Named sequence of FlowSteps with builder-pattern AddStep
- `FlowInstance.cs` — Runtime state of an active flow: step completion, timing, failure tracking
- `LifecycleRequest.cs` — Struct: pending destroy verification with frame tracking
- `VerifyResult.cs` — Readonly struct: result of a lifecycle verification (label, passed, timestamp)
- `VehicleDiagSnapshot.cs` — Struct: per-frame vehicle data (speed, input, grounding, slip)

### Logic
- `FlowTracer.cs` — Manages active/completed/failed flows; timeout detection; event logging
- `FlowTemplates.cs` — Static: predefined flows (TrackLoad, VehicleSelect, ReturnToMenu, PauseUnpause, OverlayLifecycle)
- `LifecycleVerifier.cs` — Deferred object destruction checks; verifies Unity `== null` one frame after Destroy
