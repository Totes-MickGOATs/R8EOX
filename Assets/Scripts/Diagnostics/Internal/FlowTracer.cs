#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Generic;
using UnityEngine;

namespace R8EOX.Diagnostics.Internal
{
    internal class FlowTracer
    {
        private readonly Dictionary<string, FlowDefinition> registeredFlows = new Dictionary<string, FlowDefinition>();
        private readonly Dictionary<string, FlowInstance> activeFlows = new Dictionary<string, FlowInstance>();
        private readonly List<FlowInstance> completedFlows = new List<FlowInstance>();
        private readonly List<FlowInstance> failedFlows = new List<FlowInstance>();
        private readonly EventLog eventLog;

        private const int MaxCompletedHistory = 32;
        private const int MaxFailedHistory = 32;

        public FlowTracer(EventLog log)
        {
            eventLog = log;
        }

        public IReadOnlyDictionary<string, FlowInstance> ActiveFlows => activeFlows;
        public IReadOnlyList<FlowInstance> CompletedFlows => completedFlows;
        public IReadOnlyList<FlowInstance> FailedFlows => failedFlows;

        public void RegisterFlow(FlowDefinition definition)
        {
            registeredFlows[definition.Name] = definition;
        }

        public void BeginFlow(string flowName)
        {
            if (!registeredFlows.TryGetValue(flowName, out var definition))
            {
                Debug.LogWarning($"[Diag] Unknown flow: '{flowName}'. Register it first.");
                return;
            }

            if (activeFlows.ContainsKey(flowName))
            {
                // End the existing flow as failed before starting a new one
                FailFlow(flowName, "Restarted before completion");
            }

            var instance = new FlowInstance(definition, Time.realtimeSinceStartup);
            activeFlows[flowName] = instance;
            eventLog.Add(Time.realtimeSinceStartup, DiagChannel.App, $"Flow '{flowName}' started");
        }

        public void ReportStep(string flowName, string stepName)
        {
            if (!activeFlows.TryGetValue(flowName, out var instance)) return;

            float now = Time.realtimeSinceStartup;
            bool accepted = instance.ReportStep(stepName, now);

            if (accepted)
            {
                eventLog.Add(now, DiagChannel.App, $"Flow '{flowName}' step '{stepName}' completed");
            }
            else
            {
                eventLog.Add(now, DiagChannel.App, $"Flow '{flowName}' step '{stepName}' rejected", EventSeverity.Warning);
            }

            if (instance.Status == FlowStatus.Completed)
            {
                MoveToCompleted(flowName, instance);
            }
        }

        public void FailFlow(string flowName, string reason)
        {
            if (!activeFlows.TryGetValue(flowName, out var instance)) return;

            float now = Time.realtimeSinceStartup;
            instance.Fail(reason, now);
            eventLog.Add(now, DiagChannel.App, $"Flow '{flowName}' FAILED: {reason}", EventSeverity.Error);
            MoveToFailed(flowName, instance);
        }

        public void Tick(float currentTime)
        {
            // Check timeouts on active flows
            var toTimeout = new List<string>();
            foreach (var kvp in activeFlows)
            {
                kvp.Value.CheckTimeouts(currentTime);
                if (kvp.Value.Status == FlowStatus.TimedOut)
                    toTimeout.Add(kvp.Key);
            }

            foreach (var name in toTimeout)
            {
                var instance = activeFlows[name];
                eventLog.Add(currentTime, DiagChannel.App,
                    $"Flow '{name}' TIMED OUT: {instance.FailureReason}", EventSeverity.Error);
                MoveToFailed(name, instance);
            }
        }

        public void Clear()
        {
            activeFlows.Clear();
            completedFlows.Clear();
            failedFlows.Clear();
        }

        private void MoveToCompleted(string flowName, FlowInstance instance)
        {
            activeFlows.Remove(flowName);
            completedFlows.Add(instance);
            if (completedFlows.Count > MaxCompletedHistory)
                completedFlows.RemoveAt(0);
        }

        private void MoveToFailed(string flowName, FlowInstance instance)
        {
            activeFlows.Remove(flowName);
            failedFlows.Add(instance);
            if (failedFlows.Count > MaxFailedHistory)
                failedFlows.RemoveAt(0);
        }
    }
}
#endif
