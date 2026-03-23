namespace R8EOX.Vehicle.Internal
{
    internal static class AirPhysicsStage
    {
        public static void Execute(
            ref VehicleFrame frame,
            RCAirPhysics airPhysics)
        {
            if (!frame.IsAirborne || airPhysics == null)
                return;

            airPhysics.Apply(frame.Dt, frame.SmoothThrottle,
                frame.BrakeInput, frame.SteerInput);
        }
    }
}
