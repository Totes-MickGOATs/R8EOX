#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using R8EOX.Editor;

namespace R8EOX.Tests.EditMode
{
    /// <summary>
    /// EditMode tests for the convention-based TrackBuilder.
    /// Validates terrain creation, layer configuration, environment setup,
    /// and idempotency when building the Outpost track.
    /// </summary>
    [TestFixture]
    public class OutpostTrackSetupTests
    {
        const string k_OutpostTrackPath = "Assets/Tracks/Outpost";

        [TearDown]
        public void TearDown()
        {
            foreach (var terrain in Terrain.activeTerrains)
            {
                Object.DestroyImmediate(terrain.gameObject);
            }
        }

        [Test]
        public void BuildOutpostTrack_CreatesTerrainGameObject()
        {
            TrackBuilder.Build(k_OutpostTrackPath);

            var terrainGO = GameObject.Find("OutpostTerrain");
            Assert.IsNotNull(terrainGO, "Expected a GameObject named 'OutpostTerrain' to exist.");

            var terrain = terrainGO.GetComponent<Terrain>();
            Assert.IsNotNull(terrain, "Expected 'OutpostTerrain' to have a Terrain component.");
        }

        [Test]
        public void BuildOutpostTrack_TerrainDataNotNull()
        {
            TrackBuilder.Build(k_OutpostTrackPath);

            var terrain = GameObject.Find("OutpostTerrain").GetComponent<Terrain>();
            Assert.IsNotNull(terrain, "Expected a Terrain to exist in the scene.");
            Assert.IsNotNull(terrain.terrainData, "TerrainData must be assigned.");
        }

        [Test]
        public void BuildOutpostTrack_TerrainHasTwoLayers()
        {
            TrackBuilder.Build(k_OutpostTrackPath);

            var terrain = GameObject.Find("OutpostTerrain").GetComponent<Terrain>();
            Assert.IsNotNull(terrain, "Expected a Terrain to exist in the scene.");
            Assert.IsNotNull(terrain.terrainData, "TerrainData must be assigned.");

            TerrainLayer[] layers = terrain.terrainData.terrainLayers;
            Assert.IsNotNull(layers, "TerrainLayers array must not be null.");
            Assert.AreEqual(2, layers.Length, "Expected exactly 2 terrain layers.");
        }

        [Test]
        public void BuildOutpostTrack_IsIdempotent()
        {
            TrackBuilder.Build(k_OutpostTrackPath);

            LogAssert.ignoreFailingMessages = true;
            TrackBuilder.Build(k_OutpostTrackPath);
            LogAssert.ignoreFailingMessages = false;

            var terrains = Object.FindObjectsByType<Terrain>(FindObjectsInactive.Exclude);
            Assert.AreEqual(1, terrains.Length,
                "Running the builder twice should produce exactly one Terrain.");
        }

        [Test]
        public void BuildOutpostTrack_TerrainHasMaterialAssigned()
        {
            TrackBuilder.Build(k_OutpostTrackPath);

            var terrain = GameObject.Find("OutpostTerrain").GetComponent<Terrain>();
            Assert.IsNotNull(terrain, "Expected a Terrain to exist in the scene.");
            Assert.IsNotNull(terrain.materialTemplate,
                "Terrain materialTemplate must not be null.");
        }

        [Test]
        public void BuildOutpostTrack_ConfiguresFog()
        {
            TrackBuilder.Build(k_OutpostTrackPath);

            Assert.IsTrue(RenderSettings.fog, "Fog should be enabled.");
            Assert.AreEqual(FogMode.Exponential, RenderSettings.fogMode,
                "Fog mode should be Exponential.");
        }

        [Test]
        public void BuildOutpostTrack_TerrainSizeMatchesDefaults()
        {
            TrackBuilder.Build(k_OutpostTrackPath);

            var terrain = GameObject.Find("OutpostTerrain").GetComponent<Terrain>();
            Assert.IsNotNull(terrain, "Expected a Terrain to exist in the scene.");
            Assert.IsNotNull(terrain.terrainData, "TerrainData must be assigned.");

            Vector3 size = terrain.terrainData.size;
            Assert.AreEqual(500f, size.x, 0.01f, "Terrain width (X) should be 500m.");
            Assert.AreEqual(10f, size.y, 0.01f, "Terrain height scale (Y) should be 10m.");
            Assert.AreEqual(500f, size.z, 0.01f, "Terrain length (Z) should be 500m.");
        }
    }
}
#endif
