#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace R8EOX.Diagnostics.Internal
{
    internal readonly struct FlowStep
    {
        public readonly string Name;
        public readonly bool IsOptional;
        public readonly float TimeoutSeconds;

        public FlowStep(string name, bool isOptional = false, float timeoutSeconds = 0f)
        {
            Name = name;
            IsOptional = isOptional;
            TimeoutSeconds = timeoutSeconds;
        }

        public override string ToString() => IsOptional ? $"[{Name}?]" : $"[{Name}]";
    }
}
#endif
