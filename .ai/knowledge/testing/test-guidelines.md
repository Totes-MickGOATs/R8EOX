# Testing Guidelines

## Framework

NUnit via `com.unity.test-framework` 1.6.0

## Test Types

### EditMode Tests
- Location: `Assets/Tests/EditMode/`
- Namespace: `R8EOX.Tests`
- Run without entering Play Mode — fast
- Good for: pure logic, ScriptableObjects, data structures, utility functions

### PlayMode Tests
- Location: `Assets/Tests/PlayMode/`
- Namespace: `R8EOX.Tests.PlayMode`
- Assembly: `R8EOX.Tests.PlayMode` (separate asmdef, `includePlatforms` empty for player loop)
- Run in Play Mode with full Unity lifecycle
- Good for: MonoBehaviour integration, physics, coroutines, scene loading
- Base class: extend `E2ETestBase` for tests that load scenes (handles DDOL cleanup)
- Utilities: `E2ETestUtils` (scene loading, waits), `TestInputHelper` (input simulation)
- Categories: `smoke`, `integrity`, `flow`, `integration`, `regression`
- Use `[UnityTest]` returning `IEnumerator` (not `[Test]`) for async operations
- Use `[UnitySetUp]` / `[UnityTearDown]` for setup needing yield

## Test Structure

```csharp
using NUnit.Framework;
using R8EOX;

namespace R8EOX.Tests
{
    [TestFixture]
    public class HealthSystemTests
    {
        private HealthSystem health;

        [SetUp]
        public void SetUp()
        {
            // Create fresh instance for each test
            var go = new GameObject();
            health = go.AddComponent<HealthSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(health.gameObject);
        }

        [Test]
        public void TakeDamage_ReducesHealth()
        {
            health.TakeDamage(25);
            Assert.That(health.CurrentHealth, Is.EqualTo(75));
        }

        [Test]
        public void TakeDamage_AtZero_TriggersDeath()
        {
            bool deathTriggered = false;
            health.OnDeath += () => deathTriggered = true;

            health.TakeDamage(100);

            Assert.That(deathTriggered, Is.True);
        }
    }
}
```

## Conventions

- Use `Assert.That` constraint model (not classic `Assert.AreEqual`)
- Test method names: `MethodName_Condition_ExpectedResult`
- One assertion per test when possible
- Use `[SetUp]` / `[TearDown]` for shared setup/cleanup
- Clean up GameObjects in `[TearDown]` with `DestroyImmediate`

## Running Tests

- MCP: `mcp__UnityMCP__run_tests` — runs all or filtered tests
- Unity: Window > General > Test Runner
- Filter by: EditMode, PlayMode, test name, category

## PlayMode Test Infrastructure

### E2ETestBase (abstract base)
- `[UnitySetUp]` — destroys all DontDestroyOnLoad objects (`[AppRoot]`, `[SessionManager]`, etc.)
- `[UnityTearDown]` — resets `Time.timeScale`, loads empty scene, cleans DDOL again
- DDOL cleanup trick: create temp GO → `DontDestroyOnLoad(temp)` → enumerate root objects in DDOL scene → destroy all → destroy temp

### E2ETestUtils (static helpers)
- `LoadSceneAndWait(sceneName, timeout)` — async scene load + wait for activation
- `WaitUntil(condition, timeout, message)` — generic predicate wait with timeout
- `WaitForGameObject(name, timeout)` — poll `GameObject.Find` each frame
- `WaitForComponent<T>(timeout)` — poll `FindAnyObjectByType<T>()`
- `WaitForNoErrors(settleFrames)` — subscribe to `Application.logMessageReceived`
- `WaitForFrames(count)` — simple frame skip

### TestInputHelper
- `SwapToScriptedInput(vehicle)` — remove `RCInput`, add `ScriptedInput`, return it
- `CreateTestDevices()` — add `Keyboard` + `Gamepad` via `InputSystem.AddDevice<T>()`
- Vehicle tests: use `ScriptedInput` (same path AI uses, bypasses Input System)
- Menu tests: use `InputTestFixture` + device simulation

## E2E Quality Workflow

The `unity-e2e-tester` agent runs against an isolated Unity editor (git worktree):
1. Sets up worktree + launches test editor
2. Pins to test editor via `set_active_instance`
3. Runs: Console → EditMode → Scene Integrity → Play Mode → PlayMode Tests → UX Review
4. Produces structured quality report (Technical + Experience sections)
5. Auto-dispatches fixes for blocking issues
6. Invoke with `/e2e` (full), `/e2e smoke`, `/e2e regression`, `/e2e ux`

## File Limits

Test files follow the same 300 line limit. If a test file grows large, split by feature area.
