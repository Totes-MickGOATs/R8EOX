using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace R8EOX.Tests.PlayMode
{
    /// <summary>
    /// Abstract base used by PlayMode tests that load scenes.
    /// Handles DontDestroyOnLoad cleanup between tests and resets Time.timeScale.
    /// </summary>
    public abstract class E2ETestBase
    {
        [UnitySetUp]
        public IEnumerator BaseSetUp()
        {
            CleanupDontDestroyOnLoad();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator BaseTearDown()
        {
            Time.timeScale = 1f;
            LogAssert.ignoreFailingMessages = true;
            CleanupDontDestroyOnLoad();

            // Use a unique name to avoid collision if a prior teardown failed
            string sceneName = $"TestCleanup_{System.Guid.NewGuid():N}";
            var newScene = SceneManager.CreateScene(sceneName);
            SceneManager.SetActiveScene(newScene);

            // Unload all other scenes
            for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene != newScene && scene.isLoaded)
                    SceneManager.UnloadSceneAsync(scene);
            }

            yield return null;
            LogAssert.ignoreFailingMessages = false;
        }

        /// <summary>
        /// Destroys all GameObjects in the DontDestroyOnLoad scene.
        /// Uses the temp-GO trick: create a GO, move it to DDOL, enumerate
        /// root objects in that scene, destroy all, destroy the temp GO.
        /// </summary>
        private static void CleanupDontDestroyOnLoad()
        {
            var temp = new GameObject("__DDOLProbe__");
            Object.DontDestroyOnLoad(temp);

            var ddolScene = temp.scene;
            var rootObjects = ddolScene.GetRootGameObjects();

            foreach (var go in rootObjects)
            {
                if (go != temp)
                    Object.Destroy(go);
            }

            Object.Destroy(temp);
        }
    }
}
