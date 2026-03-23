#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using LD = R8EOX.Editor.Builders.VehicleSelectLayoutData;

namespace R8EOX.Editor.Builders
{
    internal static class VehicleListEntryBuilder
    {
        internal static void BuildEntryPrefab()
        {
            const string entryPath = "Assets/Prefabs/UI/VehicleListEntry.prefab";
            var entryRoot = PrefabUtility.LoadPrefabContents(entryPath);

            var le = entryRoot.GetComponent<LayoutElement>();
            if (le == null) le = entryRoot.AddComponent<LayoutElement>();
            le.preferredHeight = 60;
            le.minHeight = 60;

            ApplyHighlight(entryRoot);
            ApplyThumbnail(entryRoot);
            ApplyNameText(entryRoot);
            ApplyCategoryText(entryRoot);

            PrefabUtility.SaveAsPrefabAsset(entryRoot, entryPath);
            PrefabUtility.UnloadPrefabContents(entryRoot);
        }

        private static void ApplyHighlight(GameObject entryRoot)
        {
            var t = entryRoot.transform.Find("Highlight");
            if (t == null) { Debug.LogWarning("[VehicleListEntryBuilder] Highlight not found."); return; }
            LD.SetRectStretch(t.GetComponent<RectTransform>());
            var img = t.GetComponent<Image>();
            if (img != null) img.color = LD.EntryNormal;
        }

        private static void ApplyThumbnail(GameObject entryRoot)
        {
            var t = entryRoot.transform.Find("Thumbnail");
            if (t == null) return;
            VehicleSelectLayoutBuilder.SetRect(t.GetComponent<RectTransform>(),
                new Vector2(0, 0.5f), new Vector2(0, 0.5f),
                new Vector2(5, -25), new Vector2(55, 25), new Vector2(0, 0.5f));
        }

        private static void ApplyNameText(GameObject entryRoot)
        {
            var t = entryRoot.transform.Find("NameText");
            if (t == null) return;
            VehicleSelectLayoutBuilder.SetRect(t.GetComponent<RectTransform>(),
                new Vector2(0, 0.5f), new Vector2(1, 1),
                new Vector2(60, 0), new Vector2(-10, -5), new Vector2(0, 0.5f));
            var tmp = t.GetComponent<TextMeshProUGUI>();
            if (tmp == null) return;
            tmp.fontSize = 18;
            tmp.color = LD.TextWhite;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
        }

        private static void ApplyCategoryText(GameObject entryRoot)
        {
            var t = entryRoot.transform.Find("CategoryText");
            if (t == null) return;
            VehicleSelectLayoutBuilder.SetRect(t.GetComponent<RectTransform>(),
                new Vector2(0, 0), new Vector2(1, 0.5f),
                new Vector2(60, 5), new Vector2(-10, 0), new Vector2(0, 0.5f));
            var tmp = t.GetComponent<TextMeshProUGUI>();
            if (tmp == null) return;
            tmp.fontSize = 13;
            tmp.color = LD.TextGrey;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
        }
    }
}
#endif
