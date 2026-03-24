using UnityEngine;
using R8EOX.Vehicle.Internal;

namespace R8EOX.Vehicle
{
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleManager : MonoBehaviour
    {
        const float k_DefaultMass = 1.5f;
        const float k_FlipHeightOffset = 0.35f, k_MsToKmh = 3.6f;

        [Header("Motor")]
        [Tooltip("Motor preset selecting predefined engine parameters")]
        [SerializeField] private MotorPreset _motorPreset = MotorPreset.Motor17_5T;

        [Header("Engine")]
        [Tooltip("Maximum engine force applied at the wheels (Newtons)")]
        [SerializeField] private float _engineForceMax = 18.0f;
        [Tooltip("Maximum forward speed (m/s)")]
        [SerializeField] private float _maxSpeed = 20f;
        [Tooltip("Braking force applied when brake input is active (Newtons)")]
        [SerializeField] private float _brakeForce = 15.3f;
        [Tooltip("Reverse drive force (Newtons)")]
        [SerializeField] private float _reverseForce = 9.9f;
        [Tooltip("Coast drag force applied when no throttle is given (Newtons)")]
        [SerializeField] private float _coastDrag = 2.5f;

        [Header("Throttle")]
        [Tooltip("Rate at which throttle ramps up (units/sec)")]
        [SerializeField] private float _throttleRampUp = 4.0f;
        [Tooltip("Rate at which throttle ramps down when released (units/sec)")]
        [SerializeField] private float _throttleRampDown = 10f;

        [Header("Steering")]
        [Tooltip("Maximum steering angle in radians")]
        [SerializeField] private float _steeringMax = 0.50f;
        [Tooltip("Steering interpolation speed (units/sec)")]
        [SerializeField] private float _steeringSpeed = 7f;
        [Tooltip("Speed threshold above which high-speed steering factor activates (m/s)")]
        [SerializeField] private float _steeringSpeedLimit = 8f;
        [Range(0f, 1f)]
        [Tooltip("Steering angle multiplier applied at high speed (0=no steering, 1=full steering)")]
        [SerializeField] private float _steeringHighSpeedFactor = 0.4f;

        [Header("Suspension Front")]
        [Tooltip("Front axle spring strength (N/m)")]
        [SerializeField] private float _frontSpringStrength = 700.0f;
        [Tooltip("Front axle spring damping coefficient (N·s/m)")]
        [SerializeField] private float _frontSpringDamping = 13.0f;

        [Header("Suspension Rear")]
        [Tooltip("Rear axle spring strength (N/m)")]
        [SerializeField] private float _rearSpringStrength = 350.0f;
        [Tooltip("Rear axle spring damping coefficient (N·s/m)")]
        [SerializeField] private float _rearSpringDamping = 9.2f;

        [Header("Traction")]
        [Range(0f, 1f)]
        [Tooltip("Grip coefficient multiplier applied to all wheels (0=no grip, 1=full grip)")]
        [SerializeField] private float _gripCoeff = 0.7f;

        [Header("Drivetrain")]
        [Tooltip("Final drive gear ratio (motor RPM to wheel RPM)")]
        [SerializeField] private float _gearRatio = 7.5f;

        [Header("CoM")]
        [Tooltip("Centre of mass offset from the Rigidbody origin (world-space Y is most critical)")]
        [SerializeField] private Vector3 _comGround = new Vector3(0f, -0.05f, 0f);

        [Header("Crash")]
        [Tooltip("Tilt angle (degrees) at which tumble mode begins to engage")]
        [SerializeField] private float _tumbleEngageDeg = 50f;
        [Tooltip("Tilt angle (degrees) at which tumble mode is fully active")]
        [SerializeField] private float _tumbleFullDeg = 70f;
        [Tooltip("Hysteresis band (degrees) to prevent tumble state oscillation")]
        [SerializeField] private float _tumbleHysteresisDeg = 5f;

        // State
        public float CurrentEngineForce { get; private set; } public float CurrentBrakeForce { get; private set; }
        public float SmoothThrottle { get; private set; }     public float CurrentSteering   { get; private set; }
        public bool  IsAirborne     { get; private set; }     public float TumbleFactor       { get; private set; }
        public float TiltAngle      { get; private set; }     public bool  ReverseEngaged     { get; private set; }
        public float ForwardSpeed   { get; private set; }     internal MotorPreset ActiveMotorPreset => _motorPreset;
        public float LastCollisionImpulse => _collision != null ? _collision.LastImpulse : 0f;
        public float CumulativeDamage     => _collision != null ? _collision.CumulativeDamage : 0f;

        // Tuning accessors
        public float EngineForceMax => _engineForceMax; public float MaxSpeed    => _maxSpeed;
        public float BrakeForce     => _brakeForce;     public float ReverseForce => _reverseForce;
        public float CoastDrag      => _coastDrag;      public float ThrottleRampUp => _throttleRampUp;
        public float ThrottleRampDown => _throttleRampDown; public float SteeringMax => _steeringMax;
        public float SteeringSpeed    => _steeringSpeed;    public float SteeringSpeedLimit => _steeringSpeedLimit;
        public float SteeringHighSpeedFactor => _steeringHighSpeedFactor;
        public float FrontSpringStrength => _frontSpringStrength; public float FrontSpringDamping => _frontSpringDamping;
        public float RearSpringStrength  => _rearSpringStrength;  public float RearSpringDamping  => _rearSpringDamping;
        public float GripCoeff => _gripCoeff; public float ComGroundY => _comGround.y;
        public float TumbleEngageDeg => _tumbleEngageDeg; public float TumbleFullDeg => _tumbleFullDeg;
        internal RCAirPhysics AirPhysics => _airPhysics; internal Drivetrain DrivetrainRef => _drivetrain;
        public float Mass => _rb != null ? _rb.mass : k_DefaultMass;
        public static float FlipHeightOffset => k_FlipHeightOffset;

        // Private fields
        Rigidbody _rb; R8EOX.Input.IVehicleInput _input; RCAirPhysics _airPhysics; Drivetrain _drivetrain;
        WheelManager _wheels = new WheelManager(); TumbleController _tumble;
        AirborneDetector _airDetect = new AirborneDetector(5); bool _flipRequested;
        CollisionTracker _collision; float _brakeInput;
#if UNITY_EDITOR || DEBUG
        float _debugLogTimer;
#endif

        void Awake()
        {
            _rb = GetComponent<Rigidbody>(); _input = GetComponent<R8EOX.Input.IVehicleInput>();
            _airPhysics = GetComponentInChildren<RCAirPhysics>(); _drivetrain = GetComponentInChildren<Drivetrain>(); _collision = GetComponent<CollisionTracker>();
        }

        void Start()
        {
            ApplyMotorPreset();
            _rb.centerOfMass = _comGround;

            _tumble = new TumbleController();
            _wheels.Discover(transform);
            _wheels.Configure(gameObject.layer, _drivetrain,
                _frontSpringStrength, _frontSpringDamping, _rearSpringStrength, _rearSpringDamping, _gripCoeff);

#if UNITY_EDITOR || DEBUG
            Debug.Log($"[VehicleManager] Motor={_motorPreset} engine={_engineForceMax}N max={_maxSpeed}m/s " +
                      $"mass={_rb.mass}kg frontSpring={_frontSpringStrength} rearSpring={_rearSpringStrength} grip={_gripCoeff}");
#endif
        }

        void Update()
        {
            if (_input != null && _input.ResetPressed) _flipRequested = true;
            if (_input != null && _input.DebugTogglePressed)
                foreach (var w in _wheels.All) w.ShowDebug = !w.ShowDebug;
        }

        void FixedUpdate()
        {
            if (_rb == null) return;
            var frame = new VehicleFrame { Dt = Time.fixedDeltaTime, ReverseEngaged = ReverseEngaged };
            if (_collision != null) _collision.ResetFrame();
            if (_flipRequested) { _flipRequested = false; DoFlip(); }

            InputStage.Execute(ref frame, _input, _rb, transform,
                SmoothThrottle, _throttleRampUp, _throttleRampDown);
            AirborneStage.Execute(ref frame, _wheels, _airDetect, _tumble, transform,
                _tumbleEngageDeg, _tumbleFullDeg, _tumbleHysteresisDeg);
            _rb.centerOfMass = _comGround;
            GroundDriveStage.Execute(ref frame, _rb, transform, _wheels,
                _engineForceMax, _brakeForce, _reverseForce, _coastDrag, _maxSpeed);
            DrivetrainStage.Execute(ref frame, _drivetrain, _wheels);
            SteeringStage.Execute(ref frame, CurrentSteering,
                _steeringMax, _steeringSpeed, _steeringSpeedLimit, _steeringHighSpeedFactor);
            AirPhysicsStage.Execute(ref frame, _airPhysics);
            WheelSolveStage.Execute(ref frame, _rb, _wheels);

            // Sync frame → persistent state
            SmoothThrottle = frame.SmoothThrottle; ForwardSpeed = frame.ForwardSpeed;
            IsAirborne = frame.IsAirborne; TumbleFactor = frame.TumbleFactor; TiltAngle = frame.TiltAngle;
            CurrentEngineForce = frame.EngineForce; CurrentBrakeForce = frame.BrakeForce;
            ReverseEngaged = frame.ReverseEngaged; CurrentSteering = frame.CurrentSteering;
            _brakeInput = frame.BrakeInput;
#if UNITY_EDITOR || DEBUG
            _debugLogTimer += frame.Dt;
            if (_debugLogTimer >= 0.5f) { Debug.Log($"[esc] throttle={SmoothThrottle:F3} engineForce={CurrentEngineForce:F2}N brake={CurrentBrakeForce:F2}N reverse={ReverseEngaged} airborne={IsAirborne}"); _debugLogTimer = 0f; }
#endif
        }

        // Public API
        public float GetSpeedKmh()        => _rb != null ? _rb.linearVelocity.magnitude * k_MsToKmh : 0f;
        public float GetForwardSpeedKmh() => _rb != null ? Vector3.Dot(_rb.linearVelocity, transform.forward) * k_MsToKmh : 0f;

        public float GetSlip()
        {
            float slip = 0f; int count = 0;
            foreach (var w in _wheels.All) { if (w.IsMotor) { slip += w.SlipRatio; count++; } }
            return count > 0 ? slip / count : 0f;
        }

        public VehicleTelemetry GetTelemetry()
        {
            var allWheels = _wheels.All;
            var wt = new WheelTelemetry[allWheels.Length];
            int grounded = 0; float rpmSum = 0f; int motorCount = 0;
            for (int i = 0; i < allWheels.Length; i++)
            {
                var w = allWheels[i];
                wt[i] = new WheelTelemetry(w.ContactPoint, w.ContactNormal, w.SlipRatio, w.GripFactor,
                    w.WheelRpm, w.SuspensionForce, w.IsOnGround, 0);
                if (w.IsOnGround) grounded++;
                if (w.IsMotor) { rpmSum += w.WheelRpm; motorCount++; }
            }
            float engineRpm = motorCount > 0 ? (rpmSum / motorCount) * _gearRatio : 0f;
            float impulse = _collision != null ? _collision.LastImpulse : 0f;
            return new VehicleTelemetry(ForwardSpeed, GetSpeedKmh(), engineRpm,
                SmoothThrottle, _brakeInput, CurrentSteering,
                IsAirborne, TumbleFactor, TiltAngle, grounded, impulse, wt);
        }

        internal RaycastWheel[] GetAllWheels()
        { if (_wheels.All.Length == 0) _wheels.Discover(transform); return _wheels.All; }

        public void ApplySuspensionSettings() =>
            _wheels.PushSuspension(_frontSpringStrength, _frontSpringDamping, _rearSpringStrength, _rearSpringDamping);
        public void ApplyTractionSettings() => _wheels.PushGrip(_gripCoeff);

        public void SetMotorParams(float engineForce, float maxSpeed, float brakeForce, float reverseForce, float coastDrag)
        { _motorPreset = MotorPreset.Custom; _engineForceMax = engineForce; _maxSpeed = maxSpeed; _brakeForce = brakeForce; _reverseForce = reverseForce; _coastDrag = coastDrag; }
        public void SetThrottleResponse(float rampUp, float rampDown) { _throttleRampUp = rampUp; _throttleRampDown = rampDown; }
        public void SetSteeringParams(float max, float speed, float speedLimit, float highSpeedFactor)
        { _steeringMax = max; _steeringSpeed = speed; _steeringSpeedLimit = speedLimit; _steeringHighSpeedFactor = highSpeedFactor; }
        public void SetSuspension(float springStrength, float damping)
        { _frontSpringStrength = _rearSpringStrength = springStrength; _frontSpringDamping = _rearSpringDamping = damping; if (_wheels.All.Length > 0) ApplySuspensionSettings(); }
        public void SetAxleSuspension(float frontK, float frontDamp, float rearK, float rearDamp)
        { _frontSpringStrength = frontK; _frontSpringDamping = frontDamp; _rearSpringStrength = rearK; _rearSpringDamping = rearDamp; if (_wheels.All.Length > 0) ApplySuspensionSettings(); }
        public void SetTraction(float gripCoeff) { _gripCoeff = gripCoeff; if (_wheels.All.Length > 0) ApplyTractionSettings(); }
        public void SetCrashParams(float engageDeg, float fullDeg)
        { _tumbleEngageDeg = engageDeg; _tumbleFullDeg = fullDeg; }
        public void SetCentreOfMass(float groundY) => _comGround = new Vector3(0f, groundY, 0f);
        public void SetMass(float mass) { if (_rb != null) _rb.mass = mass; }
        public void SetGearRatio(float ratio) { _gearRatio = ratio; }
        internal void SelectMotorPreset(MotorPreset preset) { _motorPreset = preset; ApplyMotorPreset(); }

        public void Respawn(Vector3 position, Quaternion rotation)
        { transform.SetPositionAndRotation(position, rotation); ResetPhysics(); if (_collision != null) _collision.ResetDamage(); }

        public void ResetPhysics()
        { _rb.linearVelocity = _rb.angularVelocity = Vector3.zero; SmoothThrottle = 0f; CurrentSteering = 0f;
          CurrentEngineForce = 0f; CurrentBrakeForce = 0f; ReverseEngaged = false; _flipRequested = false; }

        public void SetPaused(bool paused) { _rb.isKinematic = paused; }

        // Private
        void ApplyMotorPreset()
        {
            var p = MotorPresetRegistry.Get(_motorPreset); if (p == null) return;
            var d = p.Value;
            _engineForceMax = d.EngineForceMax; _brakeForce = d.BrakeForce; _reverseForce = d.ReverseForce;
            _coastDrag = d.CoastDrag; _maxSpeed = d.MaxSpeed; _throttleRampUp = d.ThrottleRampUp;
        }

        void DoFlip()
        {
            Vector3 euler = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, euler.y, 0f);
            transform.position += Vector3.up * k_FlipHeightOffset;
            _rb.linearVelocity = _rb.angularVelocity = Vector3.zero;
        }
    }
}
