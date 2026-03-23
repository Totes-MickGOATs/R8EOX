# PlayMode Tests

## Purpose
Integration and end-to-end tests that run in Play Mode with full Unity lifecycle. Test scene loading, session flow, vehicle spawning, and runtime behavior.

## Conventions
- Namespace: `R8EOX.Tests.PlayMode`
- Assembly: `R8EOX.Tests.PlayMode` (defined in `R8EOX.Tests.PlayMode.asmdef`)
- Use `[UnityTest]` (returns `IEnumerator`) instead of `[Test]` for async operations
- Use `[UnitySetUp]` / `[UnityTearDown]` for setup/teardown that needs to yield
- Extend `E2ETestBase` for tests that load scenes (handles DDOL cleanup)
- Use `E2ETestUtils` static helpers for common waits and scene loading
- Use `TestInputHelper` for input simulation (ScriptedInput for vehicles, InputTestFixture for menus)
- Categorize tests with `[Category("...")]`: `smoke`, `integrity`, `flow`, `integration`

## Test Categories
- **smoke** — Boot/menu loads without errors, basic scene structure
- **integrity** — Scene components present with valid references
- **flow** — Session lifecycle: bootstrapper → session → spawn → ready
- **integration** — Vehicle spawn correctness: components, position, input

## Contents
- `R8EOX.Tests.PlayMode.asmdef` — Assembly definition for PlayMode tests
- `E2ETestBase.cs` — Abstract base class: DDOL cleanup, timeScale reset, empty scene teardown
- `E2ETestUtils.cs` — Static helpers: LoadSceneAndWait, WaitUntil, WaitForGameObject, WaitForComponent, WaitForNoErrors, WaitForFrames
- `TestInputHelper.cs` — Input simulation: SwapToScriptedInput, CreateTestDevices
- `SmokeTests.cs` — Boot/menu scene loading, AppRoot presence, zero-error checks
- `SceneIntegrityTests.cs` — Track scene components: SessionBootstrapper, TrackManager, CameraManager, spawn points
- `SessionFlowTests.cs` — Editor-play flow: SessionManager creation, vehicle spawn, camera wiring, HUD, cleanup
- `VehicleSpawnTests.cs` — Vehicle correctness: Rigidbody, input, terrain height, spawn point position
- `Regressions/` — Regression tests for bugs found by E2E agent
