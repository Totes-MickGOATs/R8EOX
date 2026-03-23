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
