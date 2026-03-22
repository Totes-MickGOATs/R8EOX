using UnityEngine;

namespace R8EOX.AI.Internal
{
    internal class AIDriver
    {
        private GameObject vehicle;
        private RacingLine racingLine;
        private AIBehavior behavior;

        internal void Initialize(GameObject targetVehicle, RacingLine line, int difficulty)
        {
            vehicle = targetVehicle;
            racingLine = line;
            // TODO: Configure behavior based on difficulty
        }

        internal void Tick(float deltaTime)
        {
            // TODO: Calculate desired throttle, brake, steering from racing line
            // TODO: Apply corrections based on behavior (overtaking, defending)
        }

        internal float GetThrottle()
        {
            // TODO: Return AI throttle input
            return 0f;
        }

        internal float GetBrake()
        {
            // TODO: Return AI brake input
            return 0f;
        }

        internal float GetSteering()
        {
            // TODO: Return AI steering input
            return 0f;
        }
    }
}
