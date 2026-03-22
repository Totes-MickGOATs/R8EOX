using UnityEngine;

namespace R8EOX.VFX.Internal
{
    internal class SparkEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem sparkParticles;

        internal void SpawnSparks(Vector3 position, Vector3 normal, float intensity)
        {
            if (sparkParticles == null)
            {
                return;
            }

            sparkParticles.transform.position = position;
            sparkParticles.transform.forward = normal;
            sparkParticles.Emit(Mathf.CeilToInt(intensity * 10f));
        }
    }
}
