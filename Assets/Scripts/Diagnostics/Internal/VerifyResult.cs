#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace R8EOX.Diagnostics.Internal
{
    internal readonly struct VerifyResult
    {
        public readonly string Label;
        public readonly bool Passed;
        public readonly float Timestamp;

        public VerifyResult(string label, bool passed, float timestamp)
        {
            Label = label;
            Passed = passed;
            Timestamp = timestamp;
        }
    }
}
#endif
