#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace R8EOX.Diagnostics.Internal
{
    internal enum FlowStatus
    {
        Active,
        Completed,
        Failed,
        TimedOut
    }
}
#endif
