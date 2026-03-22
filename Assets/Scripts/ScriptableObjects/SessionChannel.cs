using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewSessionChannel", menuName = "R8EOX/SessionChannel")]
    public class SessionChannel : ScriptableObject
    {
        [System.NonSerialized] private SessionConfig activeConfig;

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
