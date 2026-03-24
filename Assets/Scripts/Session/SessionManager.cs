using System.Collections.Generic;
using UnityEngine;
using R8EOX.Diagnostics;
using R8EOX.Session.Internal;
using R8EOX.Track;

namespace R8EOX.Session
{
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
        private AI.AIManager aiManager;

        private readonly SessionState state = new SessionState();
        private readonly VehicleSpawner vehicleSpawner = new VehicleSpawner();

        private SessionConfig activeConfig;
        private SessionMode effectiveMode;
        private System.Action onAborted;

        private Vector3 swapPosition;
        private Quaternion swapRotation;
        private bool isSwapping;
        private SetupErrorOverlay errorOverlay;

        public bool IsReady => state.CurrentPhase == SessionPhase.Ready;
        public bool IsActive => state.CurrentPhase != SessionPhase.Idle;
        public GameObject PlayerVehicle => vehicleSpawner.PlayerVehicle;
        public SessionMode EffectiveMode => effectiveMode;

        public void OnSceneReady(
            TrackManager track, Race.RaceManager race,
            Camera.CameraManager cam, UI.UIManager ui = null,
            Audio.AudioManager audio = null, VFX.VFXManager vfx = null,
            AI.AIManager ai = null)
        {
            trackManager = track; raceManager = race; cameraManager = cam;
            uiManager = ui; audioManager = audio; vfxManager = vfx; aiManager = ai;
            if (uiManager != null)
            {
                uiManager.SetSessionManager(this);
                if (race != null) uiManager.SetRaceManager(race);
            }
            Diag.FlowStep("TrackLoad", "TrackSystemsReady");
            if (activeConfig != null) EnterVehicleSelectOrSpawn();
        }

