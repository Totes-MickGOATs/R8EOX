using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewRaceConfig", menuName = "R8EOX/RaceConfig")]
    public class RaceConfig : ScriptableObject
    {
        [Header("Race Rules")]
        [SerializeField] private int totalLaps = 3;
        [SerializeField] private int maxPlayers = 8;
        [SerializeField] private float countdownDuration = 3f;

        [Header("Timing")]
        [SerializeField] private float maxRaceTime = 600f;
        [SerializeField] private bool enableBestLapTracking = true;

        [Header("AI")]
        [SerializeField] private int aiOpponentCount = 7;
        [SerializeField] private int aiDifficultyLevel = 5;

        [Header("Respawn")]
        [SerializeField] private float respawnDelay = 2f;
        [SerializeField] private bool autoRespawnOnFlip = true;

        public int TotalLaps => totalLaps;
        public int MaxPlayers => maxPlayers;
        public float CountdownDuration => countdownDuration;
        public float MaxRaceTime => maxRaceTime;
        public bool EnableBestLapTracking => enableBestLapTracking;
        public int AIOpponentCount => aiOpponentCount;
        public int AIDifficultyLevel => aiDifficultyLevel;
        public float RespawnDelay => respawnDelay;
        public bool AutoRespawnOnFlip => autoRespawnOnFlip;
    }
}
