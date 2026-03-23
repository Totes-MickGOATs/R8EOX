using System.Collections.Generic;
using UnityEngine;
using R8EOX.Session.Internal;
using R8EOX.Track;

namespace R8EOX.Session
{
    /// <summary>
    /// Top-level session orchestrator. Manages lifecycle from begin
    /// through vehicle selection, spawning, wiring, and teardown.
    /// </summary>
    public class SessionManager : MonoBehaviour
    {
        [Header("Channel")]
        [SerializeField] private SessionChannel sessionChannel;

        private TrackManager trackManager;
        private Race.RaceManager raceManager;
        private Camera.CameraManager cameraManager;
        private UI.UIManager uiManager;
        private Audio.AudioManager audioManager;
        private VFX.VFXManager vfxManager;

        private readonly SessionState state = new SessionState();
        private readonly VehicleSpawner vehicleSpawner = new VehicleSpawner();

        private SessionConfig activeConfig;
        private SessionMode effectiveMode;

        private Vector3 swapPosition;
        private Quaternion swapRotation;
        private bool isSwapping;

        /// <summary>True when vehicles are spawned and systems wired.</summary>
        public bool IsReady => state.CurrentPhase == SessionPhase.Ready;
        /// <summary>True when session is active (not Idle).</summary>
        public bool IsActive => state.CurrentPhase != SessionPhase.Idle;
        /// <summary>Player vehicle spawned this session.</summary>
        public GameObject PlayerVehicle => vehicleSpawner.PlayerVehicle;
        /// <summary>Effective mode after track-readiness degradation.</summary>
        public SessionMode EffectiveMode => effectiveMode;

        /// <summary>Provide scene-resident manager references.</summary>
        public void OnSceneReady(
            TrackManager track, Race.RaceManager race,
            Camera.CameraManager cam, UI.UIManager ui = null,
            Audio.AudioManager audio = null, VFX.VFXManager vfx = null)
        {
            trackManager = track;
            raceManager = race;
            cameraManager = cam;
            uiManager = ui;
            audioManager = audio;
            vfxManager = vfx;
            if (uiManager != null) uiManager.SetSessionManager(this);
            if (uiManager != null && race != null) uiManager.SetRaceManager(race);
            if (activeConfig != null) EnterVehicleSelectOrSpawn();
        }

        /// <summary>Begin a new play session with the given config.</summary>
        public void BeginSession(SessionConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[SessionManager] Cannot begin — config is null.");
                return;
            }
            activeConfig = config;
            if (sessionChannel != null) sessionChannel.SetSession(config);
            state.BeginLoading();
            Debug.Log($"[SessionManager] Session starting: mode={config.SessionMode}, vehicle={config.VehiclePrefab?.name ?? "null"}");
            if (trackManager != null) EnterVehicleSelectOrSpawn();
        }

        /// <summary>End the current session and clean up.</summary>
        public void EndSession()
        {
            if (state.CurrentPhase == SessionPhase.Idle) return;
            state.BeginTeardown();
            CleanupOverlay();
            isSwapping = false;
            if (raceManager != null) raceManager.EndRace();
            vehicleSpawner.DestroyAllSpawned();
            if (sessionChannel != null) sessionChannel.Clear();
            activeConfig = null;
            trackManager = null; raceManager = null;
            cameraManager = null; uiManager = null;
            audioManager = null; vfxManager = null;
            state.Reset();
        }

        /// <summary>Swap vehicle mid-session (called by UIManager).</summary>
        public void RequestVehicleSwap()
        {
            if (state.CurrentPhase != SessionPhase.Ready) return;
            var registry = sessionChannel != null
                ? sessionChannel.VehicleRegistry : null;
            if (registry == null || registry.OverlayPrefab == null) return;
            var (pos, rot) = vehicleSpawner.GetPlayerPositionAndRotation();
            swapPosition = pos;
            swapRotation = rot;
            isSwapping = true;
            vehicleSpawner.DestroyPlayerVehicle();
            state.ReturnToVehicleSelect();
            ShowVehicleSelectOverlay(registry);
        }

        // ----- Vehicle selection -----

        private void EnterVehicleSelectOrSpawn()
        {
            var registry = sessionChannel != null
                ? sessionChannel.VehicleRegistry : null;
            if (registry != null && registry.Count > 0
                && registry.OverlayPrefab != null)
            {
                state.BeginVehicleSelect();
                ShowVehicleSelectOverlay(registry);
            }
            else
            {
                state.BeginSpawning();
                SetupSession();
            }
        }

        private void ShowVehicleSelectOverlay(VehicleRegistry registry)
        {
            if (uiManager == null)
            {
                Debug.LogError("[SessionManager] No UIManager — skipping overlay.");
                state.BeginSpawning();
                SetupSession();
                return;
            }
            uiManager.ShowVehicleSelectOverlay(
                registry, OnVehicleSelected, OnVehicleSelectCancelled);
        }

