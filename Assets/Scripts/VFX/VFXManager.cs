using UnityEngine;

namespace R8EOX.VFX
{
    public class VFXManager : MonoBehaviour
    {
        public void SpawnTireMarks(Vector3 position, Vector3 direction, float intensity)
        {
            // TODO: Route to TireMarks component
        }

        public void SpawnExhaust(Vector3 position, float throttle)
        {
            // TODO: Route to ExhaustEffect component
        }

        public void SpawnSparks(Vector3 position, Vector3 normal)
        {
            // TODO: Route to SparkEffect component
        }

        public void SetSpeedBlur(float speed, float maxSpeed)
        {
            // TODO: Route to ScreenEffects component
        }

        public void SetDamageVignette(float damagePercent)
        {
            // TODO: Route to ScreenEffects component
        }
    }
}
