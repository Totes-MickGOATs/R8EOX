using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    /// <summary>
    /// Mutable per-frame context for the vehicle physics pipeline.
    /// Created fresh each FixedUpdate, passed by ref through all stages.
    /// </summary>
    internal struct VehicleFrame
    {
        // ---- Time ----
        public float Dt;

        // ---- Input (written by InputStage) ----
        public float ThrottleRaw;
        public float BrakeInput;
        public float SteerInput;
        public float SmoothThrottle;
        public float ForwardSpeed;

        // ---- Airborne (written by AirborneStage) ----
        public bool IsAirborne;
        public float TiltAngle;
        public float TumbleFactor;

        // ---- Drive (written by GroundDriveStage) ----
        public float EngineForce;
        public float BrakeForce;
        public float CoastDragForce;
        public bool ReverseEngaged;

        // ---- Steering (written by SteeringStage) ----
        public float CurrentSteering;
    }
}
