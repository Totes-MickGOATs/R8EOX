using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    internal class Transmission
    {
        private int currentGear;
        private float[] gearRatios;
        private float finalDriveRatio;

        internal void Initialize(float[] ratios, float finalDrive)
        {
            gearRatios = ratios;
            finalDriveRatio = finalDrive;
            currentGear = 1;
        }

        internal void ShiftUp()
        {
            // TODO: Shift to next gear if available
        }

        internal void ShiftDown()
        {
            // TODO: Shift to previous gear if available
        }

        internal float GetCurrentGearRatio()
        {
            if (gearRatios == null || currentGear < 0 || currentGear >= gearRatios.Length)
            {
                return 1f;
            }

            return gearRatios[currentGear] * finalDriveRatio;
        }

        internal int GetCurrentGear()
        {
            return currentGear;
        }
    }
}
