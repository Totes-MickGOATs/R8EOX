#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    public class SessionConfigTests
    {
        private SessionConfig config;

        [SetUp]
        public void SetUp()
        {
            config = ScriptableObject.CreateInstance<SessionConfig>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(config);
        }

        [Test]
        public void CreateInstance_ReturnsNonNull()
        {
            Assert.That(config, Is.Not.Null);
        }

        [Test]
        public void SessionMode_Default_IsPractice()
        {
            Assert.That(config.SessionMode, Is.EqualTo(SessionMode.Practice));
        }

        [Test]
        public void TotalLaps_Default_IsZero()
        {
            Assert.That(config.TotalLaps, Is.EqualTo(0));
        }

        [Test]
        public void AiOpponentCount_Default_IsZero()
        {
            Assert.That(config.AiOpponentCount, Is.EqualTo(0));
        }

        [Test]
        public void AiDifficultyLevel_Default_IsFive()
        {
            Assert.That(config.AiDifficultyLevel, Is.EqualTo(5));
        }

        [Test]
        public void CountdownDuration_Default_IsThree()
        {
            Assert.That(config.CountdownDuration, Is.EqualTo(3f));
        }

        [Test]
        public void MaxRaceTime_Default_IsSixHundred()
        {
            Assert.That(config.MaxRaceTime, Is.EqualTo(600f));
        }

        [Test]
        public void VehiclePrefab_Default_IsNull()
        {
            Assert.That(config.VehiclePrefab, Is.Null);
        }

        [Test]
        public void TrackScenePath_Default_IsNullOrEmpty()
        {
            Assert.That(string.IsNullOrEmpty(config.TrackScenePath), Is.True);
        }
    }
}
#endif
