using System.Collections.Generic;
using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewVehicleRegistry", menuName = "R8EOX/VehicleRegistry")]
    public class VehicleRegistry : ScriptableObject
    {
        [SerializeField] private VehicleDefinition[] vehicles;
        [SerializeField] private GameObject overlayPrefab;

        public GameObject OverlayPrefab => overlayPrefab;
        public int Count => vehicles?.Length ?? 0;

        public VehicleDefinition[] GetAll()
        {
            if (vehicles == null || vehicles.Length == 0)
                return System.Array.Empty<VehicleDefinition>();

            var copy = new VehicleDefinition[vehicles.Length];
            System.Array.Copy(vehicles, copy, vehicles.Length);
            return copy;
        }

        public VehicleDefinition GetDefault()
        {
            return vehicles != null && vehicles.Length > 0 ? vehicles[0] : null;
        }

        public VehicleDefinition[] GetByCategory(VehicleCategory category)
        {
            if (vehicles == null || vehicles.Length == 0)
                return System.Array.Empty<VehicleDefinition>();

            var results = new List<VehicleDefinition>();
            for (int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i] != null && vehicles[i].Category == category)
                    results.Add(vehicles[i]);
            }

            return results.ToArray();
        }
    }
}
