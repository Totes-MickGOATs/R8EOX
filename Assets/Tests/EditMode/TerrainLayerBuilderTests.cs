#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using R8EOX.Editor.Builders;

namespace R8EOX.Tests.EditMode
{
    /// <summary>
    /// Unit tests for TerrainLayerBuilder.RemapArmPixelToMohs.
    /// Verifies ARM (AO=R, Roughness=G, Metallic=B) to
    /// URP MOHS (Metallic=R, Occlusion=G, Height=B, Smoothness=A) remapping.
    /// </summary>
    [TestFixture]
    public class TerrainLayerBuilderTests
    {
        [Test]
        public void RemapArmPixelToMohs_MetallicChannel_CopiedFromArmBlue()
        {
            var arm = new Color(0f, 0f, 0.8f, 1f);

            Color mohs = TerrainLayerBuilder.RemapArmPixelToMohs(arm);

            Assert.That(mohs.r, Is.EqualTo(0.8f).Within(0.001f));
        }

        [Test]
        public void RemapArmPixelToMohs_OcclusionChannel_CopiedFromArmRed()
        {
            var arm = new Color(0.6f, 0f, 0f, 1f);

            Color mohs = TerrainLayerBuilder.RemapArmPixelToMohs(arm);

            Assert.That(mohs.g, Is.EqualTo(0.6f).Within(0.001f));
        }

        [Test]
        public void RemapArmPixelToMohs_HeightChannel_DefaultHalf()
        {
            var arm = new Color(0.3f, 0.7f, 0.1f, 1f);

            Color mohs = TerrainLayerBuilder.RemapArmPixelToMohs(arm);

            Assert.That(mohs.b, Is.EqualTo(0.5f).Within(0.001f));
        }

        [Test]
        public void RemapArmPixelToMohs_SmoothnessChannel_InvertedFromArmGreen()
        {
            var arm = new Color(0f, 0.3f, 0f, 1f);

            Color mohs = TerrainLayerBuilder.RemapArmPixelToMohs(arm);

            Assert.That(mohs.a, Is.EqualTo(0.7f).Within(0.001f));
        }

        [Test]
        public void RemapArmPixelToMohs_FullBlack_ProducesCorrectMohs()
        {
            var arm = new Color(0f, 0f, 0f, 1f);

            Color mohs = TerrainLayerBuilder.RemapArmPixelToMohs(arm);

            Assert.That(mohs.r, Is.EqualTo(0f).Within(0.001f));
            Assert.That(mohs.g, Is.EqualTo(0f).Within(0.001f));
            Assert.That(mohs.b, Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(mohs.a, Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        public void RemapArmPixelToMohs_FullWhite_ProducesCorrectMohs()
        {
            var arm = new Color(1f, 1f, 1f, 1f);

            Color mohs = TerrainLayerBuilder.RemapArmPixelToMohs(arm);

            Assert.That(mohs.r, Is.EqualTo(1f).Within(0.001f));
            Assert.That(mohs.g, Is.EqualTo(1f).Within(0.001f));
            Assert.That(mohs.b, Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(mohs.a, Is.EqualTo(0f).Within(0.001f));
        }
    }
}
#endif
