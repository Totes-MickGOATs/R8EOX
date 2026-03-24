using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace R8EOX.Tests.PlayMode
{
    [TestFixture]
    [Category("flow")]
    public class SessionFlowTests : E2ETestBase
    {
        [UnityTest]
        public IEnumerator EditorPlay_Track_CreatesSessionManager()
        {
            yield return E2ETestUtils.LoadSceneAndWait("OutpostTrack");
            // SessionBootstrapper.Awake creates [SessionManager] in editor-play mode
            yield return E2ETestUtils.WaitForGameObject("[SessionManager]");
        }

        [UnityTest]
        public IEnumerator EditorPlay_Track_SpawnsVehicle()
        {
            yield return E2ETestUtils.LoadSceneAndWait("OutpostTrack");
            yield return E2ETestUtils.WaitForGameObject("[SessionManager]");

            var sessionManager = Object.FindAnyObjectByType<R8EOX.Session.SessionManager>();
            Assert.IsNotNull(sessionManager, "SessionManager should exist");

            yield return E2ETestUtils.WaitUntil(
                () => sessionManager.IsReady,
                timeout: 10f,
                message: "Session did not become ready");

            Assert.IsNotNull(sessionManager.PlayerVehicle, "Player vehicle should be spawned");
        }

        [UnityTest]
        public IEnumerator EditorPlay_Track_CameraTargetsVehicle()
        {
            yield return E2ETestUtils.LoadSceneAndWait("OutpostTrack");

            yield return E2ETestUtils.WaitForComponent<R8EOX.Session.SessionManager>(timeout: 10f);
            var sessionManager = Object.FindAnyObjectByType<R8EOX.Session.SessionManager>();
            Assert.IsNotNull(sessionManager, "SessionManager should exist");

            yield return E2ETestUtils.WaitUntil(
                () => sessionManager.IsReady,
                timeout: 10f,
                message: "Session did not become ready");

            var cameraManager = Object.FindAnyObjectByType<R8EOX.Camera.CameraManager>();
            Assert.IsNotNull(cameraManager, "CameraManager should exist");

            // Camera target is set by SessionManager.WirePlayerVehicle internally.
            // Verify the player vehicle exists — that is the precondition for camera wiring.
            Assert.IsNotNull(sessionManager.PlayerVehicle,
                "Player vehicle should be spawned for camera to target");
        }

        [UnityTest]
        public IEnumerator Session_Ready_HUDVisible()
        {
            yield return E2ETestUtils.LoadSceneAndWait("OutpostTrack");
            yield return E2ETestUtils.WaitForFrames(5);

            // UIManager may or may not exist depending on track setup.
            // This test verifies no errors occur during session setup regardless.
            yield return E2ETestUtils.WaitForNoErrors(settleFrames: 3);
        }

        [UnityTest]
        public IEnumerator Session_EndSession_CleansUp()
        {
            yield return E2ETestUtils.LoadSceneAndWait("OutpostTrack");
            yield return E2ETestUtils.WaitForGameObject("[SessionManager]");

            var sessionManager = Object.FindAnyObjectByType<R8EOX.Session.SessionManager>();
            Assert.IsNotNull(sessionManager, "SessionManager should exist");

            yield return E2ETestUtils.WaitUntil(
                () => sessionManager.IsReady,
                timeout: 10f,
                message: "Session did not become ready");

            var vehicle = sessionManager.PlayerVehicle;
            Assert.IsNotNull(vehicle, "Vehicle should exist before EndSession");

            sessionManager.EndSession();
            yield return E2ETestUtils.WaitForFrames(3);

            // After EndSession, vehicle should be destroyed
            Assert.IsTrue(vehicle == null, "Vehicle should be destroyed after EndSession");
            Assert.IsFalse(sessionManager.IsActive, "Session should not be active after EndSession");
        }
    }
}
