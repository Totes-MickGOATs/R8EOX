using System.Collections.Generic;
using UnityEngine;

namespace R8EOX.Race.Internal
{
    internal class RaceTimer
    {
        private float raceStartTime;
        private Dictionary<GameObject, float> bestLapTimes = new();
        private Dictionary<GameObject, float> currentLapStartTime = new();

        internal void Start()
        {
            raceStartTime = Time.time;
        }

        internal float GetElapsedTime()
        {
            return Time.time - raceStartTime;
        }

        internal void OnLapCompleted(GameObject vehicle)
        {
            float lapTime = Time.time - currentLapStartTime.GetValueOrDefault(vehicle, raceStartTime);
            if (!bestLapTimes.ContainsKey(vehicle) || lapTime < bestLapTimes[vehicle])
            {
                bestLapTimes[vehicle] = lapTime;
            }
            currentLapStartTime[vehicle] = Time.time;
        }

        internal float GetBestLapTime(GameObject vehicle)
        {
            return bestLapTimes.GetValueOrDefault(vehicle, 0f);
        }

        internal void Reset()
        {
            bestLapTimes.Clear();
            currentLapStartTime.Clear();
        }
    }
}
