using System.Collections.Generic;
using UnityEngine;
using R8EOX.Session.Internal;
using R8EOX.Track;

namespace R8EOX.Session
{
    /// <summary>
    /// Top-level session orchestrator. Manages the lifecycle from
    /// "player wants to play" through vehicle spawning, system wiring,
    /// and teardown. Called by SessionBootstrapper or UI flow.
    /// </summary>
    public class SessionManager : MonoBehaviour
    {
        [Header("Channel")]
        [SerializeField] private SessionChannel sessionChannel;

        // Scene references — set by SessionBootstrapper after load
        private TrackManager trackManager;
        private Race.RaceManager raceManager;
        private Camera.CameraManager cameraManager;

        // Internal components
        private readonly SessionState state = new SessionState();
        private readonly VehicleSpawner vehicleSpawner =
            new VehicleSpawner();

        // Active session
        private SessionConfig activeConfig;
        private SessionMode effectiveMode;

        /// <summary>True when vehicles are spawned and systems wired.</summary>
        public bool IsReady =>
            state.CurrentPhase == SessionPhase.Ready;

        /// <summary>True when session is active (not Idle).</summary>
        public bool IsActive =>
            state.CurrentPhase != SessionPhase.Idle;

        /// <summary>Player vehicle spawned this session.</summary>
        public GameObject PlayerVehicle => vehicleSpawner.PlayerVehicle;

        /// <summary>Effective mode after track-readiness degradation.</summary>
        public SessionMode EffectiveMode => effectiveMode;

        /// <summary>
        /// Called by SessionBootstrapper after the track scene loads
        /// to provide scene-resident manager references.
        /// </summary>
        public void OnSceneReady(
            TrackManager track,
            Race.RaceManager race,
            Camera.CameraManager cam)
        {
            trackManager = track;
            raceManager = race;
            cameraManager = cam;

            if (activeConfig != null)
            {
                state.BeginSpawning();
                SetupSession();
            }
        }

        /// <summary>
        /// Begin a new play session. For editor-play the bootstrapper
        /// calls this immediately; for full flow, UI calls before
        /// scene loading.
        /// </summary>
        public void BeginSession(SessionConfig config)
        {
            if (config == null)
            {
                Debug.LogError(
                    "[SessionManager] Cannot begin session "
                    + "— config is null.");
                return;
            }

            activeConfig = config;

            if (sessionChannel != null)
                sessionChannel.SetSession(config);

            state.BeginLoading();
            Debug.Log(
                "[SessionManager] Session starting: "
                + $"mode={config.SessionMode}, "
                + $"vehicle={config.VehiclePrefab?.name ?? "null"}");

            // Scene refs already available (editor-play / same-scene)
            if (trackManager != null)
            {
                state.BeginSpawning();
                SetupSession();
            }
        }

        /// <summary>
        /// End the current session, destroy spawned vehicles, reset.
        /// </summary>
        public void EndSession()
        {
            if (state.CurrentPhase == SessionPhase.Idle)
                return;

            state.BeginTeardown();

            if (raceManager != null)
                raceManager.EndRace();

            vehicleSpawner.DestroyAllSpawned();

            if (sessionChannel != null)
                sessionChannel.Clear();

            activeConfig = null;
            trackManager = null;
            raceManager = null;
            cameraManager = null;

            state.Reset();

            Debug.Log("[SessionManager] Session ended.");
        }

        private void SetupSession()
        {
            if (activeConfig == null || trackManager == null)
                return;

            InitializeTrack();
            ValidateAndDegradeMode();

            if (activeConfig.VehiclePrefab == null)
            {
                Debug.LogError(
                    "[SessionManager] No vehicle prefab "
                    + "in SessionConfig!");
                return;
            }

            SpawnPlayer();
            SpawnAIOpponents();
            WireCamera();
            StartRaceIfApplicable();

            state.MarkReady();
            Debug.Log(
                "[SessionManager] Session ready: "
                + $"mode={effectiveMode}, "
                + $"vehicles={vehicleSpawner.SpawnedCount}");
        }

        private void InitializeTrack()
        {
            // TrackManager uses its own serialized TrackConfig.
            // Pass null so Initialize still discovers SpawnPoints.
            trackManager.Initialize(null);
        }

        private void ValidateAndDegradeMode()
        {
            TrackReadiness readiness =
                TrackValidator.Validate(trackManager);

            // TODO: read actual TrackType from TrackManager.Config
            // when a public property is added. Default to Circuit.
            TrackType trackType = TrackType.Circuit;

            effectiveMode = TrackValidator.DegradeMode(
                activeConfig.SessionMode, readiness, trackType);

            if (effectiveMode != activeConfig.SessionMode)
            {
                Debug.LogWarning(
                    "[SessionManager] Mode degraded from "
                    + $"{activeConfig.SessionMode} "
                    + $"to {effectiveMode}.");
            }
        }

        private void SpawnPlayer()
        {
            SpawnPointData playerSpawn =
                trackManager.GetPlayerSpawnPoint();

            bool hasSpawns =
                trackManager.GetSpawnPointCount() > 0;

            if (!hasSpawns)
            {
                Debug.LogWarning(
                    "[SessionManager] No spawn points found! "
                    + "Spawning at origin.");
                playerSpawn = new SpawnPointData
                {
                    Index = 0,
                    Position = Vector3.zero,
                    Rotation = Quaternion.identity,
                    IsPlayerSpawn = true
                };
            }

            vehicleSpawner.SpawnPlayerVehicle(
                activeConfig.VehiclePrefab, playerSpawn);
        }

        private void SpawnAIOpponents()
        {
            if (effectiveMode != SessionMode.Race)
                return;
            if (activeConfig.AiOpponentCount <= 0)
                return;

            SpawnPointData[] allSpawns =
                trackManager.GetSpawnPoints();
            var aiSpawns = new List<SpawnPointData>();
            foreach (SpawnPointData sp in allSpawns)
            {
                if (!sp.IsPlayerSpawn)
                    aiSpawns.Add(sp);
            }

            vehicleSpawner.SpawnAIVehicles(
                activeConfig.VehiclePrefab,
                aiSpawns.ToArray(),
                activeConfig.AiOpponentCount);
        }

        private void WireCamera()
        {
            if (cameraManager == null)
                return;
            if (vehicleSpawner.PlayerVehicle == null)
                return;

            cameraManager.SetTarget(
                vehicleSpawner.PlayerVehicle.transform);
        }

        private void StartRaceIfApplicable()
        {
            if (effectiveMode != SessionMode.Race)
                return;
            if (raceManager == null)
                return;

            raceManager.StartRace();
        }

        private void OnDestroy()
        {
            if (state.CurrentPhase != SessionPhase.Idle)
                EndSession();
        }
    }
}
