using System.Collections.Generic;
using UnityEngine;
using R8EOX.Track;

namespace R8EOX.Session.Internal
{
    internal class VehicleSpawner
    {
        private readonly List<GameObject> spawnedVehicles =
            new List<GameObject>();

        internal GameObject PlayerVehicle { get; private set; }

        internal GameObject SpawnPlayerVehicle(
            GameObject prefab,
            SpawnPointData spawnPoint)
        {
            if (prefab == null)
            {
                Debug.LogError(
                    "[VehicleSpawner] Cannot spawn player vehicle " +
                    "— prefab is null.");
                return null;
            }

            Vector3 safePos =
                SpawnSafety.GetSafeSpawnPosition(spawnPoint, prefab);
            GameObject vehicle =
                Object.Instantiate(prefab, safePos, spawnPoint.Rotation);
            vehicle.name = "PlayerVehicle";

            spawnedVehicles.Add(vehicle);
            PlayerVehicle = vehicle;

            Debug.Log(
                $"[VehicleSpawner] Spawned player vehicle at {safePos} " +
                $"(spawn index {spawnPoint.Index}).");
            return vehicle;
        }

        internal GameObject[] SpawnAIVehicles(
            GameObject prefab,
            SpawnPointData[] spawnPoints,
            int count)
        {
            if (prefab == null)
            {
                Debug.LogError(
                    "[VehicleSpawner] Cannot spawn AI vehicles " +
                    "— prefab is null.");
                return System.Array.Empty<GameObject>();
            }

            int toSpawn = Mathf.Min(count, spawnPoints.Length);
            var aiVehicles = new List<GameObject>(toSpawn);

            for (int i = 0; i < toSpawn; i++)
            {
                Vector3 safePos = SpawnSafety.GetSafeSpawnPosition(
                    spawnPoints[i], prefab);
                GameObject vehicle = Object.Instantiate(
                    prefab, safePos, spawnPoints[i].Rotation);
                vehicle.name = $"AIVehicle_{i}";

                spawnedVehicles.Add(vehicle);
                aiVehicles.Add(vehicle);

                Debug.Log(
                    $"[VehicleSpawner] Spawned AI vehicle {i} " +
                    $"at {safePos} " +
                    $"(spawn index {spawnPoints[i].Index}).");
            }

            return aiVehicles.ToArray();
        }

        internal void DestroyAllSpawned()
        {
            foreach (var vehicle in spawnedVehicles)
            {
                if (vehicle != null)
                    Object.Destroy(vehicle);
            }

            spawnedVehicles.Clear();
            PlayerVehicle = null;

            Debug.Log(
                "[VehicleSpawner] All spawned vehicles destroyed.");
        }

        internal int SpawnedCount => spawnedVehicles.Count;
    }
}
