using UnityEngine;
using Debug = UnityEngine.Debug;

namespace R8EOX.Diagnostics
{
    public class DiagnosticsManager : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private Internal.EventLog eventLog;
        private Internal.FlowTracer flowTracer;
        private Internal.LifecycleVerifier lifecycleVerifier;
        private Internal.VehicleDiagSnapshot vehicleSnapshot;
        private DiagnosticsOverlay overlay;
        private bool overlayVisible;

        internal Internal.EventLog EventLog => eventLog;
        internal Internal.FlowTracer FlowTracer => flowTracer;
        internal Internal.LifecycleVerifier LifecycleVerifier => lifecycleVerifier;

        internal Internal.VehicleDiagSnapshot VehicleSnapshot
        {
            get => vehicleSnapshot;
            set => vehicleSnapshot = value;
        }

        private void Awake()
        {
            eventLog = new Internal.EventLog(512);
            flowTracer = new Internal.FlowTracer(eventLog);
            lifecycleVerifier = new Internal.LifecycleVerifier(eventLog);
            Internal.FlowTemplates.RegisterAll(flowTracer);
            Diag.SetManager(this);
            Debug.Log("[DiagnosticsManager] Initialized");
        }

        private void Update()
        {
            flowTracer.Tick(Time.realtimeSinceStartup);
            HandleOverlayToggle();
        }

        private void LateUpdate()
        {
            lifecycleVerifier.Tick();
        }

        private void OnDestroy()
        {
            Diag.SetManager(null);
        }

        private void HandleOverlayToggle()
        {
            if (UnityEngine.InputSystem.Keyboard.current == null) return;
            if (!UnityEngine.InputSystem.Keyboard.current.f12Key.wasPressedThisFrame) return;

            overlayVisible = !overlayVisible;
            if (overlayVisible && overlay == null)
            {
                overlay = gameObject.AddComponent<DiagnosticsOverlay>();
                overlay.Initialize(this);
            }
            else if (overlay != null)
            {
                overlay.SetVisible(overlayVisible);
            }
        }

#endif
    }
}
