using System;
using System.Collections;
using UnityEngine;
using R8EOX.App.Internal;
using R8EOX.Session;

namespace R8EOX.App
{
    /// <summary>
    /// Persistent root manager for the entire application.
    /// Lives on the [AppRoot] GameObject and survives scene loads.
    /// Provides the public API for loading tracks and returning to the main menu.
    /// </summary>
    public class AppManager : MonoBehaviour
    {
        [Header("Channel")]
        [SerializeField] private SessionChannel sessionChannel;

        private AppState currentState;
        private SceneLoader sceneLoader;
        private GameObject sessionManagerObject;
        private SessionManager sessionManager;

        // ----- Public API -----

        /// <summary>Read-only access to the SessionChannel for MenuManager to get registries.</summary>
        public SessionChannel SessionChannel => sessionChannel;

        /// <summary>Current application state.</summary>
        public AppState CurrentState => currentState;

        // ----- Events -----

        /// <summary>Invoked with progress 0-1 while a track scene is loading.</summary>
        public Action<float> OnLoadProgress;

        /// <summary>Invoked once the track scene has finished loading.</summary>
        public Action OnLoadComplete;

        /// <summary>Invoked if scene loading fails, with an error message.</summary>
        public Action<string> OnLoadError;

        // ----- Lifecycle -----

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            sceneLoader = new SceneLoader();
            currentState = AppState.Boot;
        }

        private void Start()
        {
            currentState = AppState.Menu;
            sceneLoader.LoadScene("MainMenu");
        }

        // ----- Public Methods -----

        /// <summary>
        /// Loads a track scene and begins a session in the given mode.
        /// Creates a [SessionManager] child GO, wires the channel, calls BeginSession,
        /// then loads the track scene async so SessionBootstrapper can call OnSceneReady.
        /// </summary>
        public void LoadTrack(TrackDefinition trackDef, SessionMode mode)
        {
            if (trackDef == null)
            {
                Debug.LogError("[AppManager] LoadTrack called with null TrackDefinition.");
                return;
            }

            if (string.IsNullOrEmpty(trackDef.SceneName))
            {
                Debug.LogError($"[AppManager] TrackDefinition '{trackDef.DisplayName}' has no SceneName.");
                return;
            }

            currentState = AppState.Loading;

            SessionConfig config = SessionConfig.CreateRuntime(
                mode,
                null,
                trackDef.SceneName);

            if (sessionChannel != null)
            {
                sessionChannel.SetSession(config);
            }

            sessionManagerObject = new GameObject("[SessionManager]");
            sessionManagerObject.transform.SetParent(transform);
            sessionManager = sessionManagerObject.AddComponent<SessionManager>();
            sessionManager.SetSessionChannel(sessionChannel);
            sessionManager.BeginSession(config);

            StartCoroutine(sceneLoader.LoadSceneAsync(
                trackDef.SceneName,
                onProgress: HandleLoadProgress,
                onComplete: HandleLoadComplete,
                onError: HandleLoadError));
        }

        /// <summary>
        /// Ends the current session, destroys the SessionManager GO, and returns to the main menu.
        /// </summary>
        public void ReturnToMenu()
        {
            Time.timeScale = 1f;

            if (sessionManager != null)
            {
                sessionManager.EndSession();
            }

            if (sessionManagerObject != null)
            {
                Destroy(sessionManagerObject);
                sessionManagerObject = null;
                sessionManager = null;
            }

            currentState = AppState.Menu;
            sceneLoader.LoadScene("MainMenu");
        }

        // ----- Private Handlers -----

        private void HandleLoadProgress(float progress)
        {
            OnLoadProgress?.Invoke(progress);
        }

        private void HandleLoadComplete()
        {
            currentState = AppState.InGame;
            OnLoadComplete?.Invoke();
        }

        private void HandleLoadError(string msg)
        {
            Debug.LogError($"[AppManager] Scene load error: {msg}");
            OnLoadError?.Invoke(msg);
        }
    }
}
