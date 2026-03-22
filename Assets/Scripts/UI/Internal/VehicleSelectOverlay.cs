using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.UI.Internal
{
    internal class VehicleSelectOverlay : MonoBehaviour
    {
        [SerializeField] private VehiclePreviewPanel previewPanel;
        [SerializeField] private VehicleListPanel listPanel;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI titleText;

        private const string LastSelectedKey = "R8EOX_LastSelectedVehicle";

        private Action<VehicleDefinition> onConfirmed;
        private Action onCancelled;
        private VehicleDefinition currentSelection;

        internal void Show(
            VehicleRegistry registry,
            Action<VehicleDefinition> confirmCallback,
            Action cancelCallback = null)
        {
            onConfirmed = confirmCallback;
            onCancelled = cancelCallback;

            if (titleText != null)
            {
                titleText.text = "SELECT VEHICLE";
            }

            previewPanel.Initialize();
            listPanel.Initialize(registry.GetAll(), OnSelectionChanged);

            confirmButton.onClick.AddListener(OnConfirmPressed);

            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackPressed);
            }

            RestoreLastSelection();
            gameObject.SetActive(true);
        }

        internal void Hide()
        {
            listPanel.Teardown();
            previewPanel.Teardown();

            confirmButton.onClick.RemoveAllListeners();

            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
            }

            gameObject.SetActive(false);
        }

        private void OnSelectionChanged(VehicleDefinition definition)
        {
            currentSelection = definition;
            previewPanel.UpdatePreview(definition);

            bool canConfirm = definition != null && definition.VehiclePrefab != null;
            confirmButton.interactable = canConfirm;
        }

        private void OnConfirmPressed()
        {
            if (currentSelection == null)
            {
                return;
            }

            PlayerPrefs.SetInt(LastSelectedKey, listPanel.SelectedIndex);
            PlayerPrefs.Save();

            onConfirmed?.Invoke(currentSelection);
        }

        private void OnBackPressed()
        {
            onCancelled?.Invoke();
        }

        private void RestoreLastSelection()
        {
            if (PlayerPrefs.HasKey(LastSelectedKey))
            {
                int lastIndex = PlayerPrefs.GetInt(LastSelectedKey, 0);
                listPanel.SelectIndex(lastIndex);
            }
            else
            {
                listPanel.SelectIndex(0);
            }
        }
    }
}
