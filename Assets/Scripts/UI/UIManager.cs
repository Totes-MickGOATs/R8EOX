using System;
using UnityEngine;
using UnityEngine.InputSystem;
using R8EOX.UI.Internal;
using R8EOX.Race;
using R8EOX.Vehicle;
using R8EOX.Diagnostics;

namespace R8EOX.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RaceHUD raceHUD;
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private Leaderboard leaderboard;

        private R8EOX.Session.SessionManager sessionManager;
        private RaceManager raceManager;
        private GameObject overlayInstance;
        private VehicleSelectOverlay activeOverlay;
        private GameObject optionsOverlayInstance;
        private bool overlayActive;

        private GameObject cachedPlayerVehicle;
        private VehicleManager cachedVehicleManager;
        private bool isPaused;
        private bool countdownVisible;
        private System.Action onQuitToMenu;

        /// <summary>Set by SessionManager to enable swap routing.</summary>
        public void SetSessionManager(R8EOX.Session.SessionManager manager)
        {
            sessionManager = manager;
        }

        /// <summary>Set by SessionManager to enable HUD data polling.</summary>
        public void SetRaceManager(RaceManager rm)
        {
            raceManager = rm;
            if (raceManager != null)
                raceManager.OnPhaseChanged += HandlePhaseChanged;
        }

        /// <summary>Set by AppManager to enable quit-to-menu routing.</summary>
        public void SetQuitToMenuCallback(System.Action callback)
        {
            onQuitToMenu = callback;
        }

        public void RequestQuitToMenu()
        {
            onQuitToMenu?.Invoke();
        }

        public void RequestVehicleSwap()
        {
            if (sessionManager != null)
                sessionManager.RequestVehicleSwap();
        }

        /// <summary>Instantiate and show the vehicle selection overlay.</summary>
        public void ShowVehicleSelectOverlay(
            GameObject overlayPrefab,
            VehicleRegistry registry,
            Action<VehicleDefinition> confirmCallback,
            Action cancelCallback = null)
        {
            CleanupVehicleSelectOverlay();
            overlayInstance = Instantiate(overlayPrefab);
            activeOverlay = overlayInstance.GetComponent<VehicleSelectOverlay>();
            if (activeOverlay == null)
            {
                Debug.LogError(
                    "[UIManager] Overlay prefab missing "
                    + "VehicleSelectOverlay component!");
                Destroy(overlayInstance);
                overlayInstance = null;
                return;
            }
            activeOverlay.Show(registry, confirmCallback, cancelCallback);
            Diag.Log(DiagChannel.UI, "Vehicle select overlay shown");
        }

        /// <summary>Destroy the vehicle selection overlay if active.</summary>
        public void CleanupVehicleSelectOverlay()
        {
            Diag.Log(DiagChannel.UI, $"CleanupVehicleSelectOverlay: overlay={activeOverlay != null} instance={overlayInstance != null}");
            try
            {
                if (activeOverlay != null) activeOverlay.Hide();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[UIManager] Overlay Hide() failed: {e.Message}");
            }
            if (overlayInstance != null)
            {
                Diag.Log(DiagChannel.UI, $"Destroying overlay: {overlayInstance.name}");
                Destroy(overlayInstance);
                Diag.VerifyDestroyed(overlayInstance, "VehicleSelectOverlay");
            }
            else
            {
                Debug.LogWarning("[UIManager] overlayInstance is null — nothing to destroy!");
            }
            activeOverlay = null;
            overlayInstance = null;
        }

        public void ShowOptionsOverlay(R8EOX.Settings.SettingsManager settingsManager)
        {
            if (optionsOverlayInstance != null) return;
            optionsOverlayInstance = new GameObject("[OptionsOverlay]");
            var overlay =
                optionsOverlayInstance.AddComponent<R8EOX.UI.Internal.OptionsOverlay>();
            overlay.Show(settingsManager, HideOptionsOverlay);
            overlayActive = true;
        }

        public void HideOptionsOverlay()
        {
            if (optionsOverlayInstance != null)
                Destroy(optionsOverlayInstance);
            optionsOverlayInstance = null;
            overlayActive = false;
        }

        public void UpdateHUD(
            float speed, int position, int lap, int totalLaps)
        {
            if (raceHUD == null) return;
            raceHUD.UpdateSpeed(speed);
            raceHUD.UpdatePosition(position);
            raceHUD.UpdateLap(lap, totalLaps);
        }

        public void UpdateLeaderboard(string[] standings)
        {
            if (leaderboard != null)
                leaderboard.UpdateStandings(standings);
        }

        public void ShowPauseMenu()
        {
            isPaused = true;
            if (pauseMenu != null) pauseMenu.Show();
        }

        public void HidePauseMenu()
        {
            isPaused = false;
            if (pauseMenu != null) pauseMenu.Hide();
        }

        public void ShowCountdown(int seconds)
        {
            if (raceHUD != null) raceHUD.ShowCountdown(seconds);
        }

        public void ShowRaceResults()
        {
            // TODO: Display final results screen
        }

        private void Update()
        {
            HandlePauseInput();
            if (!isPaused && raceManager != null)
                UpdateHUDFromRaceData();
        }

        private void HandlePauseInput()
        {
            if (overlayActive) return;
            if (Keyboard.current == null) return;
            if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;
            TogglePause();
        }

        private void TogglePause()
        {
            if (isPaused)
                HidePauseMenu();
            else
                ShowPauseMenu();
        }

        private void UpdateHUDFromRaceData()
        {
            if (raceHUD == null) return;

            RacePhase phase = raceManager.GetCurrentPhase();
            if (phase == RacePhase.PreRace) return;

            UpdateCountdownDisplay(phase);
            if (phase != RacePhase.Racing && phase != RacePhase.Finished)
                return;

            CachePlayerVehicle();
            if (cachedVehicleManager == null) return;

            GameObject player = cachedPlayerVehicle;
            raceHUD.UpdateSpeed(cachedVehicleManager.GetSpeedKmh());
            raceHUD.UpdatePosition(raceManager.GetVehiclePosition(player));
            raceHUD.UpdateLap(
                raceManager.GetLapCount(player),
                raceManager.GetTotalLaps());
            raceHUD.UpdateRaceTime(raceManager.GetRaceTime());
            raceHUD.UpdateLapTime(raceManager.GetCurrentLapTime(player));
            raceHUD.UpdateBestLapTime(raceManager.GetBestLapTime(player));
        }

        private void UpdateCountdownDisplay(RacePhase phase)
        {
            if (phase == RacePhase.Countdown)
            {
                float remaining = raceManager.GetCountdownRemaining();
                raceHUD.ShowCountdown(remaining);
                countdownVisible = true;
            }
            else if (countdownVisible)
            {
                raceHUD.HideCountdown();
                countdownVisible = false;
            }
        }

        private void CachePlayerVehicle()
        {
            if (sessionManager == null) return;
            GameObject player = sessionManager.PlayerVehicle;
            if (player == null) return;

            if (player != cachedPlayerVehicle)
            {
                cachedPlayerVehicle = player;
                cachedVehicleManager =
                    player.GetComponent<VehicleManager>();
            }
        }

        private void HandlePhaseChanged(RacePhase phase)
        {
            if (phase == RacePhase.Racing && raceHUD != null)
                raceHUD.Show();

            if (phase == RacePhase.Finished)
                ShowRaceResults();
        }

        private void OnDestroy()
        {
            if (raceManager != null)
                raceManager.OnPhaseChanged -= HandlePhaseChanged;
        }
    }
}
