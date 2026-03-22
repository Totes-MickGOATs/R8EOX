using UnityEngine;

namespace R8EOX.Track.Internal
{
    internal class Centerline
    {
        private Vector3[] controlPoints;
        private float totalLength;

        internal void Initialize(Vector3[] points)
        {
            controlPoints = points;
            // TODO: Calculate total spline length
        }

        internal Vector3 GetPointAtDistance(float distance)
        {
            // TODO: Sample spline at given distance along track
            return Vector3.zero;
        }

        internal Vector3 GetNearestPoint(Vector3 worldPosition)
        {
            // TODO: Find nearest point on spline to world position
            return Vector3.zero;
        }

        internal float GetTotalLength()
        {
            return totalLength;
        }
    }
}
