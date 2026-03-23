using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace R8EOX.Tests.PlayMode
{
    [TestFixture]
    [Category("integrity")]
    public class SceneIntegrityTests : E2ETestBase
    {
        [UnityTest]
        public IEnumerator OutpostTrack_HasSessionBootstrapper()
        {
            yield return E2ETestUtils.LoadSceneAndWait("OutpostTrack");
            yield return E2ETestUtils.WaitForComponent<R8EOX.Session.Internal.SessionBootstrapper>();
        }

        [UnityTest]
        public IEnumerator OutpostTrack_HasTrackManager()
        {
            yield return E2ETestUtils.LoadSceneAndWait("OutpostTrack");
            yield return E2ETestUtils.WaitForComponent<R8EOX.Track.TrackManager>();
        }

        [UnityTest]
        public IEnumerator OutpostTrack_HasCameraManager()
        {
            yield return E2ETestUtils.LoadSceneAndWait("OutpostTrack");
            yield return E2ETestUtils.WaitForComponent<R8EOX.Camera.CameraManager>();
        }

        [UnityTest]
        public IEnumerator PhysicsTestTrack_HasRequiredManagers()
        {
            yield return E2ETestUtils.LoadSceneAndWait("PhysicsTestTrack");
            yield return E2ETestUtils.WaitForComponent<R8EOX.Session.Internal.SessionBootstrapper>();
            yield return E2ETestUtils.WaitForComponent<R8EOX.Track.TrackManager>();
            yield return E2ETestUtils.WaitForComponent<R8EOX.Camera.CameraManager>();
        }

        [UnityTest]
        public IEnumerator AllTrackScenes_HaveSpawnPoints(
            [Values("OutpostTrack", "PhysicsTestTrack")] string sceneName)
        {
            yield return E2ETestUtils.LoadSceneAndWait(sceneName);
            var trackManager = Object.FindAnyObjectByType<R8EOX.Track.TrackManager>();
            Assert.IsNotNull(trackManager, $"TrackManager missing in {sceneName}");
            trackManager.Initialize();
            Assert.That(trackManager.GetSpawnPointCount(), Is.GreaterThan(0),
                $"No spawn points found in {sceneName}");
        }
    }
}
