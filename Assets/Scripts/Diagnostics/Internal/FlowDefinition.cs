#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Generic;

namespace R8EOX.Diagnostics.Internal
{
    internal class FlowDefinition
    {
        public readonly string Name;
        private readonly List<FlowStep> steps;

        public FlowDefinition(string name)
        {
            Name = name;
            steps = new List<FlowStep>();
        }

        public int StepCount => steps.Count;

        public FlowDefinition AddStep(string stepName, bool optional = false, float timeout = 0f)
        {
            steps.Add(new FlowStep(stepName, optional, timeout));
            return this;
        }

        public FlowStep GetStep(int index) => steps[index];

        public int FindStepIndex(string stepName)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i].Name == stepName) return i;
            }
            return -1;
        }

        public IReadOnlyList<FlowStep> Steps => steps;
    }
}
#endif
