using UnityEngine;
using R8EOX.Track.Internal;

namespace R8EOX.Track
{
    public class TrackManager : MonoBehaviour
    {
        [SerializeField] private TrackConfig config;

        private SpawnPointData[] cachedSpawnData;
        private Centerline centerline;
        private Checkpoint[] checkpoints;

        public System.Action<int, GameObject> OnCheckpointPassed;

        public void Initialize(TrackConfig trackConfig)
        {
            config = trackConfig;

            DiscoverSpawnPoints();
            DiscoverCenterline();
            DiscoverCheckpoints();
        }

        private void DiscoverSpawnPoints()
        {
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

        private void DiscoverCenterline()
        {
            centerline = GetComponentInChildren<Centerline>();
            if (centerline != null)
                centerline.Initialize();
        }

        private void DiscoverCheckpoints()
        {
            checkpoints = GetComponentsInChildren<Checkpoint>();
            if (checkpoints == null || checkpoints.Length == 0)
            {
                checkpoints = System.Array.Empty<Checkpoint>();
                return;
            }

            System.Array.Sort(checkpoints, (a, b) => a.Index.CompareTo(b.Index));

            for (int i = 0; i < checkpoints.Length; i++)
                checkpoints[i].OnVehiclePassed += HandleCheckpointPassed;
        }

        private void HandleCheckpointPassed(int index, GameObject vehicle)
        {
            OnCheckpointPassed?.Invoke(index, vehicle);
        }

        private void OnDestroy()
        {
            if (checkpoints != null)
            {
                for (int i = 0; i < checkpoints.Length; i++)
                {
                    if (checkpoints[i] != null)
                        checkpoints[i].OnVehiclePassed -= HandleCheckpointPassed;
                }
            }
        }

        public int GetCheckpointCount()
        {
            return checkpoints?.Length ?? 0;
        }

        public Vector3 GetCheckpointPosition(int index)
        {
            if (checkpoints == null || index < 0 || index >= checkpoints.Length)
                return Vector3.zero;

            return checkpoints[index].transform.position;
        }

        public float GetTrackLength()
        {
            if (centerline == null || !centerline.IsValid)
                return 0f;

            return centerline.GetTotalLength();
        }

        public float GetSurfaceGripAt(Vector3 position)
        {
            // TODO: Query track surface type at position, return grip multiplier
            return 1f;
        }

        public Vector3 GetNearestCenterlinePoint(Vector3 position)
        {
            if (centerline == null || !centerline.IsValid)
                return position;

            return centerline.GetNearestPoint(position);
        }

        public float GetDistanceAlongTrack(Vector3 position)
        {
            if (centerline == null || !centerline.IsValid)
                return 0f;

            return centerline.GetDistanceAtPoint(position);
        }

        public Vector3 GetDirectionAtDistance(float distance)
        {
            if (centerline == null || !centerline.IsValid)
                return Vector3.forward;

            return centerline.GetDirectionAtDistance(distance);
        }

        public float GetCurvatureAtDistance(float distance)
        {
            if (centerline == null || !centerline.IsValid)
                return 0f;

            return centerline.GetCurvatureAtDistance(distance);
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
            return centerline != null && centerline.IsValid;
        }
    }
}
