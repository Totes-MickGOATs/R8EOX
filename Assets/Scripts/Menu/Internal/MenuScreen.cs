using System.Collections;
using UnityEngine;

namespace R8EOX.Menu.Internal
{
    [RequireComponent(typeof(CanvasGroup))]
    internal abstract class MenuScreen : MonoBehaviour
    {
        private CanvasGroup canvasGroup;

        protected CanvasGroup CanvasGroup
        {
            get
            {
                if (canvasGroup == null)
                    canvasGroup = GetComponent<CanvasGroup>();
                return canvasGroup;
            }
        }

        // Called when this screen becomes the active screen
        internal virtual void OnEnter() { }

        // Called when this screen is being left
        internal virtual void OnExit() { }

        internal void Show(float duration)
        {
#if UNITY_EDITOR
            Debug.Log($"[MenuScreen] Show({duration}) on {gameObject.name}, alpha before={CanvasGroup.alpha}");
#endif
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(FadeIn(duration));
        }

        internal void Hide(float duration)
        {
#if UNITY_EDITOR
            Debug.Log($"[MenuScreen] Hide({duration}) on {gameObject.name}");
#endif
            StopAllCoroutines();
            StartCoroutine(FadeOut(duration));
        }

        internal void ShowImmediate()
        {
#if UNITY_EDITOR
            Debug.Log($"[MenuScreen] ShowImmediate on {gameObject.name}");
#endif
            gameObject.SetActive(true);
            CanvasGroup.alpha = 1f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }

        internal void HideImmediate()
        {
#if UNITY_EDITOR
            Debug.Log($"[MenuScreen] HideImmediate on {gameObject.name}");
#endif
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }

        private IEnumerator FadeIn(float duration)
        {
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                CanvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            CanvasGroup.alpha = 1f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
#if UNITY_EDITOR
            Debug.Log($"[MenuScreen] FadeIn complete on {gameObject.name}, alpha={CanvasGroup.alpha}");
#endif
            OnEnter();
        }

        private IEnumerator FadeOut(float duration)
        {
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            float elapsed = 0f;
            float startAlpha = CanvasGroup.alpha;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                CanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }
            CanvasGroup.alpha = 0f;
#if UNITY_EDITOR
            Debug.Log($"[MenuScreen] FadeOut complete on {gameObject.name}");
#endif
            OnExit();
            gameObject.SetActive(false);
        }
    }
}
