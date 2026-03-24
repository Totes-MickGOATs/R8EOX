#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Generic;
using UnityEngine;
using R8EOX.Diagnostics;

namespace R8EOX.Diagnostics.Internal
{
    internal class LifecycleVerifier
    {
        private readonly List<LifecycleRequest> pendingRequests = new List<LifecycleRequest>();
        private readonly List<VerifyResult> recentResults = new List<VerifyResult>();
        private readonly EventLog eventLog;

        private const int MaxResultHistory = 64;

        public LifecycleVerifier(EventLog log)
        {
            eventLog = log;
        }

        public IReadOnlyList<VerifyResult> RecentResults => recentResults;

        public void RequestVerifyDestroyed(Object target, string label)
        {
            if (target == null)
            {
                // Already null at request time — immediate pass
                RecordResult(label, true, Time.realtimeSinceStartup);
                return;
            }

            pendingRequests.Add(new LifecycleRequest(target, label, Time.realtimeSinceStartup));
        }

        /// <summary>
        /// Called each frame from LateUpdate. Checks pending requests
        /// that are at least 1 frame old.
        /// </summary>
        public void Tick()
        {
            int currentFrame = Time.frameCount;

            for (int i = pendingRequests.Count - 1; i >= 0; i--)
            {
                var request = pendingRequests[i];
                if (currentFrame <= request.FrameRequested) continue;

                bool destroyed = request.IsTargetDestroyed;
                RecordResult(request.Label, destroyed, Time.realtimeSinceStartup);

                if (!destroyed)
                {
                    Debug.LogWarning(
                        $"[Diag] VERIFY FAILED: '{request.Label}' still exists " +
                        $"({currentFrame - request.FrameRequested} frames after Destroy)");
                }

                pendingRequests.RemoveAt(i);
            }
        }

        public void Clear()
        {
            pendingRequests.Clear();
            recentResults.Clear();
        }

        private void RecordResult(string label, bool passed, float timestamp)
        {
            var result = new VerifyResult(label, passed, timestamp);
            recentResults.Add(result);

            if (recentResults.Count > MaxResultHistory)
                recentResults.RemoveAt(0);

            var severity = passed ? EventSeverity.Info : EventSeverity.Error;
            var status = passed ? "PASSED" : "FAILED";
            eventLog.Add(timestamp, DiagChannel.App,
                $"Verify '{label}': {status}", severity);
        }
    }
}
#endif
