#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Linq;
using NUnit.Framework;
using R8EOX.Diagnostics;
using R8EOX.Diagnostics.Internal;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    internal sealed class EventLogTests
    {
        private EventLog _log;

        [SetUp]
        public void SetUp()
        {
            _log = new EventLog(capacity: 16);
        }

        [TearDown]
        public void TearDown()
        {
            _log = null;
        }

        // ------------------------------------------------------------------ //
        // Add
        // ------------------------------------------------------------------ //

        [Test]
        [Category("value")]
        public void Add_SingleEvent_CountIsOne()
        {
            _log.Add(0f, DiagChannel.App, "hello");

            Assert.That(_log.Count, Is.EqualTo(1));
        }

        [Test]
        [Category("value")]
        public void Add_MultipleEvents_CountMatches()
        {
            _log.Add(0f, DiagChannel.App, "a");
            _log.Add(1f, DiagChannel.Session, "b");
            _log.Add(2f, DiagChannel.UI, "c");

            Assert.That(_log.Count, Is.EqualTo(3));
        }

        // ------------------------------------------------------------------ //
        // GetEntry
        // ------------------------------------------------------------------ //

        [Test]
        [Category("value")]
        public void GetEntry_ValidIndex_ReturnsCorrectEntry()
        {
            _log.Add(1.5f, DiagChannel.Vehicle, "moved", EventSeverity.Warning);

            var entry = _log.GetEntry(0);

            Assert.That(entry.Timestamp, Is.EqualTo(1.5f).Within(0.001f));
            Assert.That(entry.Channel, Is.EqualTo(DiagChannel.Vehicle));
            Assert.That(entry.Message, Is.EqualTo("moved"));
            Assert.That(entry.Severity, Is.EqualTo(EventSeverity.Warning));
        }

        // ------------------------------------------------------------------ //
        // GetByChannel
        // ------------------------------------------------------------------ //

        [Test]
        [Category("value")]
        public void GetByChannel_FilteredChannel_OnlyReturnsMatching()
        {
            _log.Add(0f, DiagChannel.App,     "app-event");
            _log.Add(1f, DiagChannel.Session, "session-event-1");
            _log.Add(2f, DiagChannel.UI,      "ui-event");
            _log.Add(3f, DiagChannel.Session, "session-event-2");

            var results = _log.GetByChannel(DiagChannel.Session).ToList();

            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results.All(e => e.Channel == DiagChannel.Session), Is.True);
            Assert.That(results[0].Message, Is.EqualTo("session-event-1"));
            Assert.That(results[1].Message, Is.EqualTo("session-event-2"));
        }

        [Test]
        [Category("invariant")]
        public void GetByChannel_NoMatchingEvents_ReturnsEmpty()
        {
            _log.Add(0f, DiagChannel.App, "app-event");
            _log.Add(1f, DiagChannel.UI,  "ui-event");

            var results = _log.GetByChannel(DiagChannel.Race).ToList();

            Assert.That(results, Is.Empty);
        }

        // ------------------------------------------------------------------ //
        // GetRecent
        // ------------------------------------------------------------------ //

        [Test]
        [Category("value")]
        public void GetRecent_LessThanTotal_ReturnsLastN()
        {
            for (int i = 0; i < 5; i++)
                _log.Add(i, DiagChannel.App, $"msg{i}");

            var results = _log.GetRecent(3).ToList();

            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results[0].Message, Is.EqualTo("msg2"));
            Assert.That(results[1].Message, Is.EqualTo("msg3"));
            Assert.That(results[2].Message, Is.EqualTo("msg4"));
        }

        [Test]
        [Category("invariant")]
        public void GetRecent_MoreThanTotal_ReturnsAll()
        {
            _log.Add(0f, DiagChannel.App, "a");
            _log.Add(1f, DiagChannel.App, "b");

            var results = _log.GetRecent(10).ToList();

            Assert.That(results.Count, Is.EqualTo(2));
        }

        // ------------------------------------------------------------------ //
        // Clear
        // ------------------------------------------------------------------ //

        [Test]
        [Category("invariant")]
        public void Clear_AfterAdding_CountIsZero()
        {
            _log.Add(0f, DiagChannel.App, "a");
            _log.Add(1f, DiagChannel.App, "b");

            _log.Clear();

            Assert.That(_log.Count, Is.EqualTo(0));
        }

        // ------------------------------------------------------------------ //
        // GetAll
        // ------------------------------------------------------------------ //

        [Test]
        [Category("value")]
        public void GetAll_ReturnsAllInOrder()
        {
            _log.Add(0f, DiagChannel.App,     "first");
            _log.Add(1f, DiagChannel.Session, "second");
            _log.Add(2f, DiagChannel.UI,      "third");

            var results = _log.GetAll().ToList();

            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results[0].Message, Is.EqualTo("first"));
            Assert.That(results[1].Message, Is.EqualTo("second"));
            Assert.That(results[2].Message, Is.EqualTo("third"));
        }

        [Test]
        [Category("invariant")]
        public void GetAll_EmptyLog_ReturnsEmpty()
        {
            var results = _log.GetAll().ToList();

            Assert.That(results, Is.Empty);
        }
    }
}
#endif
