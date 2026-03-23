using UnityEngine;

namespace R8EOX.VFX.Internal
{
    internal class TireMarks : MonoBehaviour
    {
        [SerializeField] private Material tireMarkMaterial;
        [SerializeField] private float markWidth = 0.15f;
        [SerializeField] private int maxWheels = 4;
        [SerializeField] private float trailTime = 10f;
        [SerializeField] private float minVertexDistance = 0.1f;
        [SerializeField] private float groundOffset = 0.01f;

        private TrailRenderer[] trails;
        private Transform[] trailTransforms;
        private bool initialized;

        internal void Initialize(int wheelCount)
        {
            if (initialized) return;

            int count = Mathf.Min(wheelCount, maxWheels);
            trails = new TrailRenderer[count];
            trailTransforms = new Transform[count];

            for (int i = 0; i < count; i++)
            {
                CreateTrailForWheel(i);
            }

            initialized = true;
        }

        internal void UpdateWheel(
            int wheelIndex, Vector3 position, bool active, float intensity)
        {
            if (!initialized) return;
            if (wheelIndex < 0 || wheelIndex >= trails.Length) return;

            var trail = trails[wheelIndex];
            if (trail == null) return;

            if (active)
            {
                Vector3 offsetPosition = position
                    + Vector3.up * groundOffset;
                trailTransforms[wheelIndex].position = offsetPosition;
                trail.emitting = true;
                trail.startWidth = markWidth * Mathf.Clamp01(intensity);
                trail.endWidth = trail.startWidth;
            }
            else
            {
                trail.emitting = false;
                trailTransforms[wheelIndex].position = position
                    + Vector3.up * groundOffset;
            }
        }

        internal void ClearAllMarks()
        {
            if (trails == null) return;

            for (int i = 0; i < trails.Length; i++)
            {
                if (trails[i] != null) trails[i].Clear();
            }
        }

        private void CreateTrailForWheel(int index)
        {
            var trailObj = new GameObject($"TireMark_{index}");
            trailObj.transform.SetParent(transform);
            trailObj.transform.localPosition = Vector3.zero;

            var trail = trailObj.AddComponent<TrailRenderer>();
            trail.time = trailTime;
            trail.minVertexDistance = minVertexDistance;
            trail.startWidth = markWidth;
            trail.endWidth = markWidth;
            trail.numCapVertices = 2;
            trail.numCornerVertices = 2;
            trail.autodestruct = false;
            trail.emitting = false;
            trail.receiveShadows = false;
            trail.shadowCastingMode
                = UnityEngine.Rendering.ShadowCastingMode.Off;
            trail.textureMode = LineTextureMode.Tile;

            if (tireMarkMaterial != null)
            {
                trail.material = tireMarkMaterial;
            }

            var gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(Color.black, 0f) },
                new[]
                {
                    new GradientAlphaKey(0.8f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            trail.colorGradient = gradient;

            trails[index] = trail;
            trailTransforms[index] = trailObj.transform;
        }

        private void OnDestroy()
        {
            if (trails == null) return;

            for (int i = 0; i < trails.Length; i++)
            {
                if (trails[i] != null)
                {
                    Destroy(trails[i].gameObject);
                }
            }

            trails = null;
            trailTransforms = null;
        }
    }
}
