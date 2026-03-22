using UnityEngine;
using R8EOX.Vehicle;

namespace R8EOX.PhysicsTest.Internal
{
    internal class PathFollower : MonoBehaviour
    {
        [Header("Speed")]
        [Tooltip("Target forward speed in metres per second.")]
        [SerializeField] private float targetSpeedMps = 5f;

        [Header("Path")]
        [Tooltip("Distance ahead on the path used as a steering target (metres).")]
        [SerializeField] private float lookaheadDistance = 1.5f;

        [Header("Steering")]
        [Tooltip("Gain applied to the cross-product steering signal.")]
        [SerializeField] private float steerGain = 4f;

        private WaypointPath _path;
        private VehicleManager _vehicle;
        private IWritableInput _input;

        private float _previousT;
        private int _currentLap;

        internal float TargetSpeedMps
        {
            get => targetSpeedMps;
            set => targetSpeedMps = value;
        }

        internal float LookaheadDistance
        {
            get => lookaheadDistance;
            set => lookaheadDistance = value;
        }

        internal int CurrentLap => _currentLap;
        internal float PathProgress { get; private set; }

        internal void Initialize(WaypointPath path, VehicleManager vehicle, IWritableInput input)
        {
            _path    = path;
            _vehicle = vehicle;
            _input   = input;
        }

        private void FixedUpdate()
        {
            if (_path == null || _vehicle == null || _input == null)
                return;

            Vector3 vehiclePos = _vehicle.transform.position;
            float currentT = _path.GetNearestT(vehiclePos);
            PathProgress = currentT;

            if (_previousT > 0.9f && currentT < 0.1f)
                _currentLap++;
            _previousT = currentT;

            ComputeAndWriteControls(currentT, vehiclePos);
        }

        private void ComputeAndWriteControls(float currentT, Vector3 vehiclePos)
        {
            float lookaheadT = currentT + (lookaheadDistance / _path.TotalLength);
            if (lookaheadT > 1f) lookaheadT -= 1f;

            Vector3 targetPos = _path.GetPosition(lookaheadT);

            Vector3 toTarget = targetPos - vehiclePos;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude < 0.0001f)
            {
                _input.Throttle = 0f;
                _input.Brake    = 0f;
                _input.Steer    = 0f;
                return;
            }
            toTarget.Normalize();

            Vector3 vehicleFwd = _vehicle.transform.forward;
            vehicleFwd.y = 0f;
            vehicleFwd.Normalize();

            // Y component of cross product gives signed turn direction
            float cross = vehicleFwd.x * toTarget.z - vehicleFwd.z * toTarget.x;
            float dot   = Vector3.Dot(vehicleFwd, toTarget);

            _input.Steer    = Mathf.Clamp(-cross * steerGain, -1f, 1f);
            _input.Throttle = ComputeThrottle(_vehicle.ForwardSpeed, dot);
            _input.Brake    = ComputeBrake(_vehicle.ForwardSpeed);
        }

        private float ComputeThrottle(float speed, float alignmentDot)
        {
            const float overspeedMultiplier = 1.2f;
            if (speed > targetSpeedMps * overspeedMultiplier || speed >= targetSpeedMps)
                return 0f;
            return Mathf.Clamp01(alignmentDot);
        }

        private float ComputeBrake(float speed)
        {
            const float overspeedMultiplier = 1.2f;
            const float brakeAmount = 0.5f;
            return speed > targetSpeedMps * overspeedMultiplier ? brakeAmount : 0f;
        }
    }
}
