using R8EOX.Camera.Internal;
using UnityEngine;

namespace R8EOX.Camera
{
    public class CameraManager : MonoBehaviour
    {
        [Header("Camera Reference")]
        [SerializeField] private UnityEngine.Camera mainCamera;

        [Header("Follow Settings")]
        [SerializeField] private Vector3 followOffset = new(0f, 3f, -8f);
        [SerializeField] private float followSmoothSpeed = 10f;
        [SerializeField] private float followLookAhead = 0.5f;

        private Transform currentTarget;
        private CameraMode activeMode = CameraMode.Follow;

        private readonly FollowCamera followCamera = new();
        private readonly CinematicCamera cinematicCamera = new();
        private readonly CameraShake cameraShake = new();

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = GetComponent<UnityEngine.Camera>();
            }

            if (mainCamera == null)
            {
                mainCamera = UnityEngine.Camera.main;
            }
        }

        public void SetTarget(Transform target)
        {
            currentTarget = target;
            followCamera.Configure(followOffset, followSmoothSpeed, followLookAhead);
        }

        public void SwitchToFollowMode()
        {
            activeMode = CameraMode.Follow;
        }

        public void SwitchToCinematicMode()
        {
            activeMode = CameraMode.Cinematic;
        }

        public void TriggerShake(float intensity, float duration)
        {
            cameraShake.StartShake(intensity, duration);
        }

        public CameraMode GetActiveMode()
        {
            return activeMode;
        }

        private void LateUpdate()
        {
            if (mainCamera == null || currentTarget == null)
            {
                return;
            }

            Transform cameraTransform = mainCamera.transform;

            switch (activeMode)
            {
                case CameraMode.Follow:
                    followCamera.Tick(cameraTransform, currentTarget, Time.deltaTime);
                    break;
                case CameraMode.Cinematic:
                    cinematicCamera.Tick(cameraTransform, currentTarget, Time.deltaTime);
                    break;
            }

            cameraTransform.position += cameraShake.GetShakeOffset();
            cameraShake.Tick(Time.deltaTime);
        }
    }
}
