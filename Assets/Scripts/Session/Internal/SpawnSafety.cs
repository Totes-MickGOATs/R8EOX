using UnityEngine;
using R8EOX.Track;

namespace R8EOX.Session.Internal
{
    internal static class SpawnSafety
    {
        private const float k_ProbeHeight = 100f;
        private const float k_ClearanceBuffer = 0.15f;
        private const float k_FallbackY = 5f;

        internal static Vector3 GetSafeSpawnPosition(
            SpawnPointData spawnPoint,
            GameObject vehiclePrefab)
        {
            Vector3 spawnPos = spawnPoint.Position;
            float terrainY = GetTerrainHeight(spawnPos);

            if (float.IsNegativeInfinity(terrainY))
            {
                Debug.LogWarning(
                    $"[SpawnSafety] No terrain found below spawn index " +
                    $"{spawnPoint.Index} at {spawnPos}. " +
                    $"Using fallback Y={k_FallbackY}.");
                return new Vector3(spawnPos.x, k_FallbackY, spawnPos.z);
            }

            float lowestBound = GetLowestColliderBound(vehiclePrefab);
            float safeY = terrainY - lowestBound + k_ClearanceBuffer;

            return new Vector3(spawnPos.x, safeY, spawnPos.z);
        }

        private static float GetTerrainHeight(Vector3 position)
        {
            Vector3 origin = new Vector3(
                position.x,
                position.y + k_ProbeHeight,
                position.z);

            if (Physics.Raycast(
                    origin,
                    Vector3.down,
                    out RaycastHit hit,
                    k_ProbeHeight * 2f))
            {
                return hit.point.y;
            }

            return float.NegativeInfinity;
        }

        private static float GetLowestColliderBound(GameObject prefab)
        {
            if (prefab == null)
                return 0f;

            var colliders = prefab.GetComponentsInChildren<Collider>();
            if (colliders == null || colliders.Length == 0)
                return 0f;

            float lowestY = 0f;
            foreach (var col in colliders)
            {
                float localMinY =
                    col.bounds.min.y - prefab.transform.position.y;
                if (localMinY < lowestY)
                    lowestY = localMinY;
            }

            return lowestY;
        }
    }
}
