namespace R8EOX.Vehicle.Internal
{
    internal static class SteeringStage
    {
        public static void Execute(
            ref VehicleFrame frame,
            float prevSteering,
            float steeringMax, float steeringSpeed,
            float steeringSpeedLimit, float steeringHighSpeedFactor)
        {
            if (frame.IsAirborne)
                return;

            frame.CurrentSteering = SteeringRamp.Step(
                prevSteering, frame.Dt,
                frame.SteerInput, frame.ForwardSpeed,
                steeringMax, steeringSpeed,
                steeringSpeedLimit, steeringHighSpeedFactor);
        }
    }
}
