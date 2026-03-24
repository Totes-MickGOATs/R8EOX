# Tests

## Purpose
All NUnit-based tests for the R8EOX project. Organized into EditMode (fast, no scene) and PlayMode (full Unity lifecycle).

## Conventions
- Namespace: `R8EOX.Tests` (EditMode), `R8EOX.Tests.PlayMode` (PlayMode)
- Test naming: `MethodName_Condition_ExpectedResult`
- Use `Assert.That` constraint model (not classic `Assert.AreEqual`)
- 300 line limit per test file — split by feature area if needed
- `Assets/Tests/` is exempt from the cross-system `using R8EOX.X.Internal` lint rule
- Clean up GameObjects in `[TearDown]` / `[UnityTearDown]`

## Contents
- `EditMode/` — Fast unit tests: pure math, ScriptableObjects, data structures, validators
- `PlayMode/` — Integration tests: scene loading, session flow, vehicle spawn, runtime behavior

## EditMode Test Files
- `RingBufferTests.cs` — RingBuffer generic circular buffer
- `EventLogTests.cs` — EventLog add, filter by channel, GetRecent
- `FlowTracerTests.cs` — FlowTracer register, begin, step reporting, fail, restart, clear, timeout (14 tests)
- `LifecycleVerifierTests.cs` — LifecycleVerifier deferred destroy checks
- `DrivetrainMathTests.cs` — DrivetrainMath pure math
- `TumbleMathTests.cs` — TumbleMath pure math
- `SuspensionMathTests.cs` — SuspensionMath pure math
- `GripMathTests.cs` — GripMath pure math
- `ESCMathTests.cs` — ESCMath pure math
- `FrictionCircleMathTests.cs` — FrictionCircleMath pure math
- `WheelForceSolverTests.cs` — WheelForceSolver integration
- `GyroscopicMathTests.cs` — GyroscopicMath pure math
- `SessionConfigTests.cs` — SessionConfig ScriptableObject validation
- `SessionChannelTests.cs` — SessionChannel data
- `TrackReadinessTests.cs` — TrackReadiness validation
- `BuggyMaterialsTests.cs` — BuggyMaterials asset validation
- `TrackFolderScannerTests.cs` — TrackFolderScanner discovery
- `TerrainLayerBuilderTests.cs` — TerrainLayerBuilder
- `TerrainSplatmapBuilderTests.cs` — TerrainSplatmapBuilder
- `OutpostTrackSetupTests.cs` — Outpost track scene setup
