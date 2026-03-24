#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace R8EOX.Diagnostics.Internal
{
    internal static class FlowTemplates
    {
        public const string TrackLoad = "TrackLoad";
        public const string VehicleSelect = "VehicleSelect";
        public const string ReturnToMenu = "ReturnToMenu";
        public const string PauseUnpause = "PauseUnpause";
        public const string OverlayLifecycle = "OverlayLifecycle";

        public static void RegisterAll(FlowTracer tracer)
        {
            tracer.RegisterFlow(CreateTrackLoadFlow());
            tracer.RegisterFlow(CreateVehicleSelectFlow());
            tracer.RegisterFlow(CreateReturnToMenuFlow());
            tracer.RegisterFlow(CreatePauseUnpauseFlow());
            tracer.RegisterFlow(CreateOverlayLifecycleFlow());
        }

        private static FlowDefinition CreateTrackLoadFlow()
        {
            return new FlowDefinition(TrackLoad)
                .AddStep("MenuRequestedTrackLoad")
                .AddStep("LoadingScreenAppeared")
                .AddStep("SceneLoaded", timeout: 30f)
                .AddStep("TrackSystemsReady", timeout: 10f)
                .AddStep("VehicleSelectStarted", optional: true)
                .AddStep("VehicleSpawned", timeout: 15f)
                .AddStep("CameraTracking")
                .AddStep("SessionReady");
        }

        private static FlowDefinition CreateVehicleSelectFlow()
        {
            return new FlowDefinition(VehicleSelect)
                .AddStep("OverlayInstantiated")
                .AddStep("OverlayVisible")
                .AddStep("ConfirmPressed")
                .AddStep("OverlayDismissed")
                .AddStep("VerifyOverlayGone", timeout: 2f);
        }

        private static FlowDefinition CreateReturnToMenuFlow()
        {
            return new FlowDefinition(ReturnToMenu)
                .AddStep("ReturnRequested")
                .AddStep("SessionEnded")
                .AddStep("VehiclesDestroyed")
                .AddStep("MenuLoaded", timeout: 15f)
                .AddStep("MenuVisible");
        }

        private static FlowDefinition CreatePauseUnpauseFlow()
        {
            return new FlowDefinition(PauseUnpause)
                .AddStep("PauseRequested")
                .AddStep("TimeScaleZero")
                .AddStep("PauseMenuVisible")
                .AddStep("ResumeRequested")
                .AddStep("PauseMenuDismissed")
                .AddStep("TimeScaleRestored");
        }

        private static FlowDefinition CreateOverlayLifecycleFlow()
        {
            return new FlowDefinition(OverlayLifecycle)
                .AddStep("Instantiated")
                .AddStep("Visible")
                .AddStep("Interactive")
                .AddStep("Dismissed")
                .AddStep("Destroyed")
                .AddStep("VerifyGone", timeout: 2f);
        }
    }
}
#endif
