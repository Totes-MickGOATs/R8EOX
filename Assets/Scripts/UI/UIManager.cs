using System;
using UnityEngine;
using R8EOX.UI.Internal;

namespace R8EOX.UI
{
    public class UIManager : MonoBehaviour
    {
        private R8EOX.Session.SessionManager sessionManager;
        private GameObject overlayInstance;
        private VehicleSelectOverlay activeOverlay;

        /// <summary>Set by SessionManager to enable swap routing.</summary>
        public void SetSessionManager(R8EOX.Session.SessionManager manager)
        {
            sessionManager = manager;
        }

        public void RequestVehicleSwap()
        {
            if (sessionManager != null)
                sessionManager.RequestVehicleSwap();
        }

        /// <summary>Instantiate and show the vehicle selection overlay.</summary>
        public void ShowVehicleSelectOverlay(
            VehicleRegistry registry,
            Action<VehicleDefinition> confirmCallback)
        {
            CleanupVehicleSelectOverlay();
            overlayInstance = Instantiate(registry.OverlayPrefab);
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
            activeOverlay.Show(registry, confirmCallback);
        }

        /// <summary>Destroy the vehicle selection overlay if active.</summary>
        public void CleanupVehicleSelectOverlay()
        {
            if (activeOverlay != null) activeOverlay.Hide();
            if (overlayInstance != null) Destroy(overlayInstance);
            activeOverlay = null;
            overlayInstance = null;
        }

        public void UpdateHUD(float speed, int position, int lap, int totalLaps)
        {
            // TODO: Route data to RaceHUD
        }

        public void UpdateLeaderboard(string[] standings)
        {
            // TODO: Route data to Leaderboard
        }

        public void ShowPauseMenu()
        {
            // TODO: Activate pause menu
        }

        public void HidePauseMenu()
        {
            // TODO: Deactivate pause menu
        }

        public void ShowCountdown(int seconds)
        {
            // TODO: Display countdown overlay
        }

        public void ShowRaceResults()
        {
            // TODO: Display final results screen
        }
    }
}
