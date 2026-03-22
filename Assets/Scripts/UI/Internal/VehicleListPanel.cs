using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace R8EOX.UI.Internal
{
    internal class VehicleListPanel : MonoBehaviour
    {
        [SerializeField] private Transform listContent;
        [SerializeField] private GameObject listEntryPrefab;
        [SerializeField] private TMP_InputField searchField;
        [SerializeField] private TMP_Dropdown categoryDropdown;

        private VehicleDefinition[] allVehicles;
        private List<VehicleDefinition> filteredVehicles = new List<VehicleDefinition>();
        private List<VehicleListEntry> entryInstances = new List<VehicleListEntry>();
        private int selectedIndex = -1;
        private Action<VehicleDefinition> onSelectionChanged;

        internal int SelectedIndex => selectedIndex;

        internal VehicleDefinition SelectedVehicle =>
            selectedIndex >= 0 && selectedIndex < filteredVehicles.Count
                ? filteredVehicles[selectedIndex]
                : null;

        internal void Initialize(
            VehicleDefinition[] vehicles,
            Action<VehicleDefinition> selectionCallback)
        {
            allVehicles = vehicles;
            onSelectionChanged = selectionCallback;

            PopulateCategoryDropdown();

            if (searchField != null)
            {
                searchField.onValueChanged.AddListener(OnSearchChanged);
            }

            if (categoryDropdown != null)
            {
                categoryDropdown.onValueChanged.AddListener(OnCategoryChanged);
            }

            RebuildList();
        }

        internal void RebuildList()
        {
            DestroyEntries();
            filteredVehicles.Clear();

            string searchText = searchField != null ? searchField.text : string.Empty;
            int categoryIndex = categoryDropdown != null ? categoryDropdown.value : 0;

            for (int i = 0; i < allVehicles.Length; i++)
            {
                var vehicle = allVehicles[i];

                if (!string.IsNullOrEmpty(searchText) &&
                    !vehicle.DisplayName.Contains(
                        searchText, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (categoryIndex > 0)
                {
                    var targetCategory = (VehicleCategory)(categoryIndex - 1);
                    if (vehicle.Category != targetCategory)
                    {
                        continue;
                    }
                }

                filteredVehicles.Add(vehicle);
            }

            for (int i = 0; i < filteredVehicles.Count; i++)
            {
                var instance = Instantiate(listEntryPrefab, listContent);
                var entry = instance.GetComponent<VehicleListEntry>();
                entry.Configure(i, filteredVehicles[i], OnEntryClicked);
                entryInstances.Add(entry);
            }

            if (selectedIndex >= 0 && selectedIndex < filteredVehicles.Count)
            {
                SelectIndex(selectedIndex);
            }
            else if (filteredVehicles.Count > 0)
            {
                SelectIndex(0);
            }
            else
            {
                selectedIndex = -1;
                onSelectionChanged?.Invoke(null);
            }
        }

        internal void SelectIndex(int index)
        {
            if (index < 0 || index >= filteredVehicles.Count)
            {
                return;
            }

            if (selectedIndex >= 0 && selectedIndex < entryInstances.Count)
            {
                entryInstances[selectedIndex].SetSelected(false);
            }

            selectedIndex = index;
            entryInstances[selectedIndex].SetSelected(true);
            onSelectionChanged?.Invoke(filteredVehicles[selectedIndex]);
        }

        internal void Teardown()
        {
            if (searchField != null)
            {
                searchField.onValueChanged.RemoveListener(OnSearchChanged);
            }

            if (categoryDropdown != null)
            {
                categoryDropdown.onValueChanged.RemoveListener(OnCategoryChanged);
            }

            DestroyEntries();
            filteredVehicles.Clear();
            allVehicles = null;
            onSelectionChanged = null;
            selectedIndex = -1;
        }

        private void PopulateCategoryDropdown()
        {
            if (categoryDropdown == null)
            {
                return;
            }

            categoryDropdown.ClearOptions();

            var options = new List<string> { "All" };
            var categories = Enum.GetNames(typeof(VehicleCategory));
            options.AddRange(categories);
            categoryDropdown.AddOptions(options);
            categoryDropdown.value = 0;
        }

        private void OnSearchChanged(string text)
        {
            RebuildList();
        }

        private void OnCategoryChanged(int dropdownIndex)
        {
            RebuildList();
        }

        private void OnEntryClicked(int index)
        {
            SelectIndex(index);
        }

        private void DestroyEntries()
        {
            for (int i = 0; i < entryInstances.Count; i++)
            {
                if (entryInstances[i] != null)
                {
                    Destroy(entryInstances[i].gameObject);
                }
            }

            entryInstances.Clear();
        }
    }
}
