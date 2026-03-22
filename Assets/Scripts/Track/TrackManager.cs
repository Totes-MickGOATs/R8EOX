using UnityEngine;

namespace R8EOX.Track
{
    public class TrackManager : MonoBehaviour
    {
        [SerializeField] private TrackConfig config;

        public void Initialize(TrackConfig trackConfig)
        {
            // TODO: Set up track from config, find checkpoints
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
    }
}
