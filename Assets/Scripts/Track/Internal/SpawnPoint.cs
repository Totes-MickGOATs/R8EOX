using UnityEngine;

namespace R8EOX.Track.Internal
{
    internal class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private int spawnIndex;
        [SerializeField] private bool isPlayerSpawn;

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
            Gizmos.color = isPlayerSpawn ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }
    }
}
