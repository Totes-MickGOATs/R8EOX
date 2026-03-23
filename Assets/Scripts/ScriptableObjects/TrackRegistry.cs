using System.Collections.Generic;
using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewTrackRegistry", menuName = "R8EOX/TrackRegistry")]
    public class TrackRegistry : ScriptableObject
    {
        [SerializeField] private TrackDefinition[] tracks;

        public int Count => CountNonNull();

        private int CountNonNull()
        {
            if (tracks == null)
            {
                return 0;
            }

            int count = 0;
            foreach (TrackDefinition track in tracks)
            {
                if (track != null)
                {
                    count++;
                }
            }

            return count;
        }

        public TrackDefinition[] GetAll()
        {
            if (tracks == null)
            {
                return System.Array.Empty<TrackDefinition>();
            }

            var result = new List<TrackDefinition>(tracks.Length);
            foreach (TrackDefinition track in tracks)
            {
                if (track != null)
                {
                    result.Add(track);
                }
            }

            return result.ToArray();
        }

        public TrackDefinition GetDefault()
        {
            if (tracks == null)
            {
                return null;
            }

            foreach (TrackDefinition track in tracks)
            {
                if (track != null)
                {
                    return track;
                }
            }

            return null;
        }

        public TrackDefinition FindBySceneName(string sceneName)
        {
            if (tracks == null || string.IsNullOrEmpty(sceneName))
            {
                return null;
            }

            foreach (TrackDefinition track in tracks)
            {
                if (track != null && track.SceneName == sceneName)
                {
                    return track;
                }
            }

            return null;
        }
    }
}
