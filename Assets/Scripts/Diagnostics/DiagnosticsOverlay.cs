#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

namespace R8EOX.Diagnostics
{
    public class DiagnosticsOverlay : MonoBehaviour
    {
        private DiagnosticsManager manager;
        private bool visible = true;
        private int activeTab;
        private Vector2 scrollPosition;
        private readonly string[] tabNames = { "Flows", "Events", "Vehicle", "Verify" };
        private readonly bool[] channelFilters = new bool[System.Enum.GetValues(typeof(DiagChannel)).Length];

        private readonly Color passColor = new Color(0.4f, 1f, 0.4f);
        private readonly Color failColor = new Color(1f, 0.4f, 0.4f);
        private readonly Color waitColor = new Color(1f, 1f, 0.4f);
        private readonly Color infoColor = Color.white;

        private GUIStyle headerStyle;
        private GUIStyle labelStyle;
        private bool stylesInitialized;

        public void Initialize(DiagnosticsManager mgr)
        {
            manager = mgr;
            for (int i = 0; i < channelFilters.Length; i++)
                channelFilters[i] = true;
        }

        public void SetVisible(bool show) => visible = show;

        private void InitStyles()
        {
            if (stylesInitialized) return;
            headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12 };
            stylesInitialized = true;
        }

        private void OnGUI()
        {
            if (!visible || manager == null) return;
            InitStyles();

            GUI.Box(new Rect(10, 10, 520, 420), "");
            GUILayout.BeginArea(new Rect(15, 15, 510, 410));

            GUILayout.Label("DIAGNOSTICS (F12)", headerStyle);
            activeTab = GUILayout.Toolbar(activeTab, tabNames);
            GUILayout.Space(5);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(360));

            switch (activeTab)
            {
                case 0: DrawFlowsTab(); break;
                case 1: DrawEventsTab(); break;
                case 2: DrawVehicleTab(); break;
                case 3: DrawVerifyTab(); break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawFlowsTab()
        {
            var tracer = manager.FlowTracer;
            float now = Time.realtimeSinceStartup;

            GUI.color = waitColor;
            GUILayout.Label("ACTIVE FLOWS:", headerStyle);
            GUI.color = infoColor;

            foreach (var kvp in tracer.ActiveFlows)
            {
                var flow = kvp.Value;
                GUILayout.Label($"  {flow.Name} ({flow.GetElapsed(now):F1}s)", labelStyle);
                DrawFlowSteps(flow, now);
            }

            if (tracer.ActiveFlows.Count == 0)
                GUILayout.Label("  (none)", labelStyle);

            GUILayout.Space(10);

            GUI.color = passColor;
            GUILayout.Label("RECENT COMPLETED:", headerStyle);
            GUI.color = infoColor;
            for (int i = tracer.CompletedFlows.Count - 1; i >= 0 && i >= tracer.CompletedFlows.Count - 5; i--)
            {
                var flow = tracer.CompletedFlows[i];
                float elapsed = flow.GetElapsed(flow.GetStepTime(flow.Definition.StepCount - 1));
                GUILayout.Label($"  {flow.Name} [COMPLETED] {elapsed:F1}s", labelStyle);
            }

            GUILayout.Space(10);

            GUI.color = failColor;
            GUILayout.Label("RECENT FAILED:", headerStyle);
            GUI.color = infoColor;
            for (int i = tracer.FailedFlows.Count - 1; i >= 0 && i >= tracer.FailedFlows.Count - 5; i--)
            {
                var flow = tracer.FailedFlows[i];
                GUILayout.Label($"  {flow.Name} [FAILED] {flow.FailureReason}", labelStyle);
            }
        }

        private void DrawFlowSteps(Internal.FlowInstance flow, float now)
        {
            for (int i = 0; i < flow.Definition.StepCount; i++)
            {
                var step = flow.Definition.GetStep(i);
                bool completed = flow.IsStepCompleted(i);
                bool isCurrent = i == flow.CurrentStepIndex && !completed;

                string icon = completed ? "ok" : (isCurrent ? ".." : "  ");
                GUI.color = completed ? passColor : (isCurrent ? waitColor : infoColor);

                string timing = "";
                if (completed)
                {
                    float stepTime = flow.GetStepTime(i) - flow.StartTime;
                    timing = $" {stepTime:F1}s";
                }
                else if (isCurrent)
                {
                    timing = $" (waiting... {flow.GetElapsed(now):F1}s)";
                }

                string opt = step.IsOptional ? "?" : "";
                GUILayout.Label($"    [{icon}] {step.Name}{opt}{timing}", labelStyle);
            }
            GUI.color = infoColor;
        }

        private void DrawEventsTab()
        {
            GUILayout.BeginHorizontal();
            var channels = System.Enum.GetValues(typeof(DiagChannel));
            for (int i = 0; i < channels.Length; i++)
            {
                channelFilters[i] = GUILayout.Toggle(
                    channelFilters[i],
                    ((DiagChannel)channels.GetValue(i)).ToString(),
                    GUILayout.Width(60));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            foreach (var entry in manager.EventLog.GetRecent(100))
            {
                if (!channelFilters[(int)entry.Channel]) continue;

                GUI.color = entry.Severity switch
                {
                    Internal.EventSeverity.Warning => waitColor,
                    Internal.EventSeverity.Error   => failColor,
                    _                              => infoColor
                };
                GUILayout.Label($"[{entry.Timestamp:F2}] [{entry.Channel}] {entry.Message}", labelStyle);
            }
            GUI.color = infoColor;
        }

        private void DrawVehicleTab()
        {
            var snap = manager.VehicleSnapshot;
            GUILayout.Label("VEHICLE DIAGNOSTICS:", headerStyle);
            GUILayout.Space(5);
            GUILayout.Label($"  Speed:        {snap.SpeedKmh:F1} km/h", labelStyle);
            GUILayout.Label($"  Steer Angle:  {snap.SteerAngle:F1} deg", labelStyle);
            GUILayout.Label($"  Throttle:     {snap.ThrottleInput:F2}", labelStyle);
            GUILayout.Label($"  Brake:        {snap.BrakeInput:F2}", labelStyle);
            GUILayout.Label($"  Velocity:     {snap.Velocity}", labelStyle);
            GUILayout.Label($"  Angular Vel:  {snap.AngularVelocity}", labelStyle);
            GUILayout.Label($"  Grounded:     {snap.IsGrounded} ({snap.GroundedWheelCount} wheels)", labelStyle);
            GUILayout.Label($"  Suspension:   {snap.SuspensionTravel:F3}", labelStyle);
            GUILayout.Label($"  Slip Angle:   {snap.SlipAngle:F2} deg", labelStyle);
            GUILayout.Label($"  Slip Ratio:   {snap.SlipRatio:F3}", labelStyle);
        }

        private void DrawVerifyTab()
        {
            var results = manager.LifecycleVerifier.RecentResults;
            GUILayout.Label("LIFECYCLE VERIFICATION:", headerStyle);
            GUILayout.Space(5);

            for (int i = results.Count - 1; i >= 0; i--)
            {
                var result = results[i];
                GUI.color = result.Passed ? passColor : failColor;
                string status = result.Passed ? "PASS" : "FAIL";
                GUILayout.Label($"  [{status}] {result.Label} @ {result.Timestamp:F2}s", labelStyle);
            }

            GUI.color = infoColor;
            if (results.Count == 0)
                GUILayout.Label("  (no verifications yet)", labelStyle);
        }
    }
}
#endif
