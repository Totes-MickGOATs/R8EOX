using UnityEngine;
using UnityEngine.InputSystem;

namespace R8EOX.Tests.PlayMode
{
    /// <summary>
    /// Helpers for input simulation in PlayMode tests.
    /// Vehicle tests: use ScriptedInput (same path AI uses, bypasses Input System).
    /// Menu tests: use InputTestFixture + device simulation.
    /// </summary>
    internal static class TestInputHelper
    {
        /// <summary>
        /// Swaps the vehicle's input component from RCInput to ScriptedInput.
        /// Returns the ScriptedInput for test control.
        /// ScriptedInput is internal — accessible via InternalsVisibleTo.
        /// </summary>
        public static R8EOX.Input.Internal.ScriptedInput SwapToScriptedInput(
            GameObject vehicle)
        {
            var existing = vehicle.GetComponent<R8EOX.Input.RCInput>();
            if (existing != null)
                Object.Destroy(existing);

            var scripted = vehicle.AddComponent<R8EOX.Input.Internal.ScriptedInput>();
            return scripted;
        }

        /// <summary>
        /// Creates virtual test input devices for menu/UI testing.
        /// Returns (keyboard, gamepad) tuple.
        /// Use with InputTestFixture for event-driven input simulation.
        /// </summary>
        public static (Keyboard keyboard, Gamepad gamepad) CreateTestDevices()
        {
            var keyboard = InputSystem.AddDevice<Keyboard>();
            var gamepad = InputSystem.AddDevice<Gamepad>();
            return (keyboard, gamepad);
        }

        /// <summary>
        /// Removes virtual test devices created by CreateTestDevices.
        /// Call in TearDown to avoid device leaks between tests.
        /// </summary>
        public static void RemoveTestDevices(Keyboard keyboard, Gamepad gamepad)
        {
            if (keyboard != null)
                InputSystem.RemoveDevice(keyboard);
            if (gamepad != null)
                InputSystem.RemoveDevice(gamepad);
        }
    }
}
