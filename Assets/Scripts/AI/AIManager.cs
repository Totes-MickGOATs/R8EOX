using System.Collections.Generic;
using UnityEngine;

namespace R8EOX.AI
{
    public class AIManager : MonoBehaviour
    {
        [SerializeField] private int difficultyLevel = 1;
        private List<R8EOX.AI.Internal.AIDriver> activeDrivers = new();

        public void SpawnAIDriver(GameObject vehicle, Vector3[] racingLinePoints)
        {
            // TODO: Create AI driver for vehicle with racing line data
        }

        public void Tick(float deltaTime)
        {
            // TODO: Update all AI drivers
        }

        public void SetDifficulty(int level)
        {
            difficultyLevel = level;
            // TODO: Propagate difficulty to all active drivers
        }

        public void RemoveAllDrivers()
        {
            activeDrivers.Clear();
        }
    }
}
