using UnityEngine;

namespace R8EOX.UI.Internal
{
    internal class Leaderboard : MonoBehaviour
    {
        internal void UpdateStandings(string[] standings)
        {
            // TODO: Populate leaderboard entries
        }

        internal void Show()
        {
            gameObject.SetActive(true);
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
