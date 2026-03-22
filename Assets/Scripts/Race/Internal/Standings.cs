using System.Collections.Generic;
using UnityEngine;

namespace R8EOX.Race.Internal
{
    internal class Standings
    {
        private Dictionary<GameObject, int> lapCounts = new();
        private Dictionary<GameObject, int> lastCheckpoint = new();
        private List<GameObject> positionOrder = new();

        internal void RegisterVehicle(GameObject vehicle)
        {
            lapCounts[vehicle] = 0;
            lastCheckpoint[vehicle] = -1;
        }

        internal void OnCheckpointPassed(GameObject vehicle, int checkpointIndex, int totalCheckpoints)
        {
            lastCheckpoint[vehicle] = checkpointIndex;
            if (checkpointIndex == 0 && lastCheckpoint[vehicle] != -1)
            {
                lapCounts[vehicle]++;
            }
            // TODO: Recalculate position order
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
            positionOrder.Clear();
        }
    }
}
