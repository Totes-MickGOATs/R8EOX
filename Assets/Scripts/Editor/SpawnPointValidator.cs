#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using R8EOX.Track;

namespace R8EOX.Editor
{
    public static class SpawnPointValidator
    {
        private const float k_WarnThreshold = 0.5f;
        private const float k_ErrorThreshold = 2.0f;

        [MenuItem("R8EOX/Validate Spawn Points")]
        public static void Validate()
        {
            var trackManager = Object.FindAnyObjectByType<TrackManager>();
            if (trackManager == null)
            {
                Debug.LogWarning(
                    "[SpawnPointValidator] No TrackManager found in scene.");
                return;
            }

            trackManager.Initialize();
            SpawnPointData[] spawns = trackManager.GetSpawnPoints();

            if (spawns.Length == 0)
            {
                Debug.LogWarning(
                    "[SpawnPointValidator] No spawn points found.");
                return;
            }

            Terrain terrain = Terrain.activeTerrain;
            if (terrain == null)
            {
                Debug.LogWarning(
                    "[SpawnPointValidator] No active terrain — " +
                    "cannot validate spawn heights.");
                return;
            }

            int errors = 0;
            int warnings = 0;

            for (int i = 0; i < spawns.Length; i++)
            {
                Vector3 pos = spawns[i].Position;
                float terrainY = terrain.SampleHeight(pos)
                    + terrain.transform.position.y;
                float delta = terrainY - pos.y;

                if (delta > k_ErrorThreshold)
                {
                    Debug.LogError(
                        $"[SpawnPointValidator] Spawn {spawns[i].Index} " +
                        $"is {delta:F1}m below terrain at {pos}.");
                    errors++;
                }
                else if (delta > k_WarnThreshold)
                {
                    Debug.LogWarning(
                        $"[SpawnPointValidator] Spawn {spawns[i].Index} " +
                        $"is {delta:F1}m below terrain at {pos}.");
                    warnings++;
                }
            }

            if (errors == 0 && warnings == 0)
            {
                Debug.Log(
                    $"[SpawnPointValidator] All {spawns.Length} spawn " +
                    $"points are above terrain. No issues found.");
            }
            else
            {
                Debug.Log(
                    $"[SpawnPointValidator] Checked {spawns.Length} spawns " +
                    $"— {errors} error(s), {warnings} warning(s).");
            }
        }
    }
}
#endif
