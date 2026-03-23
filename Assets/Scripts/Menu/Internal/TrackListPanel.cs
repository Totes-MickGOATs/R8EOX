using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace R8EOX.Menu.Internal
{
    internal class TrackListPanel : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private Transform listContent;
        [SerializeField] private GameObject trackEntryPrefab;

        [Header("Search")]
        [Tooltip("Optional — leave unassigned to disable search filtering")]
        [SerializeField] private TMP_InputField searchField;

        private TrackDefinition[] allTracks;
        private readonly List<TrackDefinition> filteredTracks = new();
        private readonly List<TrackListEntry> entryInstances = new();
        private int selectedIndex = -1;
        private Action<TrackDefinition> onSelectionChanged;

        internal void Initialize(TrackDefinition[] tracks, Action<TrackDefinition> selectionCallback)
        {
            allTracks          = tracks;
            onSelectionChanged = selectionCallback;

            if (searchField != null)
            {
                searchField.onValueChanged.AddListener(OnSearchChanged);
            }

            RebuildList();
        }

        internal void Teardown()
        {
            if (searchField != null)
            {
                searchField.onValueChanged.RemoveListener(OnSearchChanged);
            }

            DestroyEntries();
            filteredTracks.Clear();
            selectedIndex = -1;
        }

        private void RebuildList()
        {
            DestroyEntries();
            filteredTracks.Clear();

            string query = searchField != null ? searchField.text : string.Empty;

            foreach (TrackDefinition track in allTracks)
            {
                if (string.IsNullOrEmpty(query) ||
                    track.DisplayName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    filteredTracks.Add(track);
                }
            }

            for (int i = 0; i < filteredTracks.Count; i++)
            {
                GameObject go    = Instantiate(trackEntryPrefab, listContent);
                TrackListEntry entry = go.GetComponent<TrackListEntry>();
                entry.Configure(i, filteredTracks[i], OnEntryClicked);
                entryInstances.Add(entry);
            }

            selectedIndex = -1;

            if (entryInstances.Count > 0)
            {
                SelectIndex(0);
            }
        }

        private void SelectIndex(int index)
        {
            if (selectedIndex >= 0 && selectedIndex < entryInstances.Count)
            {
                entryInstances[selectedIndex].SetSelected(false);
            }

            selectedIndex = index;

            if (selectedIndex >= 0 && selectedIndex < entryInstances.Count)
            {
                entryInstances[selectedIndex].SetSelected(true);
                onSelectionChanged?.Invoke(filteredTracks[selectedIndex]);
            }
        }

        private void OnSearchChanged(string text)
        {
            RebuildList();
        }

        private void OnEntryClicked(int index)
        {
            SelectIndex(index);
        }

        private void DestroyEntries()
        {
            foreach (TrackListEntry entry in entryInstances)
            {
                if (entry != null)
                {
                    Destroy(entry.gameObject);
                }
            }

            entryInstances.Clear();
        }
    }
}
