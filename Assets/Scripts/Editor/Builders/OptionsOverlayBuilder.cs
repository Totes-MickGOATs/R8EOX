#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace R8EOX.Editor.Builders
{
    internal static class OptionsOverlayBuilder
    {
        private const string PrefabPath = "Assets/Prefabs/UI/OptionsOverlay.prefab";

        [MenuItem("R8EOX/Build Options Overlay")]
        private static void Build()
        {
            EnsureDirectory();

            var go = new GameObject("OptionsOverlay");
            go.AddComponent<R8EOX.UI.Internal.OptionsOverlay>();

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabPath);
            Object.DestroyImmediate(go);

            WireOverlayRegistry(prefab);

            AssetDatabase.SaveAssets();
            Debug.Log("[OptionsOverlayBuilder] Prefab saved to " + PrefabPath);
        }

        private static void EnsureDirectory()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI"))
                AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
        }

        private static void WireOverlayRegistry(GameObject prefab)
        {
            var guids = AssetDatabase.FindAssets("t:OverlayRegistry");
            if (guids.Length == 0)
            {
                Debug.LogWarning("[OptionsOverlayBuilder] No OverlayRegistry asset found. Create one and assign manually.");
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var registry = AssetDatabase.LoadAssetAtPath<R8EOX.OverlayRegistry>(path);
            if (registry == null)
                return;

            var so = new SerializedObject(registry);
            so.FindProperty("optionsOverlayPrefab").objectReferenceValue = prefab;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(registry);
            Debug.Log("[OptionsOverlayBuilder] Wired prefab into OverlayRegistry.");
        }
    }
}
#endif
