using UnityEngine;

namespace R8EOX.Camera.Internal
{
    internal class CinematicCamera
    {
        private Vector3[] cameraPositions;
        private int currentPositionIndex;
        private float transitionSpeed = 2f;

        internal void Initialize(Vector3[] positions)
        {
            cameraPositions = positions;
            currentPositionIndex = 0;
        }

        internal void Tick(Transform camera, Transform target, float deltaTime)
        {
            if (cameraPositions == null || cameraPositions.Length == 0 || target == null)
            {
                return;
            }

            // TODO: Smoothly transition between cinematic positions
            camera.position = Vector3.Lerp(camera.position, cameraPositions[currentPositionIndex], deltaTime * transitionSpeed);
            camera.LookAt(target);
        }

        internal void NextPosition()
        {
            if (cameraPositions != null)
            {
                currentPositionIndex = (currentPositionIndex + 1) % cameraPositions.Length;
            }
        }
    }
}
