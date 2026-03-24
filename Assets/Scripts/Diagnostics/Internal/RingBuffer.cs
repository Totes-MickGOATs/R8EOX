#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;

namespace R8EOX.Diagnostics.Internal
{
    internal class RingBuffer<T> : IEnumerable<T>
    {
        private readonly T[] buffer;
        private int head;
        private int count;

        public RingBuffer(int capacity)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            buffer = new T[capacity];
        }

        public int Count => count;
        public int Capacity => buffer.Length;

        public void Add(T item)
        {
            buffer[head] = item;
            head = (head + 1) % buffer.Length;
            if (count < buffer.Length) count++;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                int actualIndex = (head - count + index + buffer.Length) % buffer.Length;
                return buffer[actualIndex];
            }
        }

        public void Clear()
        {
            head = 0;
            count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
#endif