        private void OnVehicleSelected(VehicleDefinition definition)
        {
            activeConfig.SetVehiclePrefab(definition.VehiclePrefab);
            CleanupOverlay();
            state.EndVehicleSelect();
            if (isSwapping)
            {
                isSwapping = false;
                vehicleSpawner.SpawnPlayerVehicleAt(
                    definition.VehiclePrefab, swapPosition, swapRotation);
                WirePlayerVehicle();
                state.MarkReady();
            }
            else
            {
                SetupSession();
            }
        }

        private void OnVehicleSelectCancelled()
        {
            CleanupOverlay();
            if (isSwapping)
            {
                isSwapping = false;
                vehicleSpawner.SpawnPlayerVehicleAt(
                    activeConfig.VehiclePrefab, swapPosition, swapRotation);
                WirePlayerVehicle();
                state.EndVehicleSelect();
                state.MarkReady();
            }
            else
            {
                state.BeginSpawning();
                SetupSession();
            }
        }

        private void CleanupOverlay()
        {
            if (uiManager != null) uiManager.CleanupVehicleSelectOverlay();
        }

        // ----- Session setup -----

        private void Update()
        {
            if (effectiveMode == SessionMode.Race
                && raceManager != null
                && state.CurrentPhase == SessionPhase.Ready)
            {
                raceManager.Tick(Time.deltaTime);
            }
        }

        private void SetupSession()
        {
            if (activeConfig == null || trackManager == null) return;
            InitializeTrack();
            if (raceManager != null) raceManager.Initialize(trackManager);
            ValidateAndDegradeMode();
            if (activeConfig.VehiclePrefab == null)
            {
                Debug.LogError("[SessionManager] No vehicle prefab in SessionConfig!");
                return;
            }
            SpawnPlayer();
            ValidateVehicle();
            SpawnAIOpponents();
            WirePlayerVehicle();
            StartRaceIfApplicable();
            state.MarkReady();
            Debug.Log(
                $"[SessionManager] Session ready: mode={effectiveMode}, "
                + $"vehicles={vehicleSpawner.SpawnedCount}");
        }

        private void InitializeTrack() => trackManager.Initialize(null);

        private void ValidateAndDegradeMode()
        {
            var readiness = TrackValidator.Validate(trackManager);
            var trackType = TrackType.Circuit; // TODO: read from TrackManager
            effectiveMode = TrackValidator.DegradeMode(
                activeConfig.SessionMode, readiness, trackType);
            if (effectiveMode != activeConfig.SessionMode)
            {
                Debug.LogWarning(
                    $"[SessionManager] Mode degraded from "
                    + $"{activeConfig.SessionMode} to {effectiveMode}.");
            }
        }

        private void SpawnPlayer()
        {
            var playerSpawn = trackManager.GetPlayerSpawnPoint();
            if (trackManager.GetSpawnPointCount() <= 0)
            {
                Debug.LogWarning("[SessionManager] No spawn points! Spawning at origin.");
                playerSpawn = new SpawnPointData
                {
                    Index = 0, Position = Vector3.zero,
                    Rotation = Quaternion.identity, IsPlayerSpawn = true
                };
            }
            vehicleSpawner.SpawnPlayerVehicle(activeConfig.VehiclePrefab, playerSpawn);
            if (raceManager != null && vehicleSpawner.PlayerVehicle != null)
                raceManager.RegisterVehicle(vehicleSpawner.PlayerVehicle);
        }

        private void ValidateVehicle()
        {
            var vehicle = vehicleSpawner.PlayerVehicle;
            if (vehicle == null) return;
            var readiness = VehicleValidator.Validate(vehicle);
            VehicleValidator.LogReadiness(readiness, vehicle.name);
        }

        private void SpawnAIOpponents()
        {
            if (effectiveMode != SessionMode.Race) return;
            if (activeConfig.AiOpponentCount <= 0) return;
            var allSpawns = trackManager.GetSpawnPoints();
            var aiSpawns = new List<SpawnPointData>();
            foreach (var sp in allSpawns)
            {
                if (!sp.IsPlayerSpawn) aiSpawns.Add(sp);
            }
            var aiVehicles = vehicleSpawner.SpawnAIVehicles(
                activeConfig.VehiclePrefab, aiSpawns.ToArray(),
                activeConfig.AiOpponentCount);
            if (raceManager != null)
            {
                foreach (var vehicle in aiVehicles)
                    raceManager.RegisterVehicle(vehicle);
            }
        }

        private void WirePlayerVehicle()
        {
            var player = vehicleSpawner.PlayerVehicle;
            if (player == null) return;
            if (cameraManager != null) cameraManager.SetTarget(player.transform);
            if (audioManager != null) audioManager.SetTarget(player);
            if (vfxManager != null) vfxManager.SetTarget(player);
        }

        private void StartRaceIfApplicable()
        {
            if (effectiveMode != SessionMode.Race || raceManager == null) return;
            raceManager.StartRace();
        }

        private void OnDestroy()
        {
            CleanupOverlay();
            if (state.CurrentPhase != SessionPhase.Idle) EndSession();
        }
    }
}
