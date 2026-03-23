namespace R8EOX.Vehicle.Internal
{
    /// <summary>Immutable motor tuning values for a single preset.</summary>
    internal struct MotorData
    {
        public readonly float EngineForceMax;
        public readonly float BrakeForce;
        public readonly float ReverseForce;
        public readonly float CoastDrag;
        public readonly float MaxSpeed;
        public readonly float ThrottleRampUp;

        public MotorData(float engine, float brake, float reverse, float coast, float max, float ramp)
        {
            EngineForceMax = engine;
            BrakeForce     = brake;
            ReverseForce   = reverse;
            CoastDrag      = coast;
            MaxSpeed       = max;
            ThrottleRampUp = ramp;
        }
    }
}
