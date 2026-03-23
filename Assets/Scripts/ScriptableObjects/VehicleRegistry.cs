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

            var valid = new List<VehicleDefinition>();
            for (int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i] != null)
                    valid.Add(vehicles[i]);
                else
                    Debug.LogWarning($"[VehicleRegistry] Null entry at index {i} — skipped.");
            }
            return valid.ToArray();
        }

        public VehicleDefinition GetDefault()
        {
            if (vehicles == null || vehicles.Length == 0) return null;
            for (int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i] != null) return vehicles[i];
            }
            return null;
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
