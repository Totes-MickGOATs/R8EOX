using R8EOX.Track;

namespace R8EOX.Session.Internal
{
    internal static class SetupValidator
    {
        internal static void Validate(
            SetupErrorOverlay overlay,
            TrackManager trackManager,
            Camera.CameraManager cameraManager,
            Race.RaceManager raceManager,
            UI.UIManager uiManager,
            Audio.AudioManager audioManager,
            VFX.VFXManager vfxManager,
            AI.AIManager aiManager,
            SessionMode effectiveMode)
        {
            if (cameraManager == null)
                overlay.AddError("Camera",
                    "No CameraManager in scene",
                    "Add CameraManager component and assign in SessionBootstrapper");
            if (trackManager.GetSpawnPointCount() <= 0)
                overlay.AddError("Track",
                    "No spawn points found",
                    "Add SpawnGrid or SpawnPoint children to TrackManager");
            if (trackManager.GetTrackConfig() == null)
                overlay.AddError("Track",
                    "TrackManager has no TrackConfig assigned",
                    "Create a TrackConfig asset and assign it in the Inspector");
            if (raceManager == null)
                overlay.AddWarning("Race",
                    "No RaceManager — degrading to Practice mode");
            if (uiManager == null)
                overlay.AddWarning("UI",
                    "No UIManager — HUD disabled");
            if (audioManager == null)
                overlay.AddWarning("Audio",
                    "No AudioManager — audio disabled");
            if (vfxManager == null)
                overlay.AddWarning("VFX",
                    "No VFXManager — visual effects disabled");
            if (aiManager == null && effectiveMode == SessionMode.Race)
                overlay.AddWarning("AI",
                    "No AIManager — AI opponents disabled");
        }
    }
}
