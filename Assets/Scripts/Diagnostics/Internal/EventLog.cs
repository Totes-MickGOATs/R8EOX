#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Generic;

namespace R8EOX.Diagnostics.Internal
{
    internal class EventLog
    {
        private readonly RingBuffer<EventEntry> entries;

        public EventLog(int capacity = 256)
        {
            entries = new RingBuffer<EventEntry>(capacity);
        }

        public int Count => entries.Count;

        public void Add(float timestamp, DiagChannel channel, string message, EventSeverity severity = EventSeverity.Info)
        {
            entries.Add(new EventEntry(timestamp, channel, message, severity));
        }

        public EventEntry GetEntry(int index) => entries[index];

        public IEnumerable<EventEntry> GetAll()
        {
            for (int i = 0; i < entries.Count; i++)
                yield return entries[i];
        }

        public IEnumerable<EventEntry> GetByChannel(DiagChannel channel)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry.Channel == channel)
                    yield return entry;
            }
        }

        public IEnumerable<EventEntry> GetRecent(int maxCount)
        {
            int start = entries.Count > maxCount ? entries.Count - maxCount : 0;
            for (int i = start; i < entries.Count; i++)
                yield return entries[i];
        }

        public void Clear() => entries.Clear();
    }
}
#endif
