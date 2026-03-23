using UnityEngine;

namespace R8EOX.AI.Internal
{
    /// <summary>
    /// Wraps TrackManager centerline queries for AI path following.
    /// Provides lookahead points and curvature queries with optional
    /// lateral offset from the centerline for line variation.
    /// </summary>
    internal class RacingLine
    {
        private Track.TrackManager trackManager;
        private float trackLength;
        private float lateralOffset;

        internal bool IsValid => trackManager != null && trackLength > 0f;

        internal void Initialize(Track.TrackManager track, float offset)
        {
            trackManager = track;
            trackLength = track.GetTrackLength();
            lateralOffset = offset;
        }

        /// <summary>
        /// Get a target point ahead of the vehicle on the centerline,
        /// offset perpendicular to the track direction for variety.
        /// </summary>
        internal Vector3 GetLookaheadPoint(float currentDistance, float lookaheadDistance)
        {
            if (!IsValid) return Vector3.zero;

            float targetDist = WrapDistance(currentDistance + lookaheadDistance);
            Vector3 centerPoint = trackManager.GetPointAtDistance(targetDist);

            if (Mathf.Approximately(lateralOffset, 0f))
                return centerPoint;

            Vector3 direction = trackManager.GetDirectionAtDistance(targetDist);
            Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
            return centerPoint + right * lateralOffset;
        }

        /// <summary>
        /// Query curvature at a point ahead of the current position.
        /// Used for braking decisions before corners.
        /// </summary>
        internal float GetCurvatureAhead(float currentDistance, float lookDistance)
        {
            if (!IsValid) return 0f;

            float targetDist = WrapDistance(currentDistance + lookDistance);
            return trackManager.GetCurvatureAtDistance(targetDist);
        }

        /// <summary>
        /// Project a world position onto the centerline and return distance along track.
        /// </summary>
        internal float GetCurrentDistance(Vector3 position)
        {
            if (!IsValid) return 0f;
            return trackManager.GetDistanceAlongTrack(position);
        }

        /// <summary>
        /// Get the direction of the track at a given distance.
        /// </summary>
        internal Vector3 GetDirectionAtDistance(float distance)
        {
            if (!IsValid) return Vector3.forward;
            return trackManager.GetDirectionAtDistance(WrapDistance(distance));
        }

        private float WrapDistance(float distance)
        {
            if (trackLength <= 0f) return 0f;
            return ((distance % trackLength) + trackLength) % trackLength;
        }
    }
}
