using System.Collections.Generic;
using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewVehicleRegistry", menuName = "R8EOX/VehicleRegistry")]
    public class VehicleRegistry : ScriptableObject
    {
        [SerializeField] private VehicleDefinition[] vehicles;

        private VehicleDefinition[] _cachedAll;
        private VehicleDefinition[] _cachedPlayable;
        private bool _cacheValid;

        public int Count { get { EnsureCache(); return _cachedAll.Length; } }

        private void OnEnable() => _cacheValid = false;

        private void OnValidate() => _cacheValid = false;

        private void EnsureCache()
        {
            if (_cacheValid) return;
            RebuildCache();
        }

        private void RebuildCache()
        {
            if (vehicles == null)
            {
                _cachedAll = System.Array.Empty<VehicleDefinition>();
                _cachedPlayable = System.Array.Empty<VehicleDefinition>();
                _cacheValid = true;
                return;
            }

            var all = new List<VehicleDefinition>(vehicles.Length);
            var playable = new List<VehicleDefinition>(vehicles.Length);
            for (int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i] == null)
                {
                    Debug.LogWarning($"[VehicleRegistry] Null entry at index {i} — skipped.");
                    continue;
                }
                all.Add(vehicles[i]);
                if (vehicles[i].IsPlayable) playable.Add(vehicles[i]);
            }

            _cachedAll = all.ToArray();
            _cachedPlayable = playable.ToArray();
            _cacheValid = true;
        }

        public VehicleDefinition[] GetAll()
        {
            EnsureCache();
            return _cachedAll;
        }

        public VehicleDefinition GetDefault()
        {
            EnsureCache();
            return _cachedAll.Length > 0 ? _cachedAll[0] : null;
        }

        public VehicleDefinition[] GetPlayable()
        {
            EnsureCache();
            return _cachedPlayable;
        }

        public VehicleDefinition[] GetByCategory(VehicleCategory category)
        {
            EnsureCache();
            if (_cachedAll.Length == 0)
                return System.Array.Empty<VehicleDefinition>();

            var results = new List<VehicleDefinition>();
            for (int i = 0; i < _cachedAll.Length; i++)
            {
                if (_cachedAll[i].Category == category)
                    results.Add(_cachedAll[i]);
            }

            return results.ToArray();
        }
    }
}
