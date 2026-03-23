using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace R8EOX.Tests.PlayMode
{
    public static class E2ETestUtils
    {
        // Load a scene by name and wait until it's fully active
        public static IEnumerator LoadSceneAndWait(string sceneName, float timeout = 10f)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            Assert.IsNotNull(op, $"Failed to start loading scene '{sceneName}'");

            float elapsed = 0f;
            while (!op.isDone)
            {
                elapsed += Time.unscaledDeltaTime;
                if (elapsed > timeout)
                    Assert.Fail($"Scene '{sceneName}' did not load within {timeout}s");
                yield return null;
            }

            // Wait one extra frame for Awake/Start to run
            yield return null;
        }

        // Wait until a condition is true, with timeout
        public static IEnumerator WaitUntil(Func<bool> condition, float timeout = 5f, string message = null)
        {
            float elapsed = 0f;
            while (!condition())
            {
                elapsed += Time.unscaledDeltaTime;
                if (elapsed > timeout)
                {
                    string msg = message ?? "Condition not met";
                    Assert.Fail($"{msg} within {timeout}s");
                }
                yield return null;
            }
        }

        // Wait until a GameObject with the given name exists
        public static IEnumerator WaitForGameObject(string name, float timeout = 5f)
        {
            GameObject found = null;
            float elapsed = 0f;
            while (found == null)
            {
                found = GameObject.Find(name);
                if (found != null) break;

                elapsed += Time.unscaledDeltaTime;
                if (elapsed > timeout)
                    Assert.Fail($"GameObject '{name}' not found within {timeout}s");
                yield return null;
            }
        }

        // Wait until a component of type T exists in the scene
        public static IEnumerator WaitForComponent<T>(float timeout = 5f) where T : UnityEngine.Object
        {
            T found = null;
            float elapsed = 0f;
            while (found == null)
            {
                found = UnityEngine.Object.FindAnyObjectByType<T>();
                if (found != null) break;

                elapsed += Time.unscaledDeltaTime;
                if (elapsed > timeout)
                    Assert.Fail($"Component '{typeof(T).Name}' not found within {timeout}s");
                yield return null;
            }
        }

        // Wait for a number of frames with no errors logged
        public static IEnumerator WaitForNoErrors(int settleFrames = 5)
        {
            bool errorLogged = false;
            string errorMessage = null;

            void OnLog(string condition, string stacktrace, LogType type)
            {
                if (type == LogType.Error || type == LogType.Exception)
                {
                    errorLogged = true;
                    errorMessage = condition;
                }
            }

            Application.logMessageReceived += OnLog;

            for (int i = 0; i < settleFrames; i++)
            {
                yield return null;
                if (errorLogged)
                {
                    Application.logMessageReceived -= OnLog;
                    Assert.Fail($"Error logged during settle: {errorMessage}");
                }
            }

            Application.logMessageReceived -= OnLog;
        }

        // Wait for a specific number of frames
        public static IEnumerator WaitForFrames(int count)
        {
            for (int i = 0; i < count; i++)
                yield return null;
        }
    }
}
