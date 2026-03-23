using UnityEngine;

namespace R8EOX.Session.Internal
{
    /// <summary>
    /// Placed in every gameplay scene. On Awake, detects whether we
    /// arrived via menu flow (SessionChannel has an active session)
    /// or via direct editor-play (no active session). In the latter
    /// case it creates a SessionManager and a default Practice config
    /// so the scene is playable without going through the menu.
    /// </summary>
    internal class SessionBootstrapper : MonoBehaviour
    {
        private const string ManagerObjectName = "[SessionManager]";

        [Header("Session Channel")]
        [SerializeField] private SessionChannel sessionChannel;

        [Header("Default Vehicle (Editor Play)")]
        [Tooltip(
            "Vehicle prefab spawned when playing directly from " +
            "the editor without menu flow.")]
        [SerializeField] private GameObject defaultVehiclePrefab;

        [Header("Scene References")]
        [SerializeField]
        private R8EOX.Track.TrackManager trackManager;

        [SerializeField]
        private R8EOX.Race.RaceManager raceManager;

        [SerializeField]
        private R8EOX.Camera.CameraManager cameraManager;

        [SerializeField]
        private R8EOX.UI.UIManager uiManager;

        [SerializeField]
        private R8EOX.Audio.AudioManager audioManager;

        [SerializeField]
        private R8EOX.VFX.VFXManager vfxManager;

        [SerializeField]
        private R8EOX.AI.AIManager aiManager;

        private void Awake()
        {
            if (sessionChannel != null && sessionChannel.HasActiveSession)
            {
                HandleFullFlow();
            }
            else
            {
                HandleEditorPlay();
            }
        }

        // -------------------------------------------------------
        //  Full flow — arrived from the menu via SessionManager
        // -------------------------------------------------------

        private void HandleFullFlow()
        {
            var sessionManager = FindExistingSessionManager();

            if (sessionManager == null)
            {
                Debug.LogError(
                    "[SessionBootstrapper] SessionChannel has an " +
                    "active session but no SessionManager was " +
                    "found. Falling back to editor-play flow.");
                HandleEditorPlay();
                return;
            }

            sessionManager.OnSceneReady(
                trackManager, raceManager, cameraManager, uiManager,
                audioManager, vfxManager, aiManager);

            Debug.Log(
                "[SessionBootstrapper] Full flow — passed scene " +
                "refs to existing SessionManager.");
        }

        // -------------------------------------------------------
        //  Editor-play — no menu, create everything on the fly
        // -------------------------------------------------------

        private void HandleEditorPlay()
        {
            Debug.Log(
                "[SessionBootstrapper] Editor-play mode — " +
                "creating SessionManager and default config.");

            var managerGO = new GameObject(ManagerObjectName);
            DontDestroyOnLoad(managerGO);
            var sessionManager =
                managerGO.AddComponent<SessionManager>();

            sessionManager.OnSceneReady(
                trackManager, raceManager, cameraManager, uiManager,
                audioManager, vfxManager, aiManager);
            sessionManager.SetSessionChannel(sessionChannel);

            var registry = sessionChannel != null
                ? sessionChannel.VehicleRegistry : null;
            var overlayRegistry = sessionChannel?.OverlayRegistry;
            bool useOverlay = registry != null && registry.Count > 0
                && overlayRegistry?.VehicleSelectOverlayPrefab != null;

            var config = useOverlay
                ? SessionConfig.CreateRuntime(SessionMode.Practice, null)
                : CreateDefaultConfig();

            sessionManager.BeginSession(config);

            Debug.Log(
                "[SessionBootstrapper] Editor-play session " +
                "started (Practice mode).");
        }

        // -------------------------------------------------------
        //  Helpers
        // -------------------------------------------------------

        private SessionManager FindExistingSessionManager()
        {
            var existing = GameObject.Find(ManagerObjectName);
            if (existing != null)
                return existing.GetComponent<SessionManager>();
            return null;
        }

        private SessionConfig CreateDefaultConfig()
        {
            if (defaultVehiclePrefab == null)
            {
                Debug.LogWarning(
                    "[SessionBootstrapper] No default vehicle " +
                    "prefab assigned. Vehicle spawning will be " +
                    "skipped.");
            }

            return SessionConfig.CreateRuntime(
                SessionMode.Practice,
                defaultVehiclePrefab);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Skip validation on freshly-added component — builder
            // wires all refs after AddComponent via ApplyModifiedProperties
            if (trackManager == null && cameraManager == null
                && raceManager == null)
                return;

            if (trackManager == null)
                Debug.LogError(
                    "[SessionBootstrapper] TrackManager is required " +
                    "but not assigned!", this);
            if (cameraManager == null)
                Debug.LogError(
                    "[SessionBootstrapper] CameraManager is required " +
                    "but not assigned!", this);
            if (sessionChannel == null)
                Debug.LogWarning(
                    "[SessionBootstrapper] SessionChannel not assigned " +
                    "— editor-play will use defaults.", this);
        }
#endif
    }
}
