using System.Reflection;
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
                trackManager, raceManager, cameraManager);

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
                trackManager, raceManager, cameraManager);

            var config = CreateDefaultConfig();
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

        /// <summary>
        /// Creates a runtime SessionConfig for editor-play.
        /// Defaults are Practice mode, 0 laps, 0 AI — which is
        /// exactly what we want. The vehicle prefab is set via
        /// reflection because the field is private with no setter.
        /// This runs once at startup and avoids modifying
        /// SessionConfig's public API.
        /// </summary>
        private SessionConfig CreateDefaultConfig()
        {
            var config =
                ScriptableObject.CreateInstance<SessionConfig>();
            config.name = "EditorPlayConfig";

            if (defaultVehiclePrefab != null)
            {
                SetPrivateField(
                    config, "vehiclePrefab", defaultVehiclePrefab);
            }
            else
            {
                Debug.LogWarning(
                    "[SessionBootstrapper] No default vehicle " +
                    "prefab assigned. Vehicle spawning will be " +
                    "skipped.");
            }

            return config;
        }

        private static void SetPrivateField(
            object target, string fieldName, object value)
        {
            // Combine NonPublic + instance-level flags.
            const BindingFlags flags =
                BindingFlags.NonPublic |
                (BindingFlags)(1 << 2); // BindingFlags 4 = instance member

            var field = target.GetType().GetField(
                fieldName, flags);

            if (field != null)
            {
                field.SetValue(target, value);
            }
            else
            {
                Debug.LogError(
                    $"[SessionBootstrapper] Could not find " +
                    $"field '{fieldName}' on " +
                    $"{target.GetType().Name}.");
            }
        }
    }
}
