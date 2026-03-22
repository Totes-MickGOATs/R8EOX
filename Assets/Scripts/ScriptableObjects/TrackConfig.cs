using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewTrackConfig", menuName = "R8EOX/TrackConfig")]
    public class TrackConfig : ScriptableObject
    {
        [Header("Track Info")]
        [SerializeField] private string trackName = "New Track";
        [SerializeField] private float trackLength = 5000f;
        [SerializeField] private int checkpointCount = 10;

        [Header("Surface")]
        [SerializeField] private float defaultGripMultiplier = 1.0f;

        [Header("Boundaries")]
        [SerializeField] private float boundaryBounceForce = 5f;
        [SerializeField] private float outOfBoundsRespawnDelay = 2f;

        [Header("Environment")]
        [SerializeField] private float ambientTemperature = 25f;

        public string TrackName => trackName;
        public float TrackLength => trackLength;
        public int CheckpointCount => checkpointCount;
        public float DefaultGripMultiplier => defaultGripMultiplier;
        public float BoundaryBounceForce => boundaryBounceForce;
        public float OutOfBoundsRespawnDelay => outOfBoundsRespawnDelay;
        public float AmbientTemperature => ambientTemperature;
    }
}
