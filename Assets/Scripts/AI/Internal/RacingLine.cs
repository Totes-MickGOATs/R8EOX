using UnityEngine;

namespace R8EOX.AI.Internal
{
    internal class RacingLine
    {
        private Vector3[] linePoints;
        private int currentTargetIndex;

        internal void Initialize(Vector3[] points)
        {
            linePoints = points;
            currentTargetIndex = 0;
        }

        internal Vector3 GetCurrentTarget()
        {
            if (linePoints == null || linePoints.Length == 0)
            {
                return Vector3.zero;
            }

            return linePoints[currentTargetIndex];
        }

        internal void AdvanceTarget(Vector3 currentPosition, float arrivalDistance)
        {
            if (linePoints == null)
            {
                return;
            }

            float dist = Vector3.Distance(currentPosition, linePoints[currentTargetIndex]);
            if (dist < arrivalDistance)
            {
                currentTargetIndex = (currentTargetIndex + 1) % linePoints.Length;
            }
        }

        internal float GetDistanceToTarget(Vector3 currentPosition)
        {
            return Vector3.Distance(currentPosition, GetCurrentTarget());
        }
    }
}
