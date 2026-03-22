using UnityEngine;

namespace R8EOX.Track.Internal
{
    internal enum SurfaceType
    {
        Asphalt,
        Dirt,
        Grass,
        Gravel
    }

    internal class TrackSurface : MonoBehaviour
    {
        [SerializeField] private SurfaceType surfaceType = SurfaceType.Asphalt;

        internal float GetGripMultiplier()
        {
            return surfaceType switch
            {
                SurfaceType.Asphalt => 1.0f,
                SurfaceType.Dirt => 0.6f,
                SurfaceType.Grass => 0.4f,
                SurfaceType.Gravel => 0.5f,
                _ => 1.0f
            };
        }

        internal SurfaceType GetSurfaceType()
        {
            return surfaceType;
        }
    }
}
