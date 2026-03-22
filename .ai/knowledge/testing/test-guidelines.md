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
- Namespace: `R8EOX.Tests`
- Run in Play Mode with full Unity lifecycle
- Good for: MonoBehaviour integration, physics, coroutines, scene loading

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

## File Limits

Test files follow the same 300 line limit. If a test file grows large, split by feature area.
