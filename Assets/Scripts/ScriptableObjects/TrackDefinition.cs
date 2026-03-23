using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewTrackDefinition", menuName = "R8EOX/TrackDefinition")]
    public class TrackDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string displayName;
        [SerializeField] [TextArea] private string description;
        [SerializeField] private Sprite thumbnail;

        [Header("Scene")]
        [SerializeField] private string sceneName;

        [Header("Track Configuration")]
        [SerializeField] private TrackType trackType;
        [SerializeField] private SessionMode[] supportedModes;

        [Header("Availability")]
        [SerializeField] private bool isLocked;

        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Thumbnail => thumbnail;
        public string SceneName => sceneName;
        public TrackType TrackType => trackType;
        public SessionMode[] SupportedModes => supportedModes;
        public bool IsLocked => isLocked;

        public bool IsPlayable => !isLocked && !string.IsNullOrEmpty(sceneName);

        public bool SupportsMode(SessionMode mode)
        {
            if (supportedModes == null)
            {
                return false;
            }

            foreach (SessionMode supported in supportedModes)
            {
                if (supported == mode)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
