using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace R8EOX.Tests.PlayMode
{
    [TestFixture]
    [Category("smoke")]
    public class SmokeTests : E2ETestBase
    {
        [UnityTest]
        public IEnumerator BootScene_Loads_WithoutErrors()
        {
            yield return E2ETestUtils.LoadSceneAndWait("Boot");
            yield return E2ETestUtils.WaitForFrames(3);
            yield return E2ETestUtils.WaitForNoErrors();
        }

        [UnityTest]
        public IEnumerator BootScene_HasAppRoot_WithAppManager()
        {
            yield return E2ETestUtils.LoadSceneAndWait("Boot");
            yield return E2ETestUtils.WaitForGameObject("[AppRoot]");
            yield return E2ETestUtils.WaitForComponent<R8EOX.App.AppManager>();
        }

        [UnityTest]
        public IEnumerator MainMenu_LoadsFromBoot()
        {
            yield return E2ETestUtils.LoadSceneAndWait("Boot");
            yield return E2ETestUtils.WaitForComponent<R8EOX.Menu.MenuManager>(timeout: 10f);
        }

        [UnityTest]
        public IEnumerator MainMenu_HasMenuManager()
        {
            yield return E2ETestUtils.LoadSceneAndWait("Boot");
            yield return E2ETestUtils.WaitForComponent<R8EOX.Menu.MenuManager>(timeout: 10f);
            var manager = Object.FindAnyObjectByType<R8EOX.Menu.MenuManager>();
            Assert.IsNotNull(manager, "MenuManager should exist after Boot loads MainMenu");
        }

        [UnityTest]
        public IEnumerator TrackScene_Loads_WithoutErrors()
        {
            yield return E2ETestUtils.LoadSceneAndWait("OutpostTrack");
            yield return E2ETestUtils.WaitForNoErrors();
        }
    }
}
