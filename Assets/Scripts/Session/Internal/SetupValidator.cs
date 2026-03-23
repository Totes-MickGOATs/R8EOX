using UnityEngine;
using R8EOX.Track;

namespace R8EOX.Session.Internal
{
    internal static class SetupValidator
    {
        private const float k_SpawnWarnThreshold = 0.5f;
        private const float k_SpawnErrorThreshold = 2.0f;

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
            if (trackManager == null)
            {
                overlay.AddError("Track",
                    "TrackManager is null",
                    "Assign TrackManager in scene");
                return;
            }
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
            ValidateSpawnHeights(overlay, trackManager);
        }

        private static void ValidateSpawnHeights(
            SetupErrorOverlay overlay,
            TrackManager trackManager)
        {
            SpawnPointData[] spawns = trackManager.GetSpawnPoints();
            if (spawns.Length == 0) return;

            Terrain terrain = Terrain.activeTerrain;
            if (terrain == null)
            {
                overlay.AddWarning("Spawn",
                    "No active terrain — cannot validate spawn heights");
                return;
            }

            for (int i = 0; i < spawns.Length; i++)
            {
                Vector3 pos = spawns[i].Position;
                float terrainY = terrain.SampleHeight(pos)
                    + terrain.transform.position.y;
                float delta = terrainY - pos.y;

                if (delta > k_SpawnErrorThreshold)
                {
                    overlay.AddError("Spawn",
                        $"Spawn {spawns[i].Index} is {delta:F1}m below terrain",
                        "Raise the spawn point above terrain surface");
                }
                else if (delta > k_SpawnWarnThreshold)
                {
                    overlay.AddWarning("Spawn",
                        $"Spawn {spawns[i].Index} is {delta:F1}m below terrain");
                }
            }
        }
    }
}
