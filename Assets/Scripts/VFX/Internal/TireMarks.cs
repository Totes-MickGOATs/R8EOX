using UnityEngine;

namespace R8EOX.VFX.Internal
{
    internal class TireMarks : MonoBehaviour
    {
        [SerializeField] private Material tireMarkMaterial;
        [SerializeField] private float markWidth = 0.2f;

        internal void CreateMark(Vector3 position, Vector3 direction, float intensity)
        {
            // TODO: Draw tire mark decal on track surface
            // Use object pooling for marks to avoid allocation
        }

        internal void ClearAllMarks()
        {
            // TODO: Remove all active tire marks
        }
    }
}
