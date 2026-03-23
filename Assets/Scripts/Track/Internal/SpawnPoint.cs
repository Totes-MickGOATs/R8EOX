using UnityEngine;

namespace R8EOX.Track.Internal
{
    internal class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private int spawnIndex;
        [SerializeField] private bool isPlayerSpawn;

        private const float k_WarnThreshold = 0.5f;
        private const float k_ErrorThreshold = 2.0f;

        internal int Index => spawnIndex;
        internal bool IsPlayerSpawn => isPlayerSpawn;
        internal Vector3 Position => transform.position;
        internal Quaternion Rotation => transform.rotation;

        internal SpawnPointData ToData()
        {
            return new SpawnPointData
            {
                Index = spawnIndex,
                Position = transform.position,
                Rotation = transform.rotation,
                IsPlayerSpawn = isPlayerSpawn
            };
        }

        private void OnDrawGizmos()
        {
            Color gizmoColor = isPlayerSpawn ? Color.green : Color.yellow;
            float terrainY = 0f;
            float delta = 0f;

            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null)
            {
                terrainY = terrain.SampleHeight(transform.position)
                    + terrain.transform.position.y;
                delta = terrainY - transform.position.y;
                if (delta > k_ErrorThreshold)
                    gizmoColor = Color.red;
                else if (delta > k_WarnThreshold)
                    gizmoColor = new Color(1f, 0.5f, 0f);
            }

            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.DrawRay(transform.position, transform.forward * 2f);

            if (delta > k_WarnThreshold)
            {
                Vector3 terrainPos = new Vector3(
                    transform.position.x, terrainY, transform.position.z);
                Gizmos.DrawLine(transform.position, terrainPos);
            }
        }
    }
}
