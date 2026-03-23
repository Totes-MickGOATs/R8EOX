using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    internal static class AirborneStage
    {
        public static void Execute(
            ref VehicleFrame frame,
            WheelManager wheels,
            AirborneDetector airDetect,
            TumbleController tumble,
            Transform vehicleTransform,
            float tumbleEngageDeg, float tumbleFullDeg, float tumbleHysteresisDeg)
        {
            frame.IsAirborne = airDetect.Update(!wheels.AnyOnGround());

            tumble.Update(vehicleTransform, frame.IsAirborne,
                tumbleEngageDeg, tumbleFullDeg, tumbleHysteresisDeg);

            frame.TumbleFactor = tumble.TumbleFactor;
            frame.TiltAngle = tumble.TiltAngle;
        }
    }
}
