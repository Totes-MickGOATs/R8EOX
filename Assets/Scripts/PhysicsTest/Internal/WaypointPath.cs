using UnityEngine;

namespace R8EOX.PhysicsTest.Internal
{
    internal class WaypointPath : MonoBehaviour
    {
        [Header("Debug")]
        [Tooltip("Number of samples per segment for gizmo drawing.")]
        [SerializeField] private int gizmoSamplesPerSegment = 10;

        private Transform[] _waypoints;
        private float[] _segmentLengths;
        private float _totalLength;

        internal int WaypointCount => _waypoints?.Length ?? 0;
        internal float TotalLength => _totalLength;

        private void Awake()
        {
            DiscoverWaypoints();
            PrecomputeLengths();
        }

        private void DiscoverWaypoints()
        {
            int count = transform.childCount;
            _waypoints = new Transform[count];
            for (int i = 0; i < count; i++)
                _waypoints[i] = transform.GetChild(i);

            if (_waypoints.Length < 4)
                Debug.LogWarning($"[WaypointPath] {name} has {_waypoints.Length} waypoints — minimum 4 required for Catmull-Rom.", this);
        }

        private void PrecomputeLengths()
        {
            int count = _waypoints.Length;
            _segmentLengths = new float[count];
            _totalLength = 0f;

            const int stepsPerSegment = 20;
            for (int i = 0; i < count; i++)
            {
                float segLen = 0f;
                Vector3 prev = EvaluateSegment(i, 0f);
                for (int s = 1; s <= stepsPerSegment; s++)
                {
                    Vector3 curr = EvaluateSegment(i, s / (float)stepsPerSegment);
                    segLen += Vector3.Distance(prev, curr);
                    prev = curr;
                }
                _segmentLengths[i] = segLen;
                _totalLength += segLen;
            }
        }

        internal Vector3 GetPosition(float t)
        {
            DecomposePath(t, out int seg, out float localT);
            return EvaluateSegment(seg, localT);
        }

        internal Vector3 GetDirection(float t)
        {
            DecomposePath(t, out int seg, out float localT);
            float delta = 0.01f;
            Vector3 a = EvaluateSegment(seg, Mathf.Max(0f, localT - delta));
            Vector3 b = EvaluateSegment(seg, Mathf.Min(1f, localT + delta));
            return (b - a).normalized;
        }

        internal float GetNearestT(Vector3 worldPos)
        {
            const int totalSamples = 100;
            float bestT = 0f;
            float bestDistSq = float.MaxValue;

            for (int i = 0; i <= totalSamples; i++)
            {
                float t = i / (float)totalSamples;
                float distSq = (GetPosition(t) - worldPos).sqrMagnitude;
                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    bestT = t;
                }
            }
            return bestT;
        }

        internal Vector3 GetWaypointPosition(int index)
        {
            if (_waypoints == null || index < 0 || index >= _waypoints.Length)
                return Vector3.zero;
            return _waypoints[index].position;
        }

        private void DecomposePath(float t, out int segment, out float localT)
        {
            t = Mathf.Repeat(t, 1f);
            float target = t * _totalLength;
            float accumulated = 0f;
            int count = _segmentLengths.Length;

            segment = 0;
            localT = 0f;

            for (int i = 0; i < count; i++)
            {
                float segLen = _segmentLengths[i];
                if (accumulated + segLen >= target || i == count - 1)
                {
                    segment = i;
                    localT = segLen > 0f ? Mathf.Clamp01((target - accumulated) / segLen) : 0f;
                    return;
                }
                accumulated += segLen;
            }
        }

        private Vector3 EvaluateSegment(int segmentIndex, float localT)
        {
            int count = _waypoints.Length;
            Vector3 p0 = _waypoints[((segmentIndex - 1) % count + count) % count].position;
            Vector3 p1 = _waypoints[segmentIndex % count].position;
            Vector3 p2 = _waypoints[(segmentIndex + 1) % count].position;
            Vector3 p3 = _waypoints[(segmentIndex + 2) % count].position;
            return CatmullRom(p0, p1, p2, p3, localT);
        }

        private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            return 0.5f * (
                2f * p1 +
                (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                (-p0 + 3f * p1 - 3f * p2 + p3) * t3
            );
        }

        private void OnDrawGizmos()
        {
            int count = transform.childCount;
            if (count < 2) return;

            Transform[] pts = new Transform[count];
            for (int i = 0; i < count; i++)
                pts[i] = transform.GetChild(i);

            Gizmos.color = Color.yellow;
            int samples = Mathf.Max(2, gizmoSamplesPerSegment);

            for (int i = 0; i < count; i++)
            {
                Gizmos.DrawWireSphere(pts[i].position, 0.3f);

                Vector3 p0 = pts[((i - 1) % count + count) % count].position;
                Vector3 p1 = pts[i % count].position;
                Vector3 p2 = pts[(i + 1) % count].position;
                Vector3 p3 = pts[(i + 2) % count].position;

                Vector3 prev = CatmullRom(p0, p1, p2, p3, 0f);
                for (int s = 1; s <= samples; s++)
                {
                    Vector3 curr = CatmullRom(p0, p1, p2, p3, s / (float)samples);
                    Gizmos.DrawLine(prev, curr);
                    prev = curr;
                }
            }
        }
    }
}
