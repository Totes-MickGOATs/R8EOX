using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.Menu.Internal
{
    internal static class MenuAnimator
    {
        // ── Public coroutines ──────────────────────────────────────────────

        /// <summary>
        /// Fades active children with a CanvasGroup from alpha 0→1, staggered.
        /// Children without CanvasGroup are skipped.
        /// </summary>
        internal static IEnumerator StaggerFadeIn(
            Transform parent, float staggerDelay, float elementDuration)
        {
            List<CanvasGroup> groups = CollectCanvasGroups(parent);

            foreach (CanvasGroup cg in groups)
                cg.alpha = 0f;

            foreach (CanvasGroup cg in groups)
            {
                yield return FadeCanvasGroup(cg, 0f, 1f, elementDuration);

                if (staggerDelay > 0f)
                    yield return WaitUnscaled(staggerDelay);
            }
        }

        /// <summary>
        /// Fades active children with a CanvasGroup from alpha 1→0, staggered.
        /// Children without CanvasGroup are skipped.
        /// </summary>
        internal static IEnumerator StaggerFadeOut(
            Transform parent, float staggerDelay, float elementDuration)
        {
            List<CanvasGroup> groups = CollectCanvasGroups(parent);

            foreach (CanvasGroup cg in groups)
            {
                yield return FadeCanvasGroup(cg, 1f, 0f, elementDuration);

                if (staggerDelay > 0f)
                    yield return WaitUnscaled(staggerDelay);
            }
        }

        /// <summary>
        /// Scales target from startScale to 1.0 with a cubic ease-out curve.
        /// </summary>
        internal static IEnumerator ScaleIn(
            Transform target, float duration, float startScale = 0.85f)
        {
            target.localScale = Vector3.one * startScale;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                float scale = Mathf.LerpUnclamped(startScale, 1f, eased);
                target.localScale = Vector3.one * scale;
                yield return null;
            }

            target.localScale = Vector3.one;
        }

        /// <summary>
        /// Oscillates a CanvasGroup alpha between minAlpha and 1.0 indefinitely.
        /// Stop externally via StopCoroutine.
        /// </summary>
        internal static IEnumerator PulseAlpha(
            CanvasGroup group, float minAlpha, float duration)
        {
            float elapsed = 0f;
            float range = 1f - minAlpha;

            while (true)
            {
                elapsed += Time.unscaledDeltaTime;
                float ping = Mathf.PingPong(elapsed / duration, 1f);
                group.alpha = minAlpha + ping * range;
                yield return null;
            }
        }

        /// <summary>
        /// Smoothly lerps an Image fillAmount to targetFill over duration.
        /// </summary>
        internal static IEnumerator FillBar(
            Image image, float targetFill, float duration)
        {
            float startFill = image.fillAmount;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                image.fillAmount = Mathf.Lerp(startFill, targetFill, t);
                yield return null;
            }

            image.fillAmount = targetFill;
        }

        // ── Private helpers ────────────────────────────────────────────────

        private static List<CanvasGroup> CollectCanvasGroups(Transform parent)
        {
            var result = new List<CanvasGroup>(parent.childCount);
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (!child.gameObject.activeSelf)
                    continue;
                CanvasGroup cg = child.GetComponent<CanvasGroup>();
                if (cg != null)
                    result.Add(cg);
            }
            return result;
        }

        private static IEnumerator FadeCanvasGroup(
            CanvasGroup cg, float from, float to, float duration)
        {
            cg.alpha = from;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                cg.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }
            cg.alpha = to;
        }

        private static IEnumerator WaitUnscaled(float seconds)
        {
            float elapsed = 0f;
            while (elapsed < seconds)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }
}
