#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Shared helpers for creating or loading Material and PhysicsMaterial assets
    /// from editor builder scripts.
    /// </summary>
    internal static class BuilderMaterialHelper
    {
        private const string k_UrpLitShader  = "Universal Render Pipeline/Lit";
        private const string k_VehicleMatDir = "Assets/Materials/Vehicle";
        private const string k_PhysicsMatDir = "Assets/Materials/Physics";

        /// <summary>
        /// Loads an existing URP Lit material from the Vehicle materials folder, or
        /// creates one if it does not exist. Updates the base color in either case.
        /// </summary>
        internal static Material GetOrCreateMaterial(string name, Color color,
            bool transparent = false)
        {
            string path = $"{k_VehicleMatDir}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
            {
                existing.SetColor("_BaseColor", color);
                EditorUtility.SetDirty(existing);
                return existing;
            }

            EnsureDirectory(k_VehicleMatDir);
            var mat = new Material(Shader.Find(k_UrpLitShader)) { name = name };
            mat.SetColor("_BaseColor", color);

            if (transparent)
            {
                mat.SetFloat("_Surface", 1f);
                mat.SetFloat("_Blend", 0f);
                mat.SetFloat("_AlphaClip", 0f);
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.renderQueue = 3000;
            }

            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        /// <summary>
        /// Loads an existing PhysicsMaterial asset from the Physics materials folder, or
        /// creates one if it does not exist. Updates friction/bounce values in either case.
        /// </summary>
        internal static PhysicsMaterial GetOrCreatePhysicsMaterial(string name,
            float bounce, float friction,
            PhysicsMaterialCombine bounceCombine = PhysicsMaterialCombine.Average)
        {
            string path = $"{k_PhysicsMatDir}/{name}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(path);
            if (existing != null)
            {
                existing.bounciness      = bounce;
                existing.dynamicFriction = friction;
                existing.staticFriction  = friction;
                existing.bounceCombine   = bounceCombine;
                existing.frictionCombine = PhysicsMaterialCombine.Average;
                EditorUtility.SetDirty(existing);
                return existing;
            }

            EnsureDirectory(k_PhysicsMatDir);
            var mat = new PhysicsMaterial(name)
            {
                bounciness      = bounce,
                dynamicFriction = friction,
                staticFriction  = friction,
                bounceCombine   = bounceCombine,
                frictionCombine = PhysicsMaterialCombine.Average,
            };
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        // ---- private ----

        private static void EnsureDirectory(string assetRelativePath)
        {
            System.IO.Directory.CreateDirectory(
                System.IO.Path.Combine(Application.dataPath, "..", assetRelativePath));
        }
    }
}
#endif
