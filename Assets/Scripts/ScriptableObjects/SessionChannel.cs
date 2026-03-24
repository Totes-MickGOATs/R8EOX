using UnityEngine;
using R8EOX.Session;

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
        [System.NonSerialized] private SessionManager activeManager;

        public VehicleRegistry VehicleRegistry => vehicleRegistry;
        public TrackRegistry TrackRegistry => trackRegistry;
        public OverlayRegistry OverlayRegistry => overlayRegistry;

        public bool HasActiveSession => activeConfig != null;
        public SessionConfig ActiveConfig => activeConfig;

        /// <summary>
        /// The SessionManager created by AppManager for the current session.
        /// Set before the track scene loads so SessionBootstrapper can reach it
        /// without any scene-wide search.
        /// </summary>
        public SessionManager ActiveManager => activeManager;

        public void SetSession(SessionConfig config)
        {
            activeConfig = config;
        }

        /// <summary>Registers the active SessionManager. Called by AppManager.</summary>
        public void SetManager(SessionManager manager)
        {
            activeManager = manager;
        }

        public void Clear()
        {
            activeConfig = null;
            activeManager = null;
        }
    }
}
