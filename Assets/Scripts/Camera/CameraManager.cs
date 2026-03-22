using UnityEngine;

namespace R8EOX.Camera
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera mainCamera;

        private Transform currentTarget;

        public void SetTarget(Transform target)
        {
            currentTarget = target;
        }

        public void SwitchToFollowMode()
        {
            // TODO: Activate follow camera, deactivate others
        }

        public void SwitchToCinematicMode()
        {
            // TODO: Activate cinematic camera, deactivate others
        }

        public void TriggerShake(float intensity, float duration)
        {
            // TODO: Route to CameraShake component
        }

        private void LateUpdate()
        {
            // TODO: Update active camera mode
        }
    }
}
