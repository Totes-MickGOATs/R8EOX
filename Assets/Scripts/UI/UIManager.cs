using UnityEngine;

namespace R8EOX.UI
{
    public class UIManager : MonoBehaviour
    {
        public void UpdateHUD(float speed, int position, int lap, int totalLaps)
        {
            // TODO: Route data to RaceHUD
        }

        public void UpdateLeaderboard(string[] standings)
        {
            // TODO: Route data to Leaderboard
        }

        public void ShowPauseMenu()
        {
            // TODO: Activate pause menu
        }

        public void HidePauseMenu()
        {
            // TODO: Deactivate pause menu
        }

        public void ShowCountdown(int seconds)
        {
            // TODO: Display countdown overlay
        }

        public void ShowRaceResults()
        {
            // TODO: Display final results screen
        }
    }
}
