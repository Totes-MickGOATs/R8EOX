using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace R8EOX.Tests.PlayMode
{
    [TestFixture]
    [Category("integration")]
    public class VehicleSpawnTests : E2ETestBase
    {
        private R8EOX.Session.SessionManager sessionManager;
        private GameObject playerVehicle;

        private IEnumerator SetUpSession()
        {
            yield return E2ETestUtils.LoadSceneAndWait("OutpostTrack");
            yield return E2ETestUtils.WaitForGameObject("[SessionManager]");
            sessionManager = Object.FindAnyObjectByType<R8EOX.Session.SessionManager>();
            Assert.IsNotNull(sessionManager, "SessionManager not found");
            yield return E2ETestUtils.WaitUntil(
                () => sessionManager.IsReady,
                timeout: 10f,
                message: "Session did not become ready");
            playerVehicle = sessionManager.PlayerVehicle;
            Assert.IsNotNull(playerVehicle, "Player vehicle not spawned");
        }

        [UnityTest]
        public IEnumerator Vehicle_Spawns_WithRigidbody()
        {
            yield return SetUpSession();
            var rb = playerVehicle.GetComponent<Rigidbody>();
            Assert.IsNotNull(rb, "Vehicle should have a Rigidbody component");
        }

        [UnityTest]
        public IEnumerator Vehicle_Spawns_WithInputComponent()
        {
            yield return SetUpSession();
            var input = playerVehicle.GetComponent<R8EOX.Input.IVehicleInput>()
                as Component;
            // IVehicleInput may be on the vehicle or could be RCInput or ScriptedInput
            if (input == null)
            {
                // Try getting any MonoBehaviour that implements IVehicleInput
                foreach (var mb in playerVehicle.GetComponents<MonoBehaviour>())
                {
                    if (mb is R8EOX.Input.IVehicleInput)
                    {
                        input = mb;
                        break;
                    }
                }
            }
            Assert.IsNotNull(input, "Vehicle should have an IVehicleInput component");
        }

        [UnityTest]
        public IEnumerator Vehicle_Spawns_AboveTerrain()
        {
            yield return SetUpSession();
            var terrain = Terrain.activeTerrain;
            if (terrain != null)
            {
                var pos = playerVehicle.transform.position;
                float terrainHeight = terrain.SampleHeight(pos)
                    + terrain.transform.position.y;
                Assert.That(pos.y, Is.GreaterThanOrEqualTo(terrainHeight - 0.5f),
                    $"Vehicle at Y={pos.y} is below terrain at Y={terrainHeight}");
            }
            else
            {
                // No terrain — just verify vehicle isn't underground
                Assert.That(playerVehicle.transform.position.y,
                    Is.GreaterThanOrEqualTo(-10f),
                    "Vehicle appears to be far underground");
            }
        }

        [UnityTest]
        public IEnumerator Vehicle_Spawns_AtSpawnPoint()
        {
            yield return SetUpSession();
            var trackManager = Object.FindAnyObjectByType<R8EOX.Track.TrackManager>();
            Assert.IsNotNull(trackManager, "TrackManager not found");
            trackManager.Initialize();
            var spawnPoint = trackManager.GetPlayerSpawnPoint();
            var vehiclePos = playerVehicle.transform.position;
            float distance = Vector3.Distance(
                new Vector3(vehiclePos.x, 0, vehiclePos.z),
                new Vector3(spawnPoint.Position.x, 0, spawnPoint.Position.z));
            // Allow generous tolerance since vehicle may have settled via physics
            Assert.That(distance, Is.LessThan(10f),
                $"Vehicle at {vehiclePos} is too far from spawn point at {spawnPoint.Position}");
        }
    }
}
