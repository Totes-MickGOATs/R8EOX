#if UNITY_EDITOR || DEVELOPMENT_BUILD
using NUnit.Framework;
using UnityEngine;
using R8EOX.Diagnostics;
using R8EOX.Diagnostics.Internal;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    public class LifecycleVerifierTests
    {
        private EventLog _eventLog;
        private LifecycleVerifier _verifier;
        private GameObject _go;

        [SetUp]
        public void SetUp()
        {
            _eventLog = new EventLog();
            _verifier = new LifecycleVerifier(_eventLog);
            _go = null;
        }

        [TearDown]
        public void TearDown()
        {
            if (_go != null)
                Object.DestroyImmediate(_go);
        }

        // -------------------------------------------------------------------------
        // RequestVerifyDestroyed — immediate null path
        // -------------------------------------------------------------------------

        [Test]
        public void RequestVerifyDestroyed_NullTarget_ImmediatePass()
        {
            // Arrange — pass null directly (already-null reference)
            _verifier.RequestVerifyDestroyed(null, "NullTarget");

            // Assert — result recorded immediately, not deferred
            Assert.That(_verifier.RecentResults.Count, Is.EqualTo(1));
        }

        [Test]
        public void RequestVerifyDestroyed_NullTarget_RecordsPassedTrue()
        {
            _verifier.RequestVerifyDestroyed(null, "NullTarget");

            var result = _verifier.RecentResults[0];
            Assert.That(result.Passed, Is.True);
        }

        [Test]
        public void RequestVerifyDestroyed_NullTarget_RecordsCorrectLabel()
        {
            const string label = "MyLabel";
            _verifier.RequestVerifyDestroyed(null, label);

            var result = _verifier.RecentResults[0];
            Assert.That(result.Label, Is.EqualTo(label));
        }

        // -------------------------------------------------------------------------
        // RequestVerifyDestroyed — already-null UnityEngine.Object path
        // When a UnityObject has been destroyed, == null returns true even though
        // the C# reference is not null. We simulate this by destroying and then
        // re-requesting (object is already gone from Unity's side).
        // -------------------------------------------------------------------------

        [Test]
        public void RequestVerifyDestroyed_AlreadyDestroyedObject_RecordsImmediatePass()
        {
            // Arrange — create, then destroy the object immediately
            _go = new GameObject("TestGO");
            Object destroyedRef = _go;
            Object.DestroyImmediate(_go);
            _go = null; // TearDown guard

            // Act — request verify on an already-destroyed reference
            _verifier.RequestVerifyDestroyed(destroyedRef, "AlreadyGone");

            // Assert — Unity == null is true for destroyed objects; immediate pass
            Assert.That(_verifier.RecentResults.Count, Is.EqualTo(1));
            Assert.That(_verifier.RecentResults[0].Passed, Is.True);
        }

        // -------------------------------------------------------------------------
        // RequestVerifyDestroyed — live object path (deferred)
        // In EditMode Time.frameCount does NOT advance between calls, so Tick()
        // skips any requests where currentFrame <= request.FrameRequested.
        // The deferred verify-pass path requires PlayMode — see:
        //   Assets/Tests/PlayMode/LifecycleVerifierPlayTests.cs (TODO)
        // -------------------------------------------------------------------------

        [Test]
        public void RequestVerifyDestroyed_LiveObject_AddsNoPendingResultImmediately()
        {
            // Arrange
            _go = new GameObject("LiveGO");

            // Act
            _verifier.RequestVerifyDestroyed(_go, "LiveObject");

            // Assert — no result yet; it is pending deferred check
            Assert.That(_verifier.RecentResults.Count, Is.EqualTo(0));
        }

        [Test]
        public void Tick_WithLiveObjectSameFrame_DoesNotResolveRequest()
        {
            // Arrange — live object added in the same frame (frameCount won't advance)
            _go = new GameObject("LiveGO");
            _verifier.RequestVerifyDestroyed(_go, "LiveObject");

            // Act — Tick without frame advance; request.FrameRequested == currentFrame
            _verifier.Tick();

            // Assert — still no result; deferred check skipped because same frame
            Assert.That(_verifier.RecentResults.Count, Is.EqualTo(0));
        }

        // -------------------------------------------------------------------------
        // EventLog integration — immediate-null path writes to event log
        // -------------------------------------------------------------------------

        [Test]
        public void RequestVerifyDestroyed_NullTarget_WritesToEventLog()
        {
            _verifier.RequestVerifyDestroyed(null, "LoggedLabel");

            Assert.That(_eventLog.Count, Is.GreaterThan(0));
        }

        // -------------------------------------------------------------------------
        // Clear
        // -------------------------------------------------------------------------

        [Test]
        public void Clear_AfterResults_ResetsResultsToEmpty()
        {
            _verifier.RequestVerifyDestroyed(null, "A");
            _verifier.RequestVerifyDestroyed(null, "B");

            _verifier.Clear();

            Assert.That(_verifier.RecentResults.Count, Is.EqualTo(0));
        }

        [Test]
        public void Clear_NoPendingRequests_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _verifier.Clear());
        }

        [Test]
        public void Clear_WithPendingRequest_ClearsPending()
        {
            // Arrange — add a live object so there is a pending (deferred) request
            _go = new GameObject("PendingGO");
            _verifier.RequestVerifyDestroyed(_go, "Pending");

            // Act
            _verifier.Clear();

            // Assert — no results, and a subsequent Tick should produce nothing
            _verifier.Tick();
            Assert.That(_verifier.RecentResults.Count, Is.EqualTo(0));
        }

        // -------------------------------------------------------------------------
        // RecentResults — access and ordering
        // -------------------------------------------------------------------------

        [Test]
        public void RecentResults_MultipleNullRequests_ReturnsAllResults()
        {
            _verifier.RequestVerifyDestroyed(null, "First");
            _verifier.RequestVerifyDestroyed(null, "Second");
            _verifier.RequestVerifyDestroyed(null, "Third");

            Assert.That(_verifier.RecentResults.Count, Is.EqualTo(3));
        }

        [Test]
        public void RecentResults_PreservesInsertionOrder()
        {
            _verifier.RequestVerifyDestroyed(null, "Alpha");
            _verifier.RequestVerifyDestroyed(null, "Beta");

            Assert.That(_verifier.RecentResults[0].Label, Is.EqualTo("Alpha"));
            Assert.That(_verifier.RecentResults[1].Label, Is.EqualTo("Beta"));
        }

        [Test]
        public void RecentResults_IsReadOnlyList()
        {
            // Verify the property returns a read-only view
            var results = _verifier.RecentResults;
            Assert.That(
                results,
                Is.InstanceOf<System.Collections.Generic.IReadOnlyList<VerifyResult>>());
        }
    }
}
#endif
