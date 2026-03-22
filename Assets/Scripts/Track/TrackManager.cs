using UnityEngine;
using R8EOX.Track.Internal;

namespace R8EOX.Track
{
    public class TrackManager : MonoBehaviour
    {
        [SerializeField] private TrackConfig config;

        private SpawnPoint[] spawnPoints;

        public void Initialize(TrackConfig trackConfig)
        {
            config = trackConfig;
            spawnPoints = GetComponentsInChildren<SpawnPoint>();
            System.Array.Sort(spawnPoints, (a, b) => a.Index.CompareTo(b.Index));
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
            return spawnPoints != null ? spawnPoints.Length : 0;
        }

        public SpawnPointData[] GetSpawnPoints()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
                return System.Array.Empty<SpawnPointData>();

            var data = new SpawnPointData[spawnPoints.Length];
            for (int i = 0; i < spawnPoints.Length; i++)
                data[i] = spawnPoints[i].ToData();
            return data;
        }

        public SpawnPointData GetPlayerSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
                return default;

            foreach (var sp in spawnPoints)
            {
                if (sp.IsPlayerSpawn)
                    return sp.ToData();
            }
            // Fallback: first spawn point
            return spawnPoints[0].ToData();
        }

        public bool HasCenterline()
        {
            // TODO: Check if centerline component exists and has points
            return false;
        }
    }
}