        public void SetSessionChannel(SessionChannel channel) { sessionChannel = channel; }
        public void SetAbortCallback(System.Action callback) { onAborted = callback; }
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
            if (trackManager != null) EnterVehicleSelectOrSpawn();
        }

        public void EndSession()
        {
            if (state.CurrentPhase == SessionPhase.Idle) return;
            Diag.Log(DiagChannel.Session, "EndSession start");
            state.BeginTeardown();
            if (errorOverlay != null) { Destroy(errorOverlay); errorOverlay = null; }
            CleanupOverlay();
            isSwapping = false;
            if (aiManager != null) aiManager.RemoveAllDrivers();
            if (raceManager != null) raceManager.EndRace();
            // Clear telemetry consumers before destroying vehicles to prevent
            // LateUpdate MissingReferenceException on destroyed Rigidbody/VehicleManager.
            if (audioManager != null) audioManager.SetTarget(null);
            if (vfxManager != null) vfxManager.SetTarget(null);
            vehicleSpawner.DestroyAllSpawned();
            if (sessionChannel != null) sessionChannel.Clear();
            activeConfig = null;
            trackManager = null; raceManager = null; cameraManager = null;
            uiManager = null; audioManager = null; vfxManager = null; aiManager = null;
            state.Reset();
        }

        public void RequestVehicleSwap()
        {
            if (state.CurrentPhase != SessionPhase.Ready) return;
            var registry = sessionChannel != null
                ? sessionChannel.VehicleRegistry : null;
            if (registry == null
                || sessionChannel?.OverlayRegistry?.VehicleSelectOverlayPrefab == null) return;
            var (pos, rot) = vehicleSpawner.GetPlayerPositionAndRotation();
            swapPosition = pos;
            swapRotation = rot;
            isSwapping = true;
            vehicleSpawner.DestroyPlayerVehicle();
            state.ReturnToVehicleSelect();
            ShowVehicleSelectOverlay(registry);
        }

        private void EnterVehicleSelectOrSpawn()
        {
            var registry = sessionChannel != null
                ? sessionChannel.VehicleRegistry : null;
            var overlayPrefab = sessionChannel?.OverlayRegistry?.VehicleSelectOverlayPrefab;
            Diag.Log(DiagChannel.Session, $"EnterVehicleSelectOrSpawn: registry={registry != null} count={registry?.Count ?? 0} overlayPrefab={overlayPrefab != null}");
            if (registry != null && registry.Count > 0 && overlayPrefab != null)
            {
                Diag.FlowStep("TrackLoad", "VehicleSelectStarted");
                Diag.BeginFlow("VehicleSelect");
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
            var prefab = sessionChannel.OverlayRegistry.VehicleSelectOverlayPrefab;
            uiManager.ShowVehicleSelectOverlay(
                prefab, registry, OnVehicleSelected, OnVehicleSelectCancelled);
            Diag.FlowStep("VehicleSelect", "OverlayInstantiated");
            Diag.FlowStep("VehicleSelect", "OverlayVisible");
        }

        private void OnVehicleSelected(VehicleDefinition definition)
        {
            activeConfig.SetVehiclePrefab(definition.VehiclePrefab);
            Diag.FlowStep("VehicleSelect", "ConfirmPressed");
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
                onAborted?.Invoke();
            }
        }

        private void CleanupOverlay()
        {
            if (uiManager != null) uiManager.CleanupVehicleSelectOverlay();
            Diag.FlowStep("VehicleSelect", "OverlayDismissed");
            Diag.Log(DiagChannel.Session, "CleanupOverlay completed");
        }

        private void Update()
        {
            if (state.CurrentPhase != SessionPhase.Ready) return;
            if (aiManager != null) aiManager.Tick(Time.deltaTime);
            if (effectiveMode == SessionMode.Race && raceManager != null)
                raceManager.Tick(Time.deltaTime);
        }

        private void SetupSession()
        {
            if (activeConfig == null || trackManager == null) return;
            Diag.Log(DiagChannel.Session, "SetupSession start");
            CreateErrorOverlay();
            InitializeTrack();
            Diag.Log(DiagChannel.Session, "Track initialized");
            if (raceManager != null) raceManager.Initialize(trackManager);
            if (aiManager != null) aiManager.Initialize(trackManager);
            ValidateAndDegradeMode();
            SetupValidator.Validate(errorOverlay, trackManager,
                cameraManager, raceManager, uiManager,
                audioManager, vfxManager, aiManager, effectiveMode);
            if (activeConfig.VehiclePrefab == null)
            {
                errorOverlay.AddError("Vehicle",
                    "No vehicle prefab in SessionConfig",
                    "Assign a vehicle prefab to SessionConfig or VehicleDefinition");
                return;
            }
            SpawnPlayer();
            Diag.FlowStep("TrackLoad", "VehicleSpawned");
            SpawnAIOpponents();
            WirePlayerVehicle();
            Diag.FlowStep("TrackLoad", "CameraTracking");
            StartRaceIfApplicable();
            state.MarkReady();
            Diag.FlowStep("TrackLoad", "SessionReady");
            Diag.Log(DiagChannel.Session, $"Session ready: mode={effectiveMode}");
        }

        private void CreateErrorOverlay()
        {
            errorOverlay = gameObject.GetComponent<SetupErrorOverlay>();
            if (errorOverlay == null)
                errorOverlay = gameObject.AddComponent<SetupErrorOverlay>();
        }

        private void InitializeTrack() => trackManager.Initialize();

        private void ValidateAndDegradeMode()
        {
            var readiness = TrackValidator.Validate(trackManager);
            var trackType = trackManager.GetTrackType();
            effectiveMode = TrackValidator.DegradeMode(
                activeConfig.SessionMode, readiness, trackType);
            if (effectiveMode != activeConfig.SessionMode)
                Debug.LogWarning($"[SessionManager] Mode degraded from {activeConfig.SessionMode} to {effectiveMode}.");
        }

        private void SpawnPlayer()
        {
            var playerSpawn = trackManager.GetPlayerSpawnPoint();
            if (trackManager.GetSpawnPointCount() <= 0)
            {
                Debug.LogWarning("[SessionManager] No spawn points! Spawning at origin.");
                playerSpawn = new SpawnPointData { Index = 0, Position = Vector3.up * 5f, Rotation = Quaternion.identity, IsPlayerSpawn = true };
            }
            vehicleSpawner.SpawnPlayerVehicle(activeConfig.VehiclePrefab, playerSpawn);
            if (raceManager != null && vehicleSpawner.PlayerVehicle != null)
                raceManager.RegisterVehicle(vehicleSpawner.PlayerVehicle);
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
            foreach (var vehicle in aiVehicles)
            { if (raceManager != null) raceManager.RegisterVehicle(vehicle); if (aiManager != null) aiManager.RegisterDriver(vehicle); }
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
