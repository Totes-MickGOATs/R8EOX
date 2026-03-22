#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    public class SessionChannelTests
    {
        private SessionChannel channel;

        [SetUp]
        public void SetUp()
        {
            channel = ScriptableObject.CreateInstance<SessionChannel>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(channel);
        }

        [Test]
        public void HasActiveSession_Initial_IsFalse()
        {
            Assert.That(channel.HasActiveSession, Is.False);
        }

        [Test]
        public void ActiveConfig_Initial_IsNull()
        {
            Assert.That(channel.ActiveConfig, Is.Null);
        }

        [Test]
        public void SetSession_WithConfig_HasActiveSessionIsTrue()
        {
            var config = ScriptableObject.CreateInstance<SessionConfig>();

            channel.SetSession(config);

            Assert.That(channel.HasActiveSession, Is.True);
            Object.DestroyImmediate(config);
        }

        [Test]
        public void SetSession_WithConfig_ActiveConfigReturnsConfig()
        {
            var config = ScriptableObject.CreateInstance<SessionConfig>();

            channel.SetSession(config);

            Assert.That(channel.ActiveConfig, Is.EqualTo(config));
            Object.DestroyImmediate(config);
        }

        [Test]
        public void Clear_AfterSetSession_HasActiveSessionIsFalse()
        {
            var config = ScriptableObject.CreateInstance<SessionConfig>();
            channel.SetSession(config);

            channel.Clear();

            Assert.That(channel.HasActiveSession, Is.False);
            Object.DestroyImmediate(config);
        }

        [Test]
        public void Clear_AfterSetSession_ActiveConfigIsNull()
        {
            var config = ScriptableObject.CreateInstance<SessionConfig>();
            channel.SetSession(config);

            channel.Clear();

            Assert.That(channel.ActiveConfig, Is.Null);
            Object.DestroyImmediate(config);
        }

        [Test]
        public void SetSession_WithNull_HasActiveSessionIsFalse()
        {
            channel.SetSession(null);

            Assert.That(channel.HasActiveSession, Is.False);
        }
    }
}
#endif
