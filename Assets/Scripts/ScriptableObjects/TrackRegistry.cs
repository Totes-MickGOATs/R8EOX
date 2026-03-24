using System.Collections.Generic;
using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewTrackRegistry", menuName = "R8EOX/TrackRegistry")]
    public class TrackRegistry : ScriptableObject
    {
        [SerializeField] private TrackDefinition[] tracks;

        private TrackDefinition[] _cachedAll;
        private bool _cacheValid;

        public int Count { get { EnsureCache(); return _cachedAll.Length; } }

        private void OnEnable() => _cacheValid = false;

        private void OnValidate() => _cacheValid = false;

        private void EnsureCache()
        {
            if (_cacheValid) return;
            RebuildCache();
        }

        private void RebuildCache()
        {
            if (tracks == null)
            {
                _cachedAll = System.Array.Empty<TrackDefinition>();
                _cacheValid = true;
                return;
            }

            var all = new List<TrackDefinition>(tracks.Length);
            for (int i = 0; i < tracks.Length; i++)
            {
                if (tracks[i] != null) all.Add(tracks[i]);
            }

            _cachedAll = all.ToArray();
            _cacheValid = true;
        }

        public TrackDefinition[] GetAll()
        {
            EnsureCache();
            return _cachedAll;
        }

        public TrackDefinition GetDefault()
        {
            EnsureCache();
            return _cachedAll.Length > 0 ? _cachedAll[0] : null;
        }

        public TrackDefinition FindBySceneName(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return null;

            EnsureCache();
            for (int i = 0; i < _cachedAll.Length; i++)
            {
                if (_cachedAll[i].SceneName == sceneName)
                    return _cachedAll[i];
            }

            return null;
        }
    }
}
