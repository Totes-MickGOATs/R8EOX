using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace R8EOX.App.Internal
{
    internal class SceneLoader
    {
        private const float DefaultTimeout = 30f;

        // Unity's async loading reports 0–0.9 during load, then stalls at 0.9 waiting for activation.
        private const float LoadCompleteThreshold = 0.9f;

        private readonly float timeout;

        internal SceneLoader(float timeout = DefaultTimeout)
        {
            this.timeout = timeout;
        }

        /// <summary>
        /// Loads a scene asynchronously. Yields progress updates via callback.
        /// The scene activates automatically when loading completes.
        /// </summary>
        internal IEnumerator LoadSceneAsync(
            string sceneName,
            Action<float> onProgress = null,
            Action onComplete = null,
            Action<string> onError = null)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                onError?.Invoke("Scene name is null or empty.");
                yield break;
            }

            AsyncOperation operation;
            try
            {
                operation = SceneManager.LoadSceneAsync(sceneName);
            }
            catch (Exception e)
            {
                onError?.Invoke($"Failed to start loading '{sceneName}': {e.Message}");
                yield break;
            }

            if (operation == null)
            {
                onError?.Invoke($"Scene '{sceneName}' not found in Build Settings.");
                yield break;
            }

            operation.allowSceneActivation = false;

            float elapsed = 0f;
            while (!operation.isDone)
            {
                elapsed += Time.unscaledDeltaTime;

                if (elapsed >= timeout)
                {
                    onError?.Invoke($"Scene load timed out after {timeout}s for '{sceneName}'.");
                    yield break;
                }

                float progress = Mathf.Clamp01(operation.progress / LoadCompleteThreshold);
                onProgress?.Invoke(progress);

                if (operation.progress >= LoadCompleteThreshold)
                {
                    onProgress?.Invoke(1f);
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            onComplete?.Invoke();
        }

        /// <summary>
        /// Loads a scene synchronously (non-async). Use for small scenes like MainMenu.
        /// </summary>
        internal void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
