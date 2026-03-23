using System;
using UnityEngine;
using R8EOX.Menu.Internal;

namespace R8EOX.Menu
{
    /// <summary>
    /// Top-level controller for all menu screens. Lives in the MainMenu scene.
    /// This is the public API for the Menu system — all screen transitions are
    /// coordinated here. Internal screen classes are accessed only through this manager.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private SplashScreen splashScreen;
        [SerializeField] private MainMenuScreen mainMenuScreen;
        [SerializeField] private ModeSelectScreen modeSelectScreen;
        [SerializeField] private TrackSelectScreen trackSelectScreen;
        [SerializeField] private TrackLoadingScreen loadingScreen;

        [Header("Config")]
        [Tooltip("Duration in seconds for screen transition animations.")]
        [SerializeField] private float transitionDuration = 0.3f;

        private MenuNavigator navigator;
        private R8EOX.App.AppManager appManager;
        private R8EOX.Settings.SettingsManager settingsManager;
        private SessionMode selectedMode;
        private GameObject optionsOverlayInstance;

        /// <summary>
        /// Persists across scene loads so the splash screen is only shown once per session.
        /// MenuManager is destroyed when leaving MainMenu, but this static field survives.
        /// </summary>
        private static bool skipSplash;

        private void Start()
        {
            var root = GameObject.Find("[AppRoot]");
            appManager = root?.GetComponent<R8EOX.App.AppManager>();
            settingsManager = root?.GetComponent<R8EOX.Settings.SettingsManager>();

            if (appManager == null)
            {
                Debug.LogError("[MenuManager] AppManager not found on [AppRoot]. Menu cannot initialize.");
                enabled = false;
                return;
            }

            navigator = new MenuNavigator(transitionDuration);

            InitializeScreens();
            HideAllScreensImmediate();
            SubscribeToAppEvents();

            if (skipSplash)
            {
                navigator.PushScreen(mainMenuScreen);
            }
            else
            {
                navigator.PushScreen(splashScreen);
            }

            skipSplash = true;
        }

        private void OnDestroy()
        {
            if (appManager != null)
            {
                appManager.OnLoadProgress -= HandleLoadProgress;
                appManager.OnLoadComplete -= HandleLoadComplete;
                appManager.OnLoadError -= HandleLoadError;
            }
        }

        // ------------------------------------------------------------------ //
        // Setup helpers
        // ------------------------------------------------------------------ //

        private void InitializeScreens()
        {
            splashScreen.Initialize(OnSplashComplete);
            mainMenuScreen.Initialize(OnPlayPressed, OnOptionsPressed, OnQuitPressed);
            modeSelectScreen.Initialize(OnModeSelected, OnModeBackPressed);
            // trackSelectScreen is initialized in OnModeSelected once the mode is known
            loadingScreen.ResetProgress();
        }

        private void HideAllScreensImmediate()
        {
            splashScreen.HideImmediate();
            mainMenuScreen.HideImmediate();
            modeSelectScreen.HideImmediate();
            trackSelectScreen.HideImmediate();
            loadingScreen.HideImmediate();
        }

        private void SubscribeToAppEvents()
        {
            appManager.OnLoadProgress += HandleLoadProgress;
            appManager.OnLoadComplete += HandleLoadComplete;
            appManager.OnLoadError += HandleLoadError;
        }

        // ------------------------------------------------------------------ //
        // Screen callbacks
        // ------------------------------------------------------------------ //

        private void OnSplashComplete()
        {
            navigator.ReplaceScreen(mainMenuScreen);
        }

        private void OnPlayPressed()
        {
            navigator.PushScreen(modeSelectScreen);
        }

        private void OnQuitPressed()
        {
            // MainMenuScreen handles the quit action internally (e.g. Application.Quit).
        }

        private void OnOptionsPressed()
        {
            if (settingsManager == null)
            {
                Debug.LogWarning("[MenuManager] No SettingsManager found on [AppRoot].");
                return;
            }

            var overlayRegistry = appManager.SessionChannel.OverlayRegistry;
            if (overlayRegistry == null || overlayRegistry.OptionsOverlayPrefab == null)
            {
                optionsOverlayInstance = new GameObject("[OptionsOverlay]");
                var overlay = optionsOverlayInstance.AddComponent<R8EOX.UI.Internal.OptionsOverlay>();
                overlay.Show(settingsManager, OnOptionsBack);
                return;
            }

            optionsOverlayInstance = Instantiate(overlayRegistry.OptionsOverlayPrefab);
            var overlayComp = optionsOverlayInstance.GetComponent<R8EOX.UI.Internal.OptionsOverlay>();
            overlayComp.Show(settingsManager, OnOptionsBack);
        }

        private void OnOptionsBack()
        {
            if (optionsOverlayInstance != null)
                Destroy(optionsOverlayInstance);
            optionsOverlayInstance = null;
        }

        private void OnModeSelected(SessionMode mode)
        {
            selectedMode = mode;
            trackSelectScreen.Initialize(
                appManager.SessionChannel.TrackRegistry,
                mode,
                OnTrackConfirmed,
                OnTrackBackPressed);
            navigator.PushScreen(trackSelectScreen);
        }

        private void OnModeBackPressed()
        {
            navigator.PopScreen();
        }

        private void OnTrackConfirmed(TrackDefinition track)
        {
            navigator.ReplaceScreen(loadingScreen);
            loadingScreen.ResetProgress();
            appManager.LoadTrack(track, selectedMode);
        }

        private void OnTrackBackPressed()
        {
            navigator.PopScreen();
        }

        // ------------------------------------------------------------------ //
        // AppManager event handlers
        // ------------------------------------------------------------------ //

        private void HandleLoadProgress(float progress)
        {
            loadingScreen.UpdateProgress(progress);
        }

        private void HandleLoadComplete()
        {
            // The loading screen (and this scene) will be destroyed by AppManager
            // as part of the track scene transition — nothing to do here.
        }

        private void HandleLoadError(string msg)
        {
            Debug.LogError($"[MenuManager] Load failed: {msg}");
            navigator.PopScreen();
        }
    }
}
