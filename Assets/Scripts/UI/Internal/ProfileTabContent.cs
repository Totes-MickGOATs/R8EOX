using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.UI.Internal
{
    internal class ProfileTabContent : MonoBehaviour
    {
        private R8EOX.Settings.SettingsManager settingsManager;
        private ToastManager toastManager;
        private TMP_Dropdown profileDropdown;

        internal void Initialize(R8EOX.Settings.SettingsManager settings, ToastManager toast)
        {
            settingsManager = settings;
            toastManager = toast;
            BuildUI();
        }

        private void BuildUI()
        {
            var vlg = gameObject.GetComponent<VerticalLayoutGroup>();
            if (vlg == null)
            {
                vlg = gameObject.AddComponent<VerticalLayoutGroup>();
            }

            vlg.spacing = 8f;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.padding = new RectOffset(0, 0, 0, 0);

            OptionsUIFactory.CreateSectionHeader(transform, "Active Profile");

            var profileNames = settingsManager.GetProfileNames();
            int activeIndex = System.Array.IndexOf(profileNames, settingsManager.ActiveProfileName);
            profileDropdown = OptionsUIFactory.CreateDropdownRow(
                transform,
                "Profile",
                profileNames,
                Mathf.Max(0, activeIndex),
                OnProfileSelected
            );

            var btnRow = new GameObject("ButtonRow", typeof(RectTransform));
            btnRow.transform.SetParent(transform, false);

            var btnRowLayout = btnRow.AddComponent<LayoutElement>();
            btnRowLayout.minHeight = 40f;

            var hg = btnRow.AddComponent<HorizontalLayoutGroup>();
            hg.spacing = 8f;
            hg.childForceExpandWidth = true;
            hg.childForceExpandHeight = true;
            hg.padding = new RectOffset(0, 0, 0, 0);

            OptionsUIFactory.CreateActionButton(
                btnRow.transform, "RENAME", OptionsUIFactory.STYLE_SECONDARY, OnRenamePressed);
            OptionsUIFactory.CreateActionButton(
                btnRow.transform, "NEW", OptionsUIFactory.STYLE_PRIMARY, OnNewPressed);
            OptionsUIFactory.CreateActionButton(
                btnRow.transform, "DELETE", OptionsUIFactory.STYLE_DANGER, OnDeletePressed);

            OptionsUIFactory.CreateSectionHeader(transform, "Profile Info");

            AddInfoLabel("Per-profile: audio, input, calibration, gameplay");
            AddInfoLabel("Shared: display, resolution, V-Sync, FPS cap, render scale");
        }

        private void OnProfileSelected(int index)
        {
            var names = settingsManager.GetProfileNames();
            if (index >= 0 && index < names.Length)
            {
                settingsManager.SwitchProfile(names[index]);
            }
        }

        private void OnRenamePressed()
        {
            if (settingsManager.ActiveProfileName == "Default")
            {
                toastManager?.ShowWarning("Cannot rename Default profile");
                return;
            }

            toastManager?.ShowSuccess("Rename dialog coming soon");
        }

        private void OnNewPressed()
        {
            string newName = "Profile " + (settingsManager.GetProfileNames().Length + 1);
            settingsManager.CreateProfile(newName);
            RefreshDropdown();
            toastManager?.ShowSuccess($"Created profile: {newName}");
        }

        private void OnDeletePressed()
        {
            string profileName = settingsManager.ActiveProfileName;
            if (profileName == "Default")
            {
                toastManager?.ShowWarning("Cannot delete Default profile");
                return;
            }

            ConfirmDialog.Show(
                "Delete Profile",
                $"Delete \"{profileName}\"? This cannot be undone.",
                "DELETE",
                isDanger: true,
                onConfirm: () =>
                {
                    settingsManager.DeleteProfile(profileName);
                    RefreshDropdown();
                },
                onCancel: null
            );
        }

        private void RefreshDropdown()
        {
            if (profileDropdown == null) return;

            profileDropdown.ClearOptions();
            var names = settingsManager.GetProfileNames();
            foreach (var n in names)
            {
                profileDropdown.options.Add(new TMP_Dropdown.OptionData(n));
            }

            int idx = System.Array.IndexOf(names, settingsManager.ActiveProfileName);
            profileDropdown.value = Mathf.Max(0, idx);
            profileDropdown.RefreshShownValue();
        }

        private void AddInfoLabel(string text)
        {
            var go = new GameObject("InfoLabel", typeof(RectTransform));
            go.transform.SetParent(transform, false);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 16f;
            tmp.color = new Color(0.333f, 0.333f, 0.333f);
            tmp.alignment = TextAlignmentOptions.Left;

            go.AddComponent<LayoutElement>().minHeight = 24f;
        }
    }
}
