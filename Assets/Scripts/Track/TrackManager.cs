using UnityEngine;
using R8EOX.Track.Internal;

namespace R8EOX.Track
{
    public class TrackManager : MonoBehaviour
    {
        [SerializeField] private TrackConfig config;

        private SpawnPointData[] cachedSpawnData;

        public void Initialize(TrackConfig trackConfig)
        {
            config = trackConfig;

            var grid = GetComponentInChildren<SpawnGrid>();
            if (grid != null)
            {
                cachedSpawnData = grid.ComputeSpawnPoints();
            }
            else
            {
                var points = GetComponentsInChildren<SpawnPoint>();
                System.Array.Sort(points, (a, b) => a.Index.CompareTo(b.Index));
                cachedSpawnData = new SpawnPointData[points.Length];
                for (int i = 0; i < points.Length; i++)
                    cachedSpawnData[i] = points[i].ToData();
            }
        }

        public int GetCheckpointCount()
        {
            // TODO: Return total checkpoints on track
            return 0;
        }

        public Vector3 GetCheckpointPosition(int index)
        {
            // TODO: Return world position of checkpoint by index
            return Vector3.zero;
        }

        public float GetTrackLength()
        {
            // TODO: Return total track length from centerline
            return 0f;
        }

        public float GetSurfaceGripAt(Vector3 position)
        {
            // TODO: Query track surface type at position, return grip multiplier
            return 1f;
        }

        public Vector3 GetNearestCenterlinePoint(Vector3 position)
        {
            // TODO: Project position onto centerline spline
            return Vector3.zero;
        }

        public int GetSpawnPointCount()
        {
            return cachedSpawnData != null ? cachedSpawnData.Length : 0;
        }

        public SpawnPointData[] GetSpawnPoints()
        {
            if (cachedSpawnData == null || cachedSpawnData.Length == 0)
                return System.Array.Empty<SpawnPointData>();

            return cachedSpawnData;
        }

        public SpawnPointData GetPlayerSpawnPoint()
        {
            if (cachedSpawnData == null || cachedSpawnData.Length == 0)
                return default;

            foreach (var sp in cachedSpawnData)
            {
                if (sp.IsPlayerSpawn)
                    return sp;
            }
            // Fallback: first spawn point
            return cachedSpawnData[0];
        }

        public bool HasCenterline()
        {
            // TODO: Check if centerline component exists and has points
            return false;
        }
    }
}
