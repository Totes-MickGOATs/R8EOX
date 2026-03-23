using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewOverlayRegistry", menuName = "R8EOX/OverlayRegistry")]
    public class OverlayRegistry : ScriptableObject
    {
        [SerializeField] private GameObject optionsOverlayPrefab;
        [SerializeField] private GameObject vehicleSelectOverlayPrefab;
        [SerializeField] private GameObject confirmDialogPrefab;

        public GameObject OptionsOverlayPrefab => optionsOverlayPrefab;
        public GameObject VehicleSelectOverlayPrefab => vehicleSelectOverlayPrefab;
        public GameObject ConfirmDialogPrefab => confirmDialogPrefab;
    }
}
