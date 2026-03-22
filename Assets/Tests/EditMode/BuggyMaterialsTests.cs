#if UNITY_EDITOR
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using R8EOX.Editor;

namespace R8EOX.Tests.EditMode
{
    /// <summary>
    /// EditMode tests for AddBuggyMaterials.
    /// Creates a temporary RCBuggy prefab with the expected hierarchy,
    /// runs the material applicator, and validates results.
    /// </summary>
    [TestFixture]
    public class BuggyMaterialsTests
    {
        const string k_PrefabPath = "Assets/Prefabs/RCBuggy.prefab";
        const string k_MaterialsDir = "Assets/Materials/Vehicle";

        bool prefabExistedBefore;

        [SetUp]
        public void SetUp()
        {
            prefabExistedBefore = File.Exists(
                Path.Combine(Application.dataPath, "..", k_PrefabPath));

            if (!prefabExistedBefore)
            {
                EnsureDirectoryExists("Assets/Prefabs");
                CreateMinimalBuggyPrefab();
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (!prefabExistedBefore)
            {
                AssetDatabase.DeleteAsset(k_PrefabPath);
            }

            CleanupMaterials();
            AssetDatabase.Refresh();
        }

        [Test]
        public void Apply_BodyShellHasBlueSemiMaterial()
        {
            AddBuggyMaterials.Apply();

            var root = PrefabUtility.LoadPrefabContents(k_PrefabPath);
            try
            {
                var bodyShell = root.transform.Find("BodyShell");
                Assert.IsNotNull(bodyShell, "BodyShell child not found");

                var renderer = bodyShell.GetComponent<Renderer>();
                Assert.IsNotNull(renderer, "BodyShell has no Renderer");
                Assert.IsNotNull(renderer.sharedMaterial,
                    "BodyShell material is null");
                Assert.AreEqual("BlueSemi",
                    renderer.sharedMaterial.name);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        [Test]
        public void Apply_WheelVisualHasBlackTireMaterial()
        {
            AddBuggyMaterials.Apply();

            var root = PrefabUtility.LoadPrefabContents(k_PrefabPath);
            try
            {
                var wheelFL = root.transform.Find("WheelFL");
                Assert.IsNotNull(wheelFL, "WheelFL not found");

                var tireVisual = wheelFL.Find("WheelVisual");
                Assert.IsNotNull(tireVisual,
                    "WheelVisual not found under WheelFL");

                var renderer = tireVisual.GetComponent<Renderer>();
                Assert.IsNotNull(renderer,
                    "WheelVisual has no Renderer");
                Assert.IsNotNull(renderer.sharedMaterial,
                    "WheelVisual material is null");
                Assert.AreEqual("BlackTire",
                    renderer.sharedMaterial.name);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        [Test]
        public void Apply_MaterialUsesUrpLitShader()
        {
            AddBuggyMaterials.Apply();

            var mat = AssetDatabase.LoadAssetAtPath<Material>(
                $"{k_MaterialsDir}/BlueSemi.mat");
            Assert.IsNotNull(mat, "BlueSemi material asset must exist");
            Assert.AreEqual("Universal Render Pipeline/Lit",
                mat.shader.name,
                "Material should use URP Lit shader");
        }

        [Test]
        public void Apply_MaterialUsesBaseColorProperty()
        {
            AddBuggyMaterials.Apply();

            var mat = AssetDatabase.LoadAssetAtPath<Material>(
                $"{k_MaterialsDir}/BlueSemi.mat");
            Assert.IsNotNull(mat, "BlueSemi material asset must exist");
            Assert.IsTrue(mat.HasProperty("_BaseColor"),
                "URP Lit material must have _BaseColor property");

            Color baseColor = mat.GetColor("_BaseColor");
            Assert.AreEqual(0.18f, baseColor.r, 0.01f,
                "BaseColor red channel");
            Assert.AreEqual(0.45f, baseColor.g, 0.01f,
                "BaseColor green channel");
            Assert.AreEqual(0.90f, baseColor.b, 0.01f,
                "BaseColor blue channel");
        }

        [Test]
        public void Apply_CreatesAllSixMaterialAssets()
        {
            AddBuggyMaterials.Apply();

            string[] expectedMats =
            {
                "DarkGrey", "MediumGrey", "BlueSolid",
                "BlueSemi", "BlackTire", "WhiteHub"
            };

            foreach (string matName in expectedMats)
            {
                string path = $"{k_MaterialsDir}/{matName}.mat";
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                Assert.IsNotNull(mat,
                    $"Expected material asset at {path}");
            }
        }

        [Test]
        public void Apply_CalledTwice_NoErrors()
        {
            LogAssert.NoUnexpectedReceived();
            AddBuggyMaterials.Apply();
            AddBuggyMaterials.Apply();
        }

        [Test]
        public void Apply_HubVisualHasWhiteHubMaterial()
        {
            AddBuggyMaterials.Apply();

            var root = PrefabUtility.LoadPrefabContents(k_PrefabPath);
            try
            {
                var wheelFL = root.transform.Find("WheelFL");
                Assert.IsNotNull(wheelFL, "WheelFL not found");

                var hubVisual = wheelFL.Find("HubVisual");
                Assert.IsNotNull(hubVisual,
                    "HubVisual not found under WheelFL");

                var renderer = hubVisual.GetComponent<Renderer>();
                Assert.IsNotNull(renderer,
                    "HubVisual has no Renderer");
                Assert.IsNotNull(renderer.sharedMaterial,
                    "HubVisual material is null");
                Assert.AreEqual("WhiteHub",
                    renderer.sharedMaterial.name);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        // ── Helpers ─────────────────────────────────────────────────

        static void EnsureDirectoryExists(string assetPath)
        {
            string fullPath = Path.Combine(
                Application.dataPath, "..", assetPath);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                AssetDatabase.Refresh();
            }
        }

        static void CreateMinimalBuggyPrefab()
        {
            var root = new GameObject("RCBuggy");

            AddMeshChild(root.transform, "ChassisPlate");
            AddMeshChild(root.transform, "FrontBumperMesh");
            AddMeshChild(root.transform, "RearBumperMesh");
            AddMeshChild(root.transform, "FrontShockTower");
            AddMeshChild(root.transform, "RearShockTower");
            AddMeshChild(root.transform, "BodyShell");
            AddMeshChild(root.transform, "RearWing");
            AddMeshChild(root.transform, "FrontArmL");
            AddMeshChild(root.transform, "FrontArmR");
            AddMeshChild(root.transform, "RearArmL");
            AddMeshChild(root.transform, "RearArmR");

            foreach (string wheelName in new[]
                { "WheelFL", "WheelFR", "WheelRL", "WheelRR" })
            {
                var wheel = new GameObject(wheelName);
                wheel.transform.SetParent(root.transform);
                AddMeshChild(wheel.transform, "WheelVisual");
                AddMeshChild(wheel.transform, "HubVisual");
            }

            PrefabUtility.SaveAsPrefabAsset(root, k_PrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.Refresh();
        }

        static void AddMeshChild(Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
        }

        void CleanupMaterials()
        {
            string[] matNames =
            {
                "DarkGrey", "MediumGrey", "BlueSolid",
                "BlueSemi", "BlackTire", "WhiteHub"
            };

            foreach (string matName in matNames)
            {
                string path = $"{k_MaterialsDir}/{matName}.mat";
                if (AssetDatabase.LoadAssetAtPath<Material>(path) != null)
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }
        }
    }
}
#endif
