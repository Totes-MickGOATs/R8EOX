# Diagnostics System

## Purpose
Runtime diagnostics framework for tracing game event flows, detecting silent failures, and verifying object lifecycle. Completely stripped from production builds via `[Conditional]` attributes and `#if` guards — zero runtime cost in release.

## Conventions
- Top-level: `DiagnosticsManager.cs` (namespace `R8EOX.Diagnostics`)
- Static facade: `Diag.cs` — the ONLY class other systems call
- Internal components in `Internal/` (namespace `R8EOX.Diagnostics.Internal`, `internal` access)
- All internal types wrapped in `#if UNITY_EDITOR || DEVELOPMENT_BUILD`
- `Diag` methods use `[Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]` — compiler strips call sites AND argument evaluation in release

## Contents
- `Diag.cs` — Static facade: Log, BeginFlow, FlowStep, FailFlow, VerifyDestroyed, SetVehicleSnapshot
- `DiagChannel.cs` — Enum: App, Session, Menu, UI, Vehicle, Track, Race, Camera, Audio, VFX, AI
- `DiagnosticsManager.cs` — Top-level MonoBehaviour on [AppRoot]: owns EventLog, FlowTracer, LifecycleVerifier; F12 toggles IMGUI overlay
- `DiagnosticsOverlay.cs` — IMGUI overlay with Flows/Events/Vehicle/Channels tabs
- `Internal/` — Data structures, logic, and ring buffer

## Usage Pattern

```csharp
// Other systems only ever call Diag.*
Diag.Log(DiagChannel.Session, "Vehicle spawned");
Diag.BeginFlow("TrackLoad");
Diag.FlowStep("TrackLoad", "SceneLoaded");
Diag.FailFlow("TrackLoad", "UIManager was null");
Diag.VerifyDestroyed(overlayInstance, "VehicleSelectOverlay");
```

## Key Design Decisions
- Static facade with `Diag.SetManager(this)` — NOT a singleton (no `.Instance`)
- Deferred verification: `LifecycleVerifier` checks objects 1 frame after Destroy via LateUpdate
- Channel filtering at display time — all events recorded, overlay filters display
- Sub-flows: TrackLoad contains VehicleSelect as a logical sub-flow
- No new asmdef — lives in R8EOX.Runtime assembly
