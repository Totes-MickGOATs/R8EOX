#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace R8EOX.Diagnostics.Internal
{
    internal readonly struct EventEntry
    {
        public readonly float Timestamp;
        public readonly DiagChannel Channel;
        public readonly string Message;
        public readonly EventSeverity Severity;

        public EventEntry(float timestamp, DiagChannel channel, string message, EventSeverity severity)
        {
            Timestamp = timestamp;
            Channel = channel;
            Message = message;
            Severity = severity;
        }

        public override string ToString()
        {
            return $"[{Timestamp:F2}] [{Channel}] {Message}";
        }
    }
}
#endif
