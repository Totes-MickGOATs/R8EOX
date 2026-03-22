using UnityEngine;

namespace R8EOX.Track
{
    public struct SpawnPointData
    {
        public int Index { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public bool IsPlayerSpawn { get; set; }
    }
}
