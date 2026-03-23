using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace R8EOX.Menu.Internal
{
    [RequireComponent(typeof(CanvasGroup))]
    internal class SplashScreen : MenuScreen
    {
        [Header("References")]
        [SerializeField] private RectTransform titleTransform;
        [Tooltip("CanvasGroup on the 'PRESS ANY KEY' text object")]
        [SerializeField] private CanvasGroup promptGroup;

        private Coroutine _pulseCoroutine;
        private System.Action _onAnyKeyPressed;
        private bool _inputEnabled;

        // ------------------------------------------------------------------ //
        // Public API
        // ------------------------------------------------------------------ //

        internal void Initialize(System.Action onKeyPressed)
        {
            _onAnyKeyPressed = onKeyPressed;
        }

        // ------------------------------------------------------------------ //
        // MenuScreen overrides
        // ------------------------------------------------------------------ //

        internal override void OnEnter()
        {
            base.OnEnter();
            _inputEnabled = false;

            if (titleTransform != null)
            {
                StartCoroutine(DelayedEnableInput(MenuAnimator.ScaleIn(titleTransform, 0.5f, 0.85f)));
            }
            else
            {
                _inputEnabled = true;
            }

            if (promptGroup != null)
            {
                _pulseCoroutine = StartCoroutine(MenuAnimator.PulseAlpha(promptGroup, 0.3f, 1.0f));
            }
        }

        internal override void OnExit()
        {
            base.OnExit();
            _inputEnabled = false;

            if (_pulseCoroutine != null)
            {
                StopCoroutine(_pulseCoroutine);
                _pulseCoroutine = null;
            }
        }

        // ------------------------------------------------------------------ //
        // Unity messages
        // ------------------------------------------------------------------ //

        private void Update()
        {
            if (!_inputEnabled)
            {
                return;
            }

            bool keyboardPressed = Keyboard.current != null &&
                                   Keyboard.current.anyKey.wasPressedThisFrame;

            bool gamepadPressed = Gamepad.current != null &&
                                  Gamepad.current.buttonSouth.wasPressedThisFrame;

            if (keyboardPressed || gamepadPressed)
            {
                _inputEnabled = false;
                _onAnyKeyPressed?.Invoke();
            }
        }

        // ------------------------------------------------------------------ //
        // Helpers
        // ------------------------------------------------------------------ //

        private IEnumerator DelayedEnableInput(IEnumerator scaleAnimation)
        {
            yield return StartCoroutine(scaleAnimation);
            _inputEnabled = true;
        }
    }
}
