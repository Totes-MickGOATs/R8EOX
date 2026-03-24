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

        private Coroutine pulseCoroutine;
        private System.Action onAnyKeyPressed;
        private bool inputEnabled;

        internal void Initialize(System.Action onKeyPressed)
        {
            onAnyKeyPressed = onKeyPressed;
        }

        internal override void OnEnter()
        {
            base.OnEnter();
            inputEnabled = false;

            if (titleTransform != null)
            {
                StartCoroutine(DelayedEnableInput(MenuAnimator.ScaleIn(titleTransform, 0.5f, 0.85f)));
            }
            else
            {
                inputEnabled = true;
            }

            if (promptGroup != null)
            {
                pulseCoroutine = StartCoroutine(MenuAnimator.PulseAlpha(promptGroup, 0.3f, 1.0f));
            }
        }

        internal override void OnExit()
        {
            base.OnExit();
            inputEnabled = false;

            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
            }
        }

        private void Update()
        {
            if (!inputEnabled)
            {
                return;
            }

            bool keyboardPressed = Keyboard.current != null &&
                                   Keyboard.current.anyKey.wasPressedThisFrame;

            bool gamepadPressed = Gamepad.current != null &&
                                  Gamepad.current.buttonSouth.wasPressedThisFrame;

            if (keyboardPressed || gamepadPressed)
            {
                inputEnabled = false;
                onAnyKeyPressed?.Invoke();
            }
        }

        private IEnumerator DelayedEnableInput(IEnumerator scaleAnimation)
        {
            yield return StartCoroutine(scaleAnimation);
            inputEnabled = true;
        }
    }
}
