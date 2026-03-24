#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace R8EOX.Diagnostics.Internal
{
    internal class FlowInstance
    {
        public readonly FlowDefinition Definition;
        public readonly float StartTime;

        private readonly bool[] completedSteps;
        private readonly float[] stepTimes;
        private int currentStepIndex;
        private FlowStatus status;
        private string failureReason;

        public FlowInstance(FlowDefinition definition, float startTime)
        {
            Definition = definition;
            StartTime = startTime;
            completedSteps = new bool[definition.StepCount];
            stepTimes = new float[definition.StepCount];
            status = FlowStatus.Active;
        }

        public FlowStatus Status => status;
        public int CurrentStepIndex => currentStepIndex;
        public string FailureReason => failureReason;
        public string Name => Definition.Name;

        public bool ReportStep(string stepName, float timestamp)
        {
            if (status != FlowStatus.Active) return false;

            int index = Definition.FindStepIndex(stepName);
            if (index < 0) return false;

            completedSteps[index] = true;
            stepTimes[index] = timestamp;

            // Advance current step past any completed or optional steps
            while (currentStepIndex < Definition.StepCount && completedSteps[currentStepIndex])
            {
                currentStepIndex++;
            }

            if (currentStepIndex >= Definition.StepCount)
            {
                status = FlowStatus.Completed;
            }

            return true;
        }

        public void Fail(string reason, float timestamp)
        {
            if (status != FlowStatus.Active) return;
            status = FlowStatus.Failed;
            failureReason = reason;
        }

        public bool IsStepCompleted(int index) => index >= 0 && index < completedSteps.Length && completedSteps[index];
        public float GetStepTime(int index) => index >= 0 && index < stepTimes.Length ? stepTimes[index] : 0f;
        public float GetElapsed(float currentTime) => currentTime - StartTime;

        public void CheckTimeouts(float currentTime)
        {
            if (status != FlowStatus.Active) return;
            for (int i = currentStepIndex; i < Definition.StepCount; i++)
            {
                var step = Definition.GetStep(i);
                if (step.TimeoutSeconds > 0f && !completedSteps[i])
                {
                    float elapsed = currentTime - StartTime;
                    if (elapsed > step.TimeoutSeconds)
                    {
                        status = FlowStatus.TimedOut;
                        failureReason = $"Step '{step.Name}' timed out after {step.TimeoutSeconds:F1}s";
                        return;
                    }
                }
            }
        }
    }
}
#endif
