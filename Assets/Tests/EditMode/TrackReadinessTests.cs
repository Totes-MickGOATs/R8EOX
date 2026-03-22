#if UNITY_EDITOR
using NUnit.Framework;
using TrackReadiness = R8EOX.Session.Internal.TrackReadiness;

namespace R8EOX.Tests.EditMode
{
    [TestFixture]
    public class TrackReadinessTests
    {
        [Test]
        public void IsPlayable_WithSpawnPoints_IsTrue()
        {
            var readiness = new TrackReadiness { HasSpawnPoints = true };

            Assert.That(readiness.IsPlayable, Is.True);
        }

        [Test]
        public void IsPlayable_WithoutSpawnPoints_IsFalse()
        {
            var readiness = new TrackReadiness { HasSpawnPoints = false };

            Assert.That(readiness.IsPlayable, Is.False);
        }

        [Test]
        public void IsTimeTrialReady_Circuit_RequiresSpawnPointsAndCheckpoints()
        {
            var ready = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasCheckpoints = true
            };
            Assert.That(ready.IsTimeTrialReady(TrackType.Circuit), Is.True);

            var missingCheckpoints = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasCheckpoints = false
            };
            Assert.That(missingCheckpoints.IsTimeTrialReady(TrackType.Circuit), Is.False);

            var missingSpawns = new TrackReadiness
            {
                HasSpawnPoints = false,
                HasCheckpoints = true
            };
            Assert.That(missingSpawns.IsTimeTrialReady(TrackType.Circuit), Is.False);
        }

        [Test]
        public void IsTimeTrialReady_PointToPoint_RequiresSpawnPointsAndFinishTrigger()
        {
            var ready = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasFinishTrigger = true
            };
            Assert.That(ready.IsTimeTrialReady(TrackType.PointToPoint), Is.True);

            var missingFinish = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasFinishTrigger = false
            };
            Assert.That(missingFinish.IsTimeTrialReady(TrackType.PointToPoint), Is.False);

            var missingSpawns = new TrackReadiness
            {
                HasSpawnPoints = false,
                HasFinishTrigger = true
            };
            Assert.That(missingSpawns.IsTimeTrialReady(TrackType.PointToPoint), Is.False);
        }

        [Test]
        public void IsRaceReady_Circuit_RequiresAllCircuitComponents()
        {
            var ready = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasMultipleSpawnPoints = true,
                HasCheckpoints = true,
                HasCenterline = true
            };
            Assert.That(ready.IsRaceReady(TrackType.Circuit), Is.True);
        }

        [Test]
        public void IsRaceReady_Circuit_MissingAnyComponent_IsFalse()
        {
            var missingSpawns = new TrackReadiness
            {
                HasSpawnPoints = false,
                HasMultipleSpawnPoints = true,
                HasCheckpoints = true,
                HasCenterline = true
            };
            Assert.That(missingSpawns.IsRaceReady(TrackType.Circuit), Is.False);

            var missingMultiSpawns = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasMultipleSpawnPoints = false,
                HasCheckpoints = true,
                HasCenterline = true
            };
            Assert.That(missingMultiSpawns.IsRaceReady(TrackType.Circuit), Is.False);

            var missingCheckpoints = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasMultipleSpawnPoints = true,
                HasCheckpoints = false,
                HasCenterline = true
            };
            Assert.That(missingCheckpoints.IsRaceReady(TrackType.Circuit), Is.False);

            var missingCenterline = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasMultipleSpawnPoints = true,
                HasCheckpoints = true,
                HasCenterline = false
            };
            Assert.That(missingCenterline.IsRaceReady(TrackType.Circuit), Is.False);
        }

        [Test]
        public void IsRaceReady_PointToPoint_RequiresAllP2PComponents()
        {
            var ready = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasMultipleSpawnPoints = true,
                HasFinishTrigger = true,
                HasCenterline = true
            };
            Assert.That(ready.IsRaceReady(TrackType.PointToPoint), Is.True);
        }

        [Test]
        public void IsRaceReady_PointToPoint_MissingAnyComponent_IsFalse()
        {
            var missingSpawns = new TrackReadiness
            {
                HasSpawnPoints = false,
                HasMultipleSpawnPoints = true,
                HasFinishTrigger = true,
                HasCenterline = true
            };
            Assert.That(missingSpawns.IsRaceReady(TrackType.PointToPoint), Is.False);

            var missingMultiSpawns = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasMultipleSpawnPoints = false,
                HasFinishTrigger = true,
                HasCenterline = true
            };
            Assert.That(missingMultiSpawns.IsRaceReady(TrackType.PointToPoint), Is.False);

            var missingFinish = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasMultipleSpawnPoints = true,
                HasFinishTrigger = false,
                HasCenterline = true
            };
            Assert.That(missingFinish.IsRaceReady(TrackType.PointToPoint), Is.False);

            var missingCenterline = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasMultipleSpawnPoints = true,
                HasFinishTrigger = true,
                HasCenterline = false
            };
            Assert.That(missingCenterline.IsRaceReady(TrackType.PointToPoint), Is.False);
        }

        [Test]
        public void GetMissingReport_AllPresent_ReturnsReady()
        {
            var readiness = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasMultipleSpawnPoints = true,
                HasCheckpoints = true,
                HasCenterline = true,
                HasFinishTrigger = true
            };

            string report = readiness.GetMissingReport(
                SessionMode.Race, TrackType.Circuit);

            Assert.That(report, Is.EqualTo("Track is ready"));
        }

        [Test]
        public void GetMissingReport_MissingSpawnPoints_ReportsIt()
        {
            var readiness = new TrackReadiness
            {
                HasSpawnPoints = false
            };

            string report = readiness.GetMissingReport(
                SessionMode.Practice, TrackType.Circuit);

            Assert.That(report, Does.Contain("SpawnPoint(s)"));
        }

        [Test]
        public void GetMissingReport_RaceMissingMultipleSpawnPoints_ReportsIt()
        {
            var readiness = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasMultipleSpawnPoints = false,
                HasCheckpoints = true,
                HasCenterline = true
            };

            string report = readiness.GetMissingReport(
                SessionMode.Race, TrackType.Circuit);

            Assert.That(report, Does.Contain("multiple SpawnPoints"));
        }

        [Test]
        public void GetMissingReport_TimeTrialCircuit_MissingCheckpoints_ReportsIt()
        {
            var readiness = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasCheckpoints = false
            };

            string report = readiness.GetMissingReport(
                SessionMode.TimeTrial, TrackType.Circuit);

            Assert.That(report, Does.Contain("Checkpoints"));
        }

        [Test]
        public void GetMissingReport_TimeTrialP2P_MissingFinish_ReportsIt()
        {
            var readiness = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasFinishTrigger = false
            };

            string report = readiness.GetMissingReport(
                SessionMode.TimeTrial, TrackType.PointToPoint);

            Assert.That(report, Does.Contain("Finish trigger"));
        }

        [Test]
        public void GetMissingReport_RaceMissingCenterline_ReportsIt()
        {
            var readiness = new TrackReadiness
            {
                HasSpawnPoints = true,
                HasMultipleSpawnPoints = true,
                HasCheckpoints = true,
                HasCenterline = false
            };

            string report = readiness.GetMissingReport(
                SessionMode.Race, TrackType.Circuit);

            Assert.That(report, Does.Contain("Centerline"));
        }
    }
}
#endif
