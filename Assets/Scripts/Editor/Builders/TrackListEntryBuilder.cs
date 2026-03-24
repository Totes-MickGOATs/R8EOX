#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using BRH = R8EOX.Editor.BuilderRectHelper;

namespace R8EOX.Editor.Builders
{
    internal static class TrackListEntryBuilder
    {
        private static readonly Color PanelColor = new Color(0.078f, 0.082f, 0.102f);

        /// <summary>
        /// Creates a deactivated TrackListEntry template GameObject parented to
        /// <paramref name="parent"/>. TrackListPanel instantiates this at runtime.
        /// </summary>
        internal static GameObject Build(Transform parent)
        {
            var entryGo = new GameObject("TrackListEntry");
            entryGo.transform.SetParent(parent, false);
            var entryRt = entryGo.AddComponent<RectTransform>();
            entryRt.sizeDelta = new Vector2(0f, 60f);
            entryGo.AddComponent<Image>().color = PanelColor;
            entryGo.AddComponent<Button>();
            var tle = entryGo.AddComponent<R8EOX.Menu.Internal.TrackListEntry>();

            // Highlight overlay
            var hlGo = new GameObject("Highlight");
            hlGo.transform.SetParent(entryGo.transform, false);
            BRH.StretchFill(hlGo.AddComponent<RectTransform>());
            var hlImg = hlGo.AddComponent<Image>();
            hlImg.color = new Color(1f, 1f, 1f, 0f);

            // Name label
            var nameGo = new GameObject("NameLabel");
            nameGo.transform.SetParent(entryGo.transform, false);
            BRH.StretchFill(nameGo.AddComponent<RectTransform>());
            var nameTmp = nameGo.AddComponent<TextMeshProUGUI>();
            nameTmp.alignment = TextAlignmentOptions.MidlineLeft;
            nameTmp.fontSize = 20f;

            // Status icon
            var statusGo = new GameObject("StatusIcon");
            statusGo.transform.SetParent(entryGo.transform, false);
            var statusRt = statusGo.AddComponent<RectTransform>();
            statusRt.anchorMin = new Vector2(1f, 0.5f);
            statusRt.anchorMax = new Vector2(1f, 0.5f);
            statusRt.pivot = new Vector2(1f, 0.5f);
            statusRt.sizeDelta = new Vector2(12f, 12f);
            statusRt.anchoredPosition = new Vector2(-10f, 0f);
            var statusImg = statusGo.AddComponent<Image>();
            statusImg.color = Color.green;

            // Wire serialized fields on TrackListEntry
            var soEntry = new SerializedObject(tle);
            soEntry.FindProperty("nameLabel").objectReferenceValue      = nameTmp;
            soEntry.FindProperty("highlightOverlay").objectReferenceValue = hlImg;
            soEntry.FindProperty("statusIcon").objectReferenceValue     = statusImg;
            soEntry.ApplyModifiedProperties();

            entryGo.SetActive(false);
            return entryGo;
        }

    }
}
#endif
