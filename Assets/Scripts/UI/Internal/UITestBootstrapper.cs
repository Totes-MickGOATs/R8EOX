using UnityEngine;
using R8EOX.UI.Internal;

namespace R8EOX.UI.Internal
{
    internal class UITestBootstrapper : MonoBehaviour
    {
        [SerializeField] private VehicleRegistry registry;

        private VehicleSelectOverlay activeOverlay;

        private void Start()
        {
            if (registry == null)
            {
                Debug.LogError("[UITestBootstrapper] VehicleRegistry not assigned.");
                return;
            }

            if (registry.OverlayPrefab == null)
            {
                Debug.LogError("[UITestBootstrapper] OverlayPrefab not set on registry.");
                return;
            }

            var go = Instantiate(registry.OverlayPrefab);
            activeOverlay = go.GetComponent<VehicleSelectOverlay>();
            if (activeOverlay == null)
            {
                Debug.LogError("[UITestBootstrapper] No VehicleSelectOverlay on prefab.");
                return;
            }

            activeOverlay.Show(registry, OnConfirm, OnBack);
            Debug.Log("[UITestBootstrapper] Overlay shown with " + registry.Count + " vehicles.");
        }

        private void OnConfirm(VehicleDefinition definition)
        {
            Debug.Log("[UITestBootstrapper] Confirmed: " + definition.DisplayName);
            activeOverlay.Hide();
            activeOverlay.Show(registry, OnConfirm, OnBack);
        }

        private void OnBack()
        {
            Debug.Log("[UITestBootstrapper] Back pressed — re-showing overlay.");
            activeOverlay.Hide();
            activeOverlay.Show(registry, OnConfirm, OnBack);
        }
    }
}
