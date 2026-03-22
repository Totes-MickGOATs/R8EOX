using System.Collections;
using UnityEngine;

namespace R8EOX.PhysicsTest.Internal
{
    internal class PlaybackController : MonoBehaviour
    {
        [Header("Playback")]
        [Tooltip("Available speed multiplier options.")]
        [SerializeField] private float[] availableSpeeds = { 0.25f, 0.5f, 1f, 2f };

        private bool _isPaused;
        private float _speed = 1f;
        private Coroutine _stepCoroutine;

        internal bool IsPaused => _isPaused;
        internal float Speed => _speed;
        internal float[] AvailableSpeeds => availableSpeeds;

        private void Start()
        {
            Play();
        }

        internal void Play()
        {
            _isPaused = false;
            Time.timeScale = _speed;
        }

        internal void Pause()
        {
            _isPaused = true;
            Time.timeScale = 0f;
        }

        internal void TogglePause()
        {
            if (_isPaused)
                Play();
            else
                Pause();
        }

        internal void SetSpeed(float speed)
        {
            _speed = ClampToNearest(speed);
            if (!_isPaused)
                Time.timeScale = _speed;
        }

        internal void Step()
        {
            if (_stepCoroutine != null)
                StopCoroutine(_stepCoroutine);
            _stepCoroutine = StartCoroutine(StepRoutine());
        }

        private IEnumerator StepRoutine()
        {
            Time.timeScale = 1f;
            yield return new WaitForFixedUpdate();
            Pause();
            _stepCoroutine = null;
        }

        private float ClampToNearest(float speed)
        {
            float nearest = availableSpeeds[0];
            float minDist = Mathf.Abs(speed - nearest);
            foreach (float option in availableSpeeds)
            {
                float dist = Mathf.Abs(speed - option);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = option;
                }
            }
            return nearest;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}
