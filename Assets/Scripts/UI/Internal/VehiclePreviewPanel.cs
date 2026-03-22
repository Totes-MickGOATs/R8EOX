using TMPro;
using UnityEngine;

namespace R8EOX.UI.Internal
{
    internal class VehiclePreviewPanel : MonoBehaviour
    {
        [SerializeField] private VehiclePreviewRenderer previewRenderer;
        [SerializeField] private VehicleStatsDisplay statsDisplay;
        [SerializeField] private TextMeshProUGUI vehicleNameText;

        internal void Initialize()
        {
            if (previewRenderer != null)
            {
                previewRenderer.Initialize();
            }
        }

        internal void UpdatePreview(VehicleDefinition definition)
        {
            if (definition == null)
            {
                return;
            }

            if (vehicleNameText != null)
            {
                vehicleNameText.text = definition.DisplayName;
            }

            if (previewRenderer != null)
            {
                previewRenderer.SetVehicle(definition.VehiclePrefab);
            }

            if (statsDisplay != null)
            {
                statsDisplay.UpdateStats(definition);
            }
        }

        internal void Teardown()
        {
            if (previewRenderer != null)
            {
                previewRenderer.Teardown();
            }
        }
    }
}
