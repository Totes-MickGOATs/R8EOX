using UnityEngine;

namespace R8EOX.Vehicle
{
    public class VehicleManager : MonoBehaviour
    {
        [SerializeField] private VehicleConfig config;

        public void Initialize(VehicleConfig vehicleConfig)
        {
            // TODO: Initialize vehicle with config
        }

        public void Tick(float deltaTime)
        {
            // TODO: Update all vehicle sub-components
        }

        public void ApplyInput(float throttle, float brake, float steering)
        {
            // TODO: Route input to motor and wheels
        }

        public float GetSpeed()
        {
            // TODO: Return current speed
            return 0f;
        }

        public int GetCurrentGear()
        {
            // TODO: Return current gear from transmission
            return 1;
        }

        public float GetRPM()
        {
            // TODO: Return engine RPM from motor
            return 0f;
        }
    }
}
