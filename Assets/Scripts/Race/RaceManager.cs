using System;
using System.Collections.Generic;
using UnityEngine;
using R8EOX.Race.Internal;
using R8EOX.Track;

namespace R8EOX.Race
{
    public class RaceManager : MonoBehaviour
    {
        [SerializeField] private RaceConfig config;

        private TrackManager trackManager;
        private RaceState state;
        private Standings standings;
        private RaceTimer timer;
        private RacePhase previousPhase;

        public event Action<RacePhase> OnPhaseChanged;
        public event Action<GameObject, int> OnLapCompleted;

        public void Initialize(TrackManager track)
        {
            trackManager = track;
            state = new RaceState();
            standings = new Standings();
            timer = new RaceTimer();
            previousPhase = RacePhase.PreRace;

            if (trackManager != null)
                trackManager.OnCheckpointPassed += OnCheckpointPassed;
        }

        public void RegisterVehicle(GameObject vehicle)
        {
            if (vehicle == null) return;
            standings.RegisterVehicle(vehicle);
            timer.RegisterVehicle(vehicle);
        }

        public void StartRace()
        {
            if (config == null)
            {
                Debug.LogError(
                    "[RaceManager] Cannot start — no RaceConfig assigned.");
                return;
            }

            state.BeginCountdown(config.CountdownDuration);
            FirePhaseChange();
            Debug.Log(
                $"[RaceManager] Countdown started: "
                + $"{config.CountdownDuration}s");
        }

        public void Tick(float deltaTime)
        {
            if (state == null) return;

            RacePhase phaseBefore = state.CurrentPhase;
            state.Tick(deltaTime);
            RacePhase phaseAfter = state.CurrentPhase;

            if (phaseBefore == RacePhase.Countdown
                && phaseAfter == RacePhase.Racing)
            {
                timer.Start();
                timer.OnRaceStarted();
                FirePhaseChange();
                Debug.Log("[RaceManager] Race started!");
            }

            if (phaseAfter == RacePhase.Racing)
            {
                RecalculatePositions();
                CheckMaxRaceTime();
                CheckAllVehiclesFinished();
            }
        }

        public void EndRace()
        {
            if (state == null) return;
            if (state.CurrentPhase == RacePhase.Finished) return;

            state.Finish();
            FirePhaseChange();

            if (trackManager != null)
                trackManager.OnCheckpointPassed -= OnCheckpointPassed;

            Debug.Log(
                $"[RaceManager] Race ended. Time: "
                + $"{timer.GetElapsedTime():F2}s");
        }

        public void OnCheckpointPassed(
            int checkpointIndex,
            GameObject vehicle)
        {
            if (state == null
                || state.CurrentPhase != RacePhase.Racing)
                return;

            int totalCheckpoints = trackManager.GetCheckpointCount();
            bool lapCompleted = standings.OnCheckpointPassed(
                vehicle, checkpointIndex, totalCheckpoints);

            if (lapCompleted)
            {
                int newLap = standings.GetLapCount(vehicle);
                timer.OnLapCompleted(vehicle);
                OnLapCompleted?.Invoke(vehicle, newLap);
                Debug.Log(
                    $"[RaceManager] {vehicle.name} completed lap "
                    + $"{newLap}/{config.TotalLaps}");

                if (newLap >= config.TotalLaps)
                    CheckAllVehiclesFinished();
            }
        }

        public int GetVehiclePosition(GameObject vehicle)
        {
            return standings?.GetPosition(vehicle) ?? 0;
        }

        public float GetRaceTime()
        {
            return timer?.GetElapsedTime() ?? 0f;
        }

        public int GetLapCount(GameObject vehicle)
        {
            return standings?.GetLapCount(vehicle) ?? 0;
        }

        public int GetTotalLaps()
        {
            return config != null ? config.TotalLaps : 0;
        }

        public RacePhase GetCurrentPhase()
        {
            return state?.CurrentPhase ?? RacePhase.PreRace;
        }

        public float GetCountdownRemaining()
        {
            return state?.CountdownRemaining ?? 0f;
        }

        public float GetBestLapTime(GameObject vehicle)
        {
            return timer?.GetBestLapTime(vehicle) ?? 0f;
        }

        public float GetCurrentLapTime(GameObject vehicle)
        {
            return timer?.GetCurrentLapTime(vehicle) ?? 0f;
        }

        public float GetLastLapTime(GameObject vehicle)
        {
            return timer?.GetLastLapTime(vehicle) ?? 0f;
        }

        private void RecalculatePositions()
        {
            if (trackManager == null || !trackManager.HasCenterline())
                return;

            standings.RecalculatePositions(v =>
                trackManager.GetDistanceAlongTrack(v.transform.position));
        }

        private void CheckMaxRaceTime()
        {
            if (config == null) return;

            if (timer.GetElapsedTime() >= config.MaxRaceTime)
            {
                Debug.Log("[RaceManager] Max race time reached.");
                EndRace();
            }
        }

        private void CheckAllVehiclesFinished()
        {
            var vehicles = standings.RegisteredVehicles;
            if (vehicles.Count == 0) return;

            foreach (var vehicle in vehicles)
            {
                if (standings.GetLapCount(vehicle) < config.TotalLaps)
                    return;
            }

            Debug.Log(
                "[RaceManager] All vehicles finished all laps.");
            EndRace();
        }

        private void FirePhaseChange()
        {
            RacePhase current = state.CurrentPhase;
            if (current != previousPhase)
            {
                previousPhase = current;
                OnPhaseChanged?.Invoke(current);
            }
        }

        private void OnDestroy()
        {
            if (trackManager != null)
                trackManager.OnCheckpointPassed -= OnCheckpointPassed;
        }
    }
}
