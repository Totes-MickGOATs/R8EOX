using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace R8EOX.Track.Internal
{
    [RequireComponent(typeof(SplineContainer))]
    internal class Centerline : MonoBehaviour
    {
        private const int MinKnots = 2;

        private SplineContainer container;
        private float cachedLength;
        private bool initialized;

        internal bool IsValid => container != null
            && container.Splines.Count > 0
            && container[0].Count >= MinKnots;

        internal void Initialize()
        {
            container = GetComponent<SplineContainer>();

            if (!IsValid)
            {
                Debug.LogWarning(
                    "[Centerline] SplineContainer has no valid spline or too few knots.",
                    this);
                cachedLength = 0f;
                initialized = false;
                return;
            }

            cachedLength = container.CalculateLength();
            initialized = true;
        }

        internal float GetTotalLength()
        {
            return cachedLength;
        }

        internal Vector3 GetPointAtDistance(float distance)
        {
            if (!initialized) return Vector3.zero;

            float t = DistanceToNormalizedT(distance);
            SplineUtility.Evaluate(
                container[0], t, out float3 localPos, out float3 _, out float3 _);

            return container.transform.TransformPoint(localPos);
        }

        internal Vector3 GetDirectionAtDistance(float distance)
        {
            if (!initialized) return Vector3.forward;

            float t = DistanceToNormalizedT(distance);
            SplineUtility.Evaluate(
                container[0], t, out float3 _, out float3 localTangent, out float3 _);

            Vector3 worldTangent = container.transform.TransformDirection(localTangent);
            return worldTangent.sqrMagnitude > 0.0001f
                ? worldTangent.normalized
                : Vector3.forward;
        }

        internal Vector3 GetNearestPoint(Vector3 worldPosition)
        {
            if (!initialized) return worldPosition;

            float3 localInput = container.transform.InverseTransformPoint(worldPosition);
            SplineUtility.GetNearestPoint(
                container[0], localInput, out float3 nearestLocal, out float _);

            return container.transform.TransformPoint(nearestLocal);
        }

        internal float GetDistanceAtPoint(Vector3 worldPosition)
        {
            if (!initialized) return 0f;

            float3 localInput = container.transform.InverseTransformPoint(worldPosition);
            SplineUtility.GetNearestPoint(
                container[0], localInput, out float3 _, out float t);

            return NormalizedTToDistance(t);
        }

        internal float GetCurvatureAtDistance(float distance)
        {
            if (!initialized) return 0f;

            float t = DistanceToNormalizedT(distance);
            return SplineUtility.EvaluateCurvature(container[0], t);
        }

        private float DistanceToNormalizedT(float distance)
        {
            if (cachedLength <= 0f) return 0f;

            if (container[0].Closed)
            {
                distance = ((distance % cachedLength) + cachedLength) % cachedLength;
            }

            return Mathf.Clamp01(distance / cachedLength);
        }

        private float NormalizedTToDistance(float t)
        {
            return Mathf.Clamp01(t) * cachedLength;
        }
    }
}
