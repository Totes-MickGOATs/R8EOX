using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.UI
{
    /// <summary>
    /// Persistent service on [AppRoot] that displays ephemeral toast notifications.
    /// Owns its own Canvas (sortOrder 300) so toasts render above all other UI.
    /// </summary>
    public class ToastManager : MonoBehaviour
    {
        // ── Colors ────────────────────────────────────────────────────────────
        private static readonly Color ColorSuccess = new Color(0f, 0.784f, 1f);
        private static readonly Color ColorWarning  = new Color(0.91f, 0.722f, 0.286f);
        private static readonly Color ColorError    = new Color(1f, 0.318f, 0.329f);
        private static readonly Color ColorBg       = new Color(0.078f, 0.082f, 0.102f);

        // ── Timing ───────────────────────────────────────────────────────────
        private const float FadeInDuration  = 0.2f;
        private const float FadeOutDuration = 0.3f;
        private const int   MaxVisible      = 3;

        // ── Runtime state ────────────────────────────────────────────────────
        private Canvas _canvas;
        private RectTransform _container;
        private readonly List<GameObject> _active = new List<GameObject>();

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Shows a success (cyan) toast.</summary>
        public void ShowSuccess(string message, float duration = 2f)
            => Show(message, ColorSuccess, duration);

        /// <summary>Shows a warning (gold) toast.</summary>
        public void ShowWarning(string message, float duration = 2f)
            => Show(message, ColorWarning, duration);

        /// <summary>Shows an error (red) toast.</summary>
        public void ShowError(string message, float duration = 2f)
            => Show(message, ColorError, duration);

        /// <summary>Shows a toast with a custom border color.</summary>
        public void Show(string message, Color borderColor, float duration = 2f)
        {
            EnsureCanvas();
            EnforceCap();
            GameObject toast = BuildToast(message, borderColor);
            _active.Add(toast);
            StartCoroutine(AnimateToast(toast, duration));
        }

        // ── Canvas bootstrap ─────────────────────────────────────────────────

        private void EnsureCanvas()
        {
            if (_canvas != null) return;

            var canvasGo = new GameObject("[ToastCanvas]");
            canvasGo.transform.SetParent(transform, false);

            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 300;

            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            _container = BuildContainer(canvasGo);
        }

        private static RectTransform BuildContainer(GameObject canvasGo)
        {
            var go = new GameObject("ToastContainer");
            go.transform.SetParent(canvasGo.transform, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0.5f, 0f);
            rt.anchorMax        = new Vector2(0.5f, 0f);
            rt.pivot            = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, 24f);
            rt.sizeDelta        = Vector2.zero;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment         = TextAnchor.LowerCenter;
            layout.spacing                = 8f;
            layout.childForceExpandWidth  = false;
            layout.childForceExpandHeight = false;
            layout.childControlWidth      = false;
            layout.childControlHeight     = false;

            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            return rt;
        }

        // ── Toast construction ───────────────────────────────────────────────

        private GameObject BuildToast(string message, Color borderColor)
        {
            var toastGo = new GameObject("Toast");
            toastGo.transform.SetParent(_container, false);

            toastGo.AddComponent<RectTransform>();

            var border = toastGo.AddComponent<Image>();
            border.color = borderColor;

            var toastFitter = toastGo.AddComponent<ContentSizeFitter>();
            toastFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            toastFitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

            var cg = toastGo.AddComponent<CanvasGroup>();
            cg.alpha = 0f;

            BuildInnerPanel(toastGo, message);

            return toastGo;
        }

        private static void BuildInnerPanel(GameObject toastGo, string message)
        {
            var innerGo = new GameObject("BG");
            innerGo.transform.SetParent(toastGo.transform, false);

            var innerRt = innerGo.AddComponent<RectTransform>();
            innerRt.anchorMin = Vector2.zero;
            innerRt.anchorMax = Vector2.one;
            innerRt.offsetMin = new Vector2(1f, 1f);
            innerRt.offsetMax = new Vector2(-1f, -1f);

            var bg = innerGo.AddComponent<Image>();
            bg.color = ColorBg;

            var innerLayout = innerGo.AddComponent<HorizontalLayoutGroup>();
            innerLayout.padding               = new RectOffset(16, 16, 8, 8);
            innerLayout.childForceExpandWidth  = false;
            innerLayout.childForceExpandHeight = false;
            innerLayout.childControlWidth      = true;
            innerLayout.childControlHeight     = true;

            var innerFitter = innerGo.AddComponent<ContentSizeFitter>();
            innerFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            innerFitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

            var textGo = new GameObject("Label");
            textGo.transform.SetParent(innerGo.transform, false);

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text      = message;
            tmp.color     = Color.white;
            tmp.fontSize  = 16f;
            tmp.alignment = TextAlignmentOptions.Left;
        }

        // ── Cap enforcement ──────────────────────────────────────────────────

        private void EnforceCap()
        {
            while (_active.Count >= MaxVisible)
            {
                GameObject oldest = _active[0];
                _active.RemoveAt(0);
                if (oldest != null)
                    Destroy(oldest);
            }
        }

        // ── Animation ────────────────────────────────────────────────────────

        private IEnumerator AnimateToast(GameObject toastGo, float holdDuration)
        {
            if (toastGo == null) yield break;
            var cg = toastGo.GetComponent<CanvasGroup>();

            yield return FadeCanvasGroup(cg, 0f, 1f, FadeInDuration);

            float elapsed = 0f;
            while (elapsed < holdDuration)
            {
                if (toastGo == null) yield break;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (toastGo != null)
                yield return FadeCanvasGroup(cg, 1f, 0f, FadeOutDuration);

            if (toastGo != null)
            {
                _active.Remove(toastGo);
                Destroy(toastGo);
            }
        }

        private static IEnumerator FadeCanvasGroup(
            CanvasGroup cg, float from, float to, float fadeDuration)
        {
            if (cg == null) yield break;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                if (cg == null) yield break;
                elapsed  += Time.unscaledDeltaTime;
                cg.alpha  = Mathf.Lerp(from, to, elapsed / fadeDuration);
                yield return null;
            }
            if (cg != null) cg.alpha = to;
        }
    }
}
