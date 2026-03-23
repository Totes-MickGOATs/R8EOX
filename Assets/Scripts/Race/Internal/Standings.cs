using System;
using System.Collections.Generic;
using UnityEngine;

namespace R8EOX.Race.Internal
{
    internal class Standings
    {
        private readonly Dictionary<GameObject, int> lapCounts = new();
        private readonly Dictionary<GameObject, int> lastCheckpoint = new();
        private readonly Dictionary<GameObject, int> previousCheckpoint = new();
        private readonly List<GameObject> positionOrder = new();
        private readonly List<GameObject> registeredVehicles = new();

        internal IReadOnlyList<GameObject> RegisteredVehicles =>
            registeredVehicles;

        internal void RegisterVehicle(GameObject vehicle)
        {
            lapCounts[vehicle] = 0;
            lastCheckpoint[vehicle] = -1;
            previousCheckpoint[vehicle] = -1;
            registeredVehicles.Add(vehicle);
            positionOrder.Add(vehicle);
        }

        /// <summary>
        /// Returns true if the checkpoint crossing completed a lap.
        /// </summary>
        internal bool OnCheckpointPassed(
            GameObject vehicle,
            int checkpointIndex,
            int totalCheckpoints)
        {
            int prevCheckpoint = lastCheckpoint.GetValueOrDefault(vehicle, -1);
            previousCheckpoint[vehicle] = prevCheckpoint;
            lastCheckpoint[vehicle] = checkpointIndex;

            bool lapCompleted = false;

            if (checkpointIndex == 0
                && prevCheckpoint == totalCheckpoints - 1)
            {
                lapCounts[vehicle]++;
                lapCompleted = true;
            }

            return lapCompleted;
        }

        internal void RecalculatePositions(
            Func<GameObject, float> getDistance)
        {
            positionOrder.Sort((a, b) =>
            {
                int lapA = lapCounts.GetValueOrDefault(a, 0);
                int lapB = lapCounts.GetValueOrDefault(b, 0);

                if (lapA != lapB)
                    return lapB.CompareTo(lapA);

                int cpA = lastCheckpoint.GetValueOrDefault(a, -1);
                int cpB = lastCheckpoint.GetValueOrDefault(b, -1);

                if (cpA != cpB)
                    return cpB.CompareTo(cpA);

                float distA = getDistance(a);
                float distB = getDistance(b);

                return distB.CompareTo(distA);
            });
        }

        internal int GetPosition(GameObject vehicle)
        {
            int index = positionOrder.IndexOf(vehicle);
            return index >= 0 ? index + 1 : 0;
        }

        internal int GetLapCount(GameObject vehicle)
        {
            return lapCounts.GetValueOrDefault(vehicle, 0);
        }

        internal void Reset()
        {
            lapCounts.Clear();
            lastCheckpoint.Clear();
            previousCheckpoint.Clear();
            positionOrder.Clear();
            registeredVehicles.Clear();
        }
    }
}
