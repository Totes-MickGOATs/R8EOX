#if UNITY_EDITOR
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using R8EOX.Editor.Builders;

namespace R8EOX.Tests.EditMode
{
    /// <summary>
    /// Unit tests for TerrainSplatmapBuilder.ComputeSplatmap.
    /// Verifies the top-down remaining-weight splatmap compositing
    /// algorithm with synthetic blend-mask textures.
    /// </summary>
    [TestFixture]
    public class TerrainSplatmapBuilderTests
    {
        readonly List<Texture2D> createdTextures = new List<Texture2D>();

        [TearDown]
        public void TearDown()
        {
            foreach (var tex in createdTextures)
            {
                if (tex != null)
                    Object.DestroyImmediate(tex);
            }
            createdTextures.Clear();
        }

        [Test]
        public void ComputeSplatmap_SingleLayer_AllWeightOnBase()
        {
            var masks = new Texture2D[] { null };

            float[,,] splatmap =
                TerrainSplatmapBuilder.ComputeSplatmap(2, 1, masks);

            for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
            {
                Assert.That(splatmap[y, x, 0],
                    Is.EqualTo(1f).Within(0.01f));
            }
        }

        [Test]
        public void ComputeSplatmap_TwoLayers_WhiteMask_AllOnLayer1()
        {
            var masks = new Texture2D[]
            {
                null,
                CreateSolidTexture(Color.white)
            };

            float[,,] splatmap =
                TerrainSplatmapBuilder.ComputeSplatmap(2, 2, masks);

            for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
            {
                Assert.That(splatmap[y, x, 1],
                    Is.EqualTo(1f).Within(0.01f));
                Assert.That(splatmap[y, x, 0],
                    Is.EqualTo(0f).Within(0.01f));
            }
        }

        [Test]
        public void ComputeSplatmap_TwoLayers_BlackMask_AllOnBase()
        {
            var masks = new Texture2D[]
            {
                null,
                CreateSolidTexture(Color.black)
            };

            float[,,] splatmap =
                TerrainSplatmapBuilder.ComputeSplatmap(2, 2, masks);

            for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
            {
                Assert.That(splatmap[y, x, 0],
                    Is.EqualTo(1f).Within(0.01f));
                Assert.That(splatmap[y, x, 1],
                    Is.EqualTo(0f).Within(0.01f));
            }
        }

        [Test]
        public void ComputeSplatmap_TwoLayers_HalfMask_EvenSplit()
        {
            var gray = new Color(0.5f, 0.5f, 0.5f, 1f);
            var masks = new Texture2D[]
            {
                null,
                CreateSolidTexture(gray)
            };

            float[,,] splatmap =
                TerrainSplatmapBuilder.ComputeSplatmap(2, 2, masks);

            for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
            {
                Assert.That(splatmap[y, x, 1],
                    Is.EqualTo(0.5f).Within(0.01f));
                Assert.That(splatmap[y, x, 0],
                    Is.EqualTo(0.5f).Within(0.01f));
            }
        }

        [Test]
        public void ComputeSplatmap_TwoLayers_NullMask_AllOnBase()
        {
            var masks = new Texture2D[] { null, null };

            float[,,] splatmap =
                TerrainSplatmapBuilder.ComputeSplatmap(2, 2, masks);

            for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
            {
                Assert.That(splatmap[y, x, 0],
                    Is.EqualTo(1f).Within(0.01f));
                Assert.That(splatmap[y, x, 1],
                    Is.EqualTo(0f).Within(0.01f));
            }
        }

        [Test]
        public void ComputeSplatmap_ThreeLayers_TopDownCompositing()
        {
            // masks[2] grayscale=0.5, masks[1] grayscale=0.6
            // Layer 2: remaining=1.0, weight=0.5, remaining=0.5
            // Layer 1: remaining=0.5, weight=0.3, remaining=0.2
            // Layer 0: 0.2
            var masks = new Texture2D[]
            {
                null,
                CreateSolidTexture(new Color(0.6f, 0.6f, 0.6f, 1f)),
                CreateSolidTexture(new Color(0.5f, 0.5f, 0.5f, 1f))
            };

            float[,,] splatmap =
                TerrainSplatmapBuilder.ComputeSplatmap(2, 3, masks);

            for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
            {
                Assert.That(splatmap[y, x, 2],
                    Is.EqualTo(0.5f).Within(0.01f));
                Assert.That(splatmap[y, x, 1],
                    Is.EqualTo(0.3f).Within(0.01f));
                Assert.That(splatmap[y, x, 0],
                    Is.EqualTo(0.2f).Within(0.01f));
            }
        }

        [Test]
        public void ComputeSplatmap_WeightsSumToOne()
        {
            var masks = new Texture2D[]
            {
                null,
                CreateSolidTexture(new Color(0.6f, 0.6f, 0.6f, 1f)),
                CreateSolidTexture(new Color(0.5f, 0.5f, 0.5f, 1f))
            };

            float[,,] splatmap =
                TerrainSplatmapBuilder.ComputeSplatmap(2, 3, masks);

            for (int y = 0; y < 2; y++)
            for (int x = 0; x < 2; x++)
            {
                float sum = 0f;
                for (int layer = 0; layer < 3; layer++)
                    sum += splatmap[y, x, layer];

                Assert.That(sum, Is.EqualTo(1f).Within(0.01f));
            }
        }

        Texture2D CreateSolidTexture(Color color)
        {
            var tex = new Texture2D(2, 2);
            tex.SetPixels(new[] { color, color, color, color });
            tex.Apply();
            createdTextures.Add(tex);
            return tex;
        }
    }
}
#endif
