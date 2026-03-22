namespace R8EOX.Vehicle
{
    public readonly struct VehicleTelemetry
    {
        public readonly float ForwardSpeed;         // m/s in vehicle forward direction
        public readonly float SpeedKmh;             // Convenience: magnitude * 3.6
        public readonly float EngineRpm;            // Derived from wheel RPM * gear ratio
        public readonly float ThrottleAmount;       // 0-1 smoothed throttle
        public readonly float BrakeAmount;          // 0-1 brake input
        public readonly float SteeringAngle;        // Radians of current steering
        public readonly bool IsAirborne;
        public readonly float TumbleFactor;         // 0-1 tumble blend
        public readonly float TiltAngle;            // Degrees from upright
        public readonly int WheelsOnGround;         // Count of grounded wheels
        public readonly float LastCollisionImpulse; // Last frame's collision magnitude in N
        public readonly WheelTelemetry[] Wheels;    // Per-wheel data array

        public VehicleTelemetry(
            float forwardSpeed, float speedKmh, float engineRpm,
            float throttleAmount, float brakeAmount, float steeringAngle,
            bool isAirborne, float tumbleFactor, float tiltAngle,
            int wheelsOnGround, float lastCollisionImpulse,
            WheelTelemetry[] wheels)
        {
            ForwardSpeed = forwardSpeed; SpeedKmh = speedKmh; EngineRpm = engineRpm;
            ThrottleAmount = throttleAmount; BrakeAmount = brakeAmount; SteeringAngle = steeringAngle;
            IsAirborne = isAirborne; TumbleFactor = tumbleFactor; TiltAngle = tiltAngle;
            WheelsOnGround = wheelsOnGround; LastCollisionImpulse = lastCollisionImpulse;
            Wheels = wheels;
        }
    }
}
