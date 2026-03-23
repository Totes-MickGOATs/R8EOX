using TMPro;
using UnityEngine;

namespace R8EOX.UI.Internal
{
    internal class RaceHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI positionText;
        [SerializeField] private TextMeshProUGUI lapText;
        [SerializeField] private TextMeshProUGUI raceTimeText;
        [SerializeField] private TextMeshProUGUI lapTimeText;
        [SerializeField] private TextMeshProUGUI bestLapTimeText;
        [SerializeField] private TextMeshProUGUI countdownText;

        internal void Show() => gameObject.SetActive(true);

        internal void Hide() => gameObject.SetActive(false);

        internal void UpdateSpeed(float speedKmh)
        {
            if (speedText != null)
                speedText.text = $"{Mathf.RoundToInt(speedKmh)} km/h";
        }

        internal void UpdatePosition(int position)
        {
            if (positionText != null)
                positionText.text = GetOrdinal(position);
        }

        internal void UpdateLap(int currentLap, int totalLaps)
        {
            if (lapText != null)
                lapText.text = $"Lap {currentLap}/{totalLaps}";
        }

        internal void UpdateRaceTime(float seconds)
        {
            if (raceTimeText != null)
                raceTimeText.text = FormatTime(seconds);
        }

        internal void UpdateLapTime(float seconds)
        {
            if (lapTimeText != null)
                lapTimeText.text = FormatTime(seconds);
        }

        internal void UpdateBestLapTime(float seconds)
        {
            if (bestLapTimeText != null)
            {
                bestLapTimeText.text = seconds > 0f
                    ? FormatTime(seconds)
                    : "--:--.---";
            }
        }

        internal void ShowCountdown(float seconds)
        {
            if (countdownText == null) return;
            countdownText.gameObject.SetActive(true);

            int whole = Mathf.CeilToInt(seconds);
            countdownText.text = whole > 0 ? whole.ToString() : "GO!";
        }

        internal void HideCountdown()
        {
            if (countdownText != null)
                countdownText.gameObject.SetActive(false);
        }

        private static string FormatTime(float seconds)
        {
            if (seconds <= 0f) return "0:00.000";
            int minutes = Mathf.FloorToInt(seconds / 60f);
            float remainder = seconds - minutes * 60f;
            int secs = Mathf.FloorToInt(remainder);
            int millis = Mathf.FloorToInt((remainder - secs) * 1000f);
            return $"{minutes}:{secs:D2}.{millis:D3}";
        }

        private static string GetOrdinal(int position)
        {
            if (position <= 0) return "--";
            int lastTwo = position % 100;
            int lastOne = position % 10;

            if (lastTwo >= 11 && lastTwo <= 13)
                return $"{position}th";

            return lastOne switch
            {
                1 => $"{position}st",
                2 => $"{position}nd",
                3 => $"{position}rd",
                _ => $"{position}th"
            };
        }
    }
}
