using UnityEngine;

namespace R8EOX.Camera.Internal
{
    internal class FollowCamera
    {
        private Vector3 offset = new(0f, 3f, -8f);
        private float smoothSpeed = 10f;
        private float lookAheadFactor = 0.5f;

        private Vector3 currentVelocity;

        internal void Tick(Transform camera, Transform target, float deltaTime)
        {
            if (target == null)
            {
                return;
            }

            Vector3 desiredPosition = target.position + target.TransformDirection(offset);
            camera.position = Vector3.SmoothDamp(camera.position, desiredPosition, ref currentVelocity, 1f / smoothSpeed);

            Vector3 lookTarget = target.position + target.forward * lookAheadFactor;
            camera.LookAt(lookTarget);
        }

        internal void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }
    }
}
