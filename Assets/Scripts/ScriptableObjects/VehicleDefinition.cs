using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewVehicleDefinition", menuName = "R8EOX/VehicleDefinition")]
    public class VehicleDefinition : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] [TextArea] private string description;
        [SerializeField] private Sprite thumbnail;
        [SerializeField] private GameObject vehiclePrefab;
        [SerializeField] private VehicleCategory category;
        [SerializeField] private VehicleStats stats;
        [SerializeField] private bool isPlayable = true;

        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Thumbnail => thumbnail;
        public GameObject VehiclePrefab => vehiclePrefab;
        public VehicleCategory Category => category;
        public VehicleStats Stats => stats;
        public bool IsPlayable => isPlayable;
    }
}
