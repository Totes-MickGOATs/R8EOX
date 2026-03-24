#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;
using NUnit.Framework;
using R8EOX.Diagnostics.Internal;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    public class RingBufferTests
    {
        [Test]
        public void Add_SingleItem_CountIsOne()
        {
            var buffer = new RingBuffer<int>(4);

            buffer.Add(42);

            Assert.That(buffer.Count, Is.EqualTo(1));
        }

        [Test]
        public void Add_FillToCapacity_CountEqualsCapacity()
        {
            var buffer = new RingBuffer<int>(4);

            buffer.Add(1);
            buffer.Add(2);
            buffer.Add(3);
            buffer.Add(4);

            Assert.That(buffer.Count, Is.EqualTo(4));
        }

        [Test]
        public void Add_OverflowCapacity_CountStaysAtCapacity()
        {
            var buffer = new RingBuffer<int>(3);

            buffer.Add(1);
            buffer.Add(2);
            buffer.Add(3);
            buffer.Add(4);
            buffer.Add(5);

            Assert.That(buffer.Count, Is.EqualTo(3));
        }

        [Test]
        public void Indexer_FirstItem_ReturnsOldestItem()
        {
            // Fill then overflow: items 1,2 are evicted; oldest surviving is 3
            var buffer = new RingBuffer<int>(3);
            buffer.Add(1);
            buffer.Add(2);
            buffer.Add(3);
            buffer.Add(4);
            buffer.Add(5);

            Assert.That(buffer[0], Is.EqualTo(3));
        }

        [Test]
        public void Indexer_LastItem_ReturnsNewestItem()
        {
            var buffer = new RingBuffer<int>(3);
            buffer.Add(1);
            buffer.Add(2);
            buffer.Add(3);
            buffer.Add(4);
            buffer.Add(5);

            Assert.That(buffer[buffer.Count - 1], Is.EqualTo(5));
        }

        [Test]
        public void Indexer_NegativeIndex_ThrowsArgumentOutOfRangeException()
        {
            var buffer = new RingBuffer<int>(4);
            buffer.Add(10);

            Assert.That(() => { _ = buffer[-1]; },
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Indexer_IndexEqualToCount_ThrowsArgumentOutOfRangeException()
        {
            var buffer = new RingBuffer<int>(4);
            buffer.Add(10);

            Assert.That(() => { _ = buffer[buffer.Count]; },
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Clear_AfterAdding_CountIsZero()
        {
            var buffer = new RingBuffer<string>(4);
            buffer.Add("a");
            buffer.Add("b");
            buffer.Add("c");

            buffer.Clear();

            Assert.That(buffer.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetEnumerator_AllItems_ReturnsInOldestToNewestOrder()
        {
            var buffer = new RingBuffer<int>(3);
            // Overflow by 2: oldest surviving items are 3, 4, 5
            buffer.Add(1);
            buffer.Add(2);
            buffer.Add(3);
            buffer.Add(4);
            buffer.Add(5);

            var result = new List<int>(buffer);

            Assert.That(result, Is.EqualTo(new[] { 3, 4, 5 }));
        }

        [Test]
        public void Constructor_ZeroCapacity_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => new RingBuffer<int>(0),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Constructor_NegativeCapacity_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => new RingBuffer<int>(-1),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Clear_ThenAddNewItems_CountAndValuesAreCorrect()
        {
            var buffer = new RingBuffer<int>(3);
            buffer.Add(1);
            buffer.Add(2);
            buffer.Add(3);
            buffer.Clear();

            buffer.Add(99);

            Assert.That(buffer.Count, Is.EqualTo(1));
            Assert.That(buffer[0], Is.EqualTo(99));
        }

        [Test]
        public void Capacity_ReflectsConstructorArgument()
        {
            var buffer = new RingBuffer<float>(7);

            Assert.That(buffer.Capacity, Is.EqualTo(7));
        }
    }
}
#endif
