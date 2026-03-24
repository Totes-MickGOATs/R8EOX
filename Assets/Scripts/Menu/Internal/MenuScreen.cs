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
            Debug.Log($"[MenuScreen] Show({duration}) on {gameObject.name}, alpha before={CanvasGroup.alpha}");
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(FadeIn(duration));
        }

        internal void Hide(float duration)
        {
            Debug.Log($"[MenuScreen] Hide({duration}) on {gameObject.name}");
            StopAllCoroutines();
            StartCoroutine(FadeOut(duration));
        }

        internal void ShowImmediate()
        {
            Debug.Log($"[MenuScreen] ShowImmediate on {gameObject.name}");
            gameObject.SetActive(true);
            CanvasGroup.alpha = 1f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }

        internal void HideImmediate()
        {
            Debug.Log($"[MenuScreen] HideImmediate on {gameObject.name}");
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
            Debug.Log($"[MenuScreen] FadeIn complete on {gameObject.name}, alpha={CanvasGroup.alpha}");
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
                CanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
                yield return null;
            }
            CanvasGroup.alpha = 0f;
            Debug.Log($"[MenuScreen] FadeOut complete on {gameObject.name}");
            OnExit();
            gameObject.SetActive(false);
        }
    }
}
