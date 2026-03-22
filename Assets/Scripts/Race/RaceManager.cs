using UnityEngine;

namespace R8EOX.Race
{
    public class RaceManager : MonoBehaviour
    {
        [SerializeField] private RaceConfig config;

        public void StartRace()
        {
            // TODO: Initialize race state, reset standings, begin countdown
        }

        public void EndRace()
        {
            // TODO: Finalize standings, stop timer
        }

        public void Tick(float deltaTime)
        {
            // TODO: Update race state, check for race completion
        }

        public void OnCheckpointPassed(int checkpointIndex, GameObject vehicle)
        {
            // TODO: Update standings, check for lap completion
        }

        public int GetVehiclePosition(GameObject vehicle)
        {
            // TODO: Return race position for given vehicle
            return 0;
        }

        public float GetRaceTime()
        {
            // TODO: Return elapsed race time
            return 0f;
        }
    }
}
