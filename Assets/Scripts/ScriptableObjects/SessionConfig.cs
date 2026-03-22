using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewSessionConfig", menuName = "R8EOX/SessionConfig")]
    public class SessionConfig : ScriptableObject
    {
        [Header("Session")]
        [SerializeField] private SessionMode sessionMode = SessionMode.Practice;
        [SerializeField] private string trackScenePath;
        [SerializeField] private GameObject vehiclePrefab;

        [Header("Race Rules")]
        [SerializeField] private int totalLaps;
        [SerializeField] private float countdownDuration = 3f;
        [SerializeField] private float maxRaceTime = 600f;

        [Header("AI")]
        [SerializeField] private int aiOpponentCount;
        [SerializeField] [Range(1, 10)] private int aiDifficultyLevel = 5;

        public SessionMode SessionMode => sessionMode;
        public string TrackScenePath => trackScenePath;
        public GameObject VehiclePrefab => vehiclePrefab;
        public int TotalLaps => totalLaps;
        public float CountdownDuration => countdownDuration;
        public float MaxRaceTime => maxRaceTime;
        public int AiOpponentCount => aiOpponentCount;
        public int AiDifficultyLevel => aiDifficultyLevel;
    }
}
