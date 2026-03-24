#if UNITY_EDITOR || DEVELOPMENT_BUILD
using NUnit.Framework;
using R8EOX.Diagnostics.Internal;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    public class FlowTracerTests
    {
        private EventLog eventLog;
        private FlowTracer tracer;

        [SetUp]
        public void SetUp()
        {
            eventLog = new EventLog(64);
            tracer = new FlowTracer(eventLog);
        }

        [TearDown]
        public void TearDown()
        {
            eventLog = null;
            tracer = null;
        }

        // ---- Helpers -------------------------------------------------------

        private static FlowDefinition TwoStepFlow(string name)
        {
            return new FlowDefinition(name)
                .AddStep("StepA")
                .AddStep("StepB");
        }

        // ---- RegisterFlow --------------------------------------------------

        [Test]
        public void RegisterFlow_NewFlow_CanBeginLater()
        {
            var def = TwoStepFlow("Boot");
            tracer.RegisterFlow(def);

            tracer.BeginFlow("Boot");

            Assert.That(tracer.ActiveFlows.ContainsKey("Boot"), Is.True);
        }

        // ---- BeginFlow -----------------------------------------------------

        [Test]
        public void BeginFlow_UnknownFlow_DoesNotThrow()
        {
            // No registration — must not throw; Unity logs a warning internally
            Assert.DoesNotThrow(() => tracer.BeginFlow("NotRegistered"));
        }

        [Test]
        public void BeginFlow_UnknownFlow_DoesNotAddToActive()
        {
            tracer.BeginFlow("Ghost");

            Assert.That(tracer.ActiveFlows.ContainsKey("Ghost"), Is.False);
        }

        // ---- ReportStep ----------------------------------------------------

        [Test]
        public void ReportStep_ValidStep_MovesToNext()
        {
            var def = TwoStepFlow("Boot");
            tracer.RegisterFlow(def);
            tracer.BeginFlow("Boot");

            tracer.ReportStep("Boot", "StepA");

            var instance = tracer.ActiveFlows["Boot"];
            Assert.That(instance.CurrentStepIndex, Is.EqualTo(1));
        }

        [Test]
        public void ReportStep_AllSteps_FlowCompletes()
        {
            var def = TwoStepFlow("Boot");
            tracer.RegisterFlow(def);
            tracer.BeginFlow("Boot");

            tracer.ReportStep("Boot", "StepA");
            tracer.ReportStep("Boot", "StepB");

            // After completion the instance is removed from active
            Assert.That(tracer.ActiveFlows.ContainsKey("Boot"), Is.False);
            Assert.That(tracer.CompletedFlows.Count, Is.EqualTo(1));
            Assert.That(tracer.CompletedFlows[0].Status, Is.EqualTo(FlowStatus.Completed));
        }

        [Test]
        public void ReportStep_CompletedFlow_MovesToCompletedList()
        {
            var def = TwoStepFlow("Boot");
            tracer.RegisterFlow(def);
            tracer.BeginFlow("Boot");

            tracer.ReportStep("Boot", "StepA");
            tracer.ReportStep("Boot", "StepB");

            Assert.That(tracer.CompletedFlows.Count, Is.EqualTo(1));
            Assert.That(tracer.CompletedFlows[0].Name, Is.EqualTo("Boot"));
        }

        // ---- FailFlow ------------------------------------------------------

        [Test]
        public void FailFlow_ActiveFlow_MovesToFailedList()
        {
            var def = TwoStepFlow("Boot");
            tracer.RegisterFlow(def);
            tracer.BeginFlow("Boot");

            tracer.FailFlow("Boot", "Intentional failure");

            Assert.That(tracer.ActiveFlows.ContainsKey("Boot"), Is.False);
            Assert.That(tracer.FailedFlows.Count, Is.EqualTo(1));
        }

        [Test]
        public void FailFlow_StoresReason()
        {
            var def = TwoStepFlow("Boot");
            tracer.RegisterFlow(def);
            tracer.BeginFlow("Boot");

            tracer.FailFlow("Boot", "Something went wrong");

            Assert.That(tracer.FailedFlows[0].FailureReason, Is.EqualTo("Something went wrong"));
        }

        [Test]
        public void FailFlow_ActiveFlow_StatusIsFailed()
        {
            var def = TwoStepFlow("Boot");
            tracer.RegisterFlow(def);
            tracer.BeginFlow("Boot");

            tracer.FailFlow("Boot", "Crash");

            Assert.That(tracer.FailedFlows[0].Status, Is.EqualTo(FlowStatus.Failed));
        }

        // ---- BeginFlow restart ---------------------------------------------

        [Test]
        public void BeginFlow_AlreadyActive_FailsPreviousInstance()
        {
            var def = TwoStepFlow("Boot");
            tracer.RegisterFlow(def);
            tracer.BeginFlow("Boot");

            // Restart — the old instance should be failed and moved out
            tracer.BeginFlow("Boot");

            Assert.That(tracer.FailedFlows.Count, Is.EqualTo(1));
            Assert.That(tracer.FailedFlows[0].Status, Is.EqualTo(FlowStatus.Failed));
        }

        [Test]
        public void BeginFlow_AlreadyActive_NewInstanceIsActive()
        {
            var def = TwoStepFlow("Boot");
            tracer.RegisterFlow(def);
            tracer.BeginFlow("Boot");

            tracer.BeginFlow("Boot");

            Assert.That(tracer.ActiveFlows.ContainsKey("Boot"), Is.True);
        }

        // ---- Clear ---------------------------------------------------------

        [Test]
        public void Clear_RemovesAllFlows()
        {
            var def = TwoStepFlow("Boot");
            tracer.RegisterFlow(def);
            tracer.BeginFlow("Boot");
            tracer.FailFlow("Boot", "Pre-clear");

            // Seed a second completed flow
            var def2 = TwoStepFlow("Menu");
            tracer.RegisterFlow(def2);
            tracer.BeginFlow("Menu");
            tracer.ReportStep("Menu", "StepA");
            tracer.ReportStep("Menu", "StepB");

            tracer.Clear();

            Assert.That(tracer.ActiveFlows.Count, Is.EqualTo(0));
            Assert.That(tracer.CompletedFlows.Count, Is.EqualTo(0));
            Assert.That(tracer.FailedFlows.Count, Is.EqualTo(0));
        }

        // ---- Tick / timeout ------------------------------------------------

        [Test]
        public void Tick_FlowExceedsStepTimeout_MovesToFailed()
        {
            var def = new FlowDefinition("Timed")
                .AddStep("StepA", timeout: 5f);
            tracer.RegisterFlow(def);
            tracer.BeginFlow("Timed");

            // Simulate time well past the timeout (start=0, now=10)
            tracer.Tick(10f);

            Assert.That(tracer.ActiveFlows.ContainsKey("Timed"), Is.False);
            Assert.That(tracer.FailedFlows.Count, Is.EqualTo(1));
            Assert.That(tracer.FailedFlows[0].Status, Is.EqualTo(FlowStatus.TimedOut));
        }

        [Test]
        public void Tick_FlowWithinTimeout_RemainsActive()
        {
            var def = new FlowDefinition("Timed")
                .AddStep("StepA", timeout: 5f);
            tracer.RegisterFlow(def);
            tracer.BeginFlow("Timed");

            // Well within timeout
            tracer.Tick(2f);

            Assert.That(tracer.ActiveFlows.ContainsKey("Timed"), Is.True);
        }
    }
}
#endif
