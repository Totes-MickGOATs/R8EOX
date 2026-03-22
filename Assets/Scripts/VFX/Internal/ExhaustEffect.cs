using UnityEngine;

namespace R8EOX.VFX.Internal
{
    internal class ExhaustEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem exhaustParticles;

        internal void UpdateExhaust(float throttle)
        {
            if (exhaustParticles == null)
            {
                return;
            }

            // TODO: Adjust particle emission rate based on throttle
            var emission = exhaustParticles.emission;
            emission.rateOverTime = throttle * 50f;
        }

        internal void TriggerBackfire()
        {
            // TODO: Burst of particles for gear shift backfire effect
            if (exhaustParticles != null)
            {
                exhaustParticles.Emit(20);
            }
        }
    }
}
