namespace R8EOX.Vehicle.Internal
{
    internal static class DrivetrainStage
    {
        public static void Execute(
            ref VehicleFrame frame,
            Drivetrain drivetrain,
            WheelManager wheels)
        {
            if (frame.IsAirborne || drivetrain == null)
                return;

            drivetrain.Distribute(frame.EngineForce, wheels.Front, wheels.Rear);
        }
    }
}
