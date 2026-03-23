using System.Collections.Generic;
using UnityEngine;

namespace R8EOX.Race.Internal
{
    internal class RaceTimer
    {
        private float raceStartTime;
        private readonly Dictionary<GameObject, float> bestLapTimes = new();
        private readonly Dictionary<GameObject, float> currentLapStartTime = new();
        private readonly Dictionary<GameObject, float> lastLapTimes = new();

        internal void Start()
        {
            raceStartTime = Time.time;
        }

        internal float GetElapsedTime()
        {
            return Time.time - raceStartTime;
        }

        internal void RegisterVehicle(GameObject vehicle)
        {
            currentLapStartTime[vehicle] = 0f;
        }

        internal void OnRaceStarted()
        {
            foreach (var vehicle in currentLapStartTime.Keys)
            {
                currentLapStartTime[vehicle] = Time.time;
            }
        }

        internal void OnLapCompleted(GameObject vehicle)
        {
            float lapStart =
                currentLapStartTime.GetValueOrDefault(vehicle, raceStartTime);
            float lapTime = Time.time - lapStart;

            lastLapTimes[vehicle] = lapTime;

            if (!bestLapTimes.ContainsKey(vehicle)
                || lapTime < bestLapTimes[vehicle])
            {
                bestLapTimes[vehicle] = lapTime;
            }

            currentLapStartTime[vehicle] = Time.time;
        }

        internal float GetBestLapTime(GameObject vehicle)
        {
            return bestLapTimes.GetValueOrDefault(vehicle, 0f);
        }

        internal float GetCurrentLapTime(GameObject vehicle)
        {
            if (!currentLapStartTime.ContainsKey(vehicle))
                return 0f;

            return Time.time - currentLapStartTime[vehicle];
        }

        internal float GetLastLapTime(GameObject vehicle)
        {
            return lastLapTimes.GetValueOrDefault(vehicle, 0f);
        }

        internal void Reset()
        {
            bestLapTimes.Clear();
            currentLapStartTime.Clear();
            lastLapTimes.Clear();
        }
    }
}
