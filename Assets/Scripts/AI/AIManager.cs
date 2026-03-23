using System.Collections.Generic;
using UnityEngine;
using R8EOX.AI.Internal;

namespace R8EOX.AI
{
    /// <summary>
    /// Top-level AI system manager. Registers AI drivers on vehicles,
    /// swaps their input to ScriptedInput, and ticks all drivers each frame.
    /// </summary>
    public class AIManager : MonoBehaviour
    {
        [Header("Difficulty")]
        [SerializeField] private int difficultyLevel = 5;

        [Header("Line Variation")]
        [SerializeField] private float lateralOffsetRange = 2f;

        private Track.TrackManager trackManager;
        private readonly List<AIDriver> activeDrivers = new();

        /// <summary>Number of active AI drivers.</summary>
        public int DriverCount => activeDrivers.Count;

        /// <summary>
        /// Provide the track reference. Must be called before RegisterDriver.
        /// </summary>
        public void Initialize(Track.TrackManager track)
        {
            trackManager = track;
        }

        /// <summary>
        /// Register a vehicle as AI-controlled. Disables RCInput, adds
        /// ScriptedInput, creates an AIDriver to control it.
        /// </summary>
        public void RegisterDriver(GameObject vehicle)
        {
            if (vehicle == null || trackManager == null || !trackManager.HasCenterline())
                return;

            var scriptedInput = PrepareInput(vehicle);
            if (scriptedInput == null) return;

            var vm = vehicle.GetComponent<Vehicle.VehicleManager>();

            var racingLine = new RacingLine();
            float offset = Random.Range(-lateralOffsetRange, lateralOffsetRange);
            racingLine.Initialize(trackManager, offset);

            var behavior = new AIBehavior();
            behavior.Initialize(difficultyLevel);

            var driver = new AIDriver();
            driver.Initialize(vehicle, scriptedInput, vm, racingLine, behavior);
            activeDrivers.Add(driver);
        }

        /// <summary>Update all AI drivers. Called by SessionManager each frame.</summary>
        public void Tick(float deltaTime)
        {
            for (int i = activeDrivers.Count - 1; i >= 0; i--)
            {
                if (activeDrivers[i].IsValid)
                    activeDrivers[i].Tick(deltaTime);
                else
                    activeDrivers.RemoveAt(i);
            }
        }

        /// <summary>Set difficulty level (1-10). Affects future RegisterDriver calls.</summary>
        public void SetDifficulty(int level)
        {
            difficultyLevel = Mathf.Clamp(level, 1, 10);
        }

        /// <summary>Remove all active drivers. Does not destroy vehicles.</summary>
        public void RemoveAllDrivers()
        {
            activeDrivers.Clear();
        }

        /// <summary>
        /// Disable player input and ensure ScriptedInput is attached.
        /// </summary>
        private static Input.Internal.ScriptedInput PrepareInput(GameObject vehicle)
        {
            var rcInput = vehicle.GetComponent<Input.RCInput>();
            if (rcInput != null) rcInput.enabled = false;

            var scripted = vehicle.GetComponent<Input.Internal.ScriptedInput>();
            if (scripted == null)
                scripted = vehicle.AddComponent<Input.Internal.ScriptedInput>();

            return scripted;
        }
    }
}
