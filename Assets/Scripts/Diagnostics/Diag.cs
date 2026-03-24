using System.Diagnostics;
using UnityEngine;

namespace R8EOX.Diagnostics
{
    public static class Diag
    {
        private static DiagnosticsManager manager;

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void SetManager(DiagnosticsManager mgr)
        {
            manager = mgr;
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Log(DiagChannel channel, string message)
        {
            if (manager == null) return;
            manager.EventLog.Add(Time.realtimeSinceStartup, channel, message);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(DiagChannel channel, string message)
        {
            if (manager == null) return;
            manager.EventLog.Add(Time.realtimeSinceStartup, channel, message,
                Internal.EventSeverity.Warning);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(DiagChannel channel, string message)
        {
            if (manager == null) return;
            manager.EventLog.Add(Time.realtimeSinceStartup, channel, message,
                Internal.EventSeverity.Error);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void BeginFlow(string flowName)
        {
            if (manager == null) return;
            manager.FlowTracer.BeginFlow(flowName);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void FlowStep(string flowName, string stepName)
        {
            if (manager == null) return;
            manager.FlowTracer.ReportStep(flowName, stepName);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void FailFlow(string flowName, string reason)
        {
            if (manager == null) return;
            manager.FlowTracer.FailFlow(flowName, reason);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void VerifyDestroyed(Object target, string label)
        {
            if (manager == null) return;
            manager.LifecycleVerifier.RequestVerifyDestroyed(target, label);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void SetVehicleSnapshot(Internal.VehicleDiagSnapshot snapshot)
        {
            if (manager == null) return;
            manager.VehicleSnapshot = snapshot;
        }
    }
}
