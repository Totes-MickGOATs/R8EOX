using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewSessionChannel", menuName = "R8EOX/SessionChannel")]
    public class SessionChannel : ScriptableObject
    {
        [Header("Vehicle Selection")]
        [SerializeField] private VehicleRegistry vehicleRegistry;

        [Header("Track Selection")]
        [SerializeField] private TrackRegistry trackRegistry;

        [Header("Overlay Registry")]
        [SerializeField] private OverlayRegistry overlayRegistry;

        [System.NonSerialized] private SessionConfig activeConfig;

        public VehicleRegistry VehicleRegistry => vehicleRegistry;
        public TrackRegistry TrackRegistry => trackRegistry;
        public OverlayRegistry OverlayRegistry => overlayRegistry;

        public bool HasActiveSession => activeConfig != null;
        public SessionConfig ActiveConfig => activeConfig;

        public void SetSession(SessionConfig config)
        {
            activeConfig = config;
        }

        public void Clear()
        {
            activeConfig = null;
        }
    }
}
