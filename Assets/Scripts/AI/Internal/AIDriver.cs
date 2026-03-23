using UnityEngine;

namespace R8EOX.AI.Internal
{
    /// <summary>
    /// Per-vehicle AI controller. Reads the centerline via RacingLine,
    /// calculates throttle/brake/steering, and writes to ScriptedInput.
    /// </summary>
    internal class AIDriver
    {
        private GameObject vehicle;
        private R8EOX.Input.Internal.ScriptedInput scriptedInput;
        private R8EOX.Vehicle.VehicleManager vehicleManager;
        private RacingLine racingLine;
        private AIBehavior behavior;

        // Tuning constants
        private const float BaseLookaheadDistance = 15f;
        private const float SpeedLookaheadScale = 0.5f;
        private const float MaxThrottle = 1f;
        private const float CurvatureBrakeThreshold = 0.1f;
        private const float SteeringSharpness = 3f;
        private const float BrakeCurvatureScale = 10f;
        private const float ThrottleCurvatureScale = 5f;
        private const float MinBrakeSpeed = 5f;

        /// <summary>True while the vehicle GameObject still exists.</summary>
        internal bool IsValid => vehicle != null;

        internal void Initialize(
            GameObject targetVehicle,
            R8EOX.Input.Internal.ScriptedInput input,
            R8EOX.Vehicle.VehicleManager vm,
            RacingLine line,
            AIBehavior beh)
        {
            vehicle = targetVehicle;
            scriptedInput = input;
            vehicleManager = vm;
            racingLine = line;
            behavior = beh;
        }

        internal void Tick(float deltaTime)
        {
            if (!IsValid || scriptedInput == null || racingLine == null)
                return;

            Vector3 pos = vehicle.transform.position;
            float speed = vehicleManager != null ? vehicleManager.ForwardSpeed : 0f;
            float currentDist = racingLine.GetCurrentDistance(pos);

            float lookahead = BaseLookaheadDistance + Mathf.Abs(speed) * SpeedLookaheadScale;
            Vector3 targetPoint = racingLine.GetLookaheadPoint(currentDist, lookahead);

            float steer = CalculateSteering(targetPoint, pos);
            float curvature = racingLine.GetCurvatureAhead(currentDist, lookahead * 0.5f);
            float throttle = CalculateThrottle(curvature);
            float brake = CalculateBrake(curvature, speed);

            scriptedInput.Throttle = throttle;
            scriptedInput.Brake = brake;
            scriptedInput.Steer = steer;
        }

        private float CalculateSteering(Vector3 targetPoint, Vector3 currentPos)
        {
            Vector3 toTarget = (targetPoint - currentPos);
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude < 0.01f)
                return 0f;

            toTarget.Normalize();
            float dot = Vector3.Dot(vehicle.transform.right, toTarget);
            return Mathf.Clamp(dot * SteeringSharpness, -1f, 1f);
        }

        private float CalculateThrottle(float curvature)
        {
            float baseThrottle = Mathf.Clamp01(MaxThrottle - curvature * ThrottleCurvatureScale);
            float throttle = behavior.ModifyThrottle(baseThrottle);
            throttle *= behavior.GetMaxSpeedFactor();
            return Mathf.Clamp01(throttle);
        }

        private float CalculateBrake(float curvature, float speed)
        {
            float threshold = CurvatureBrakeThreshold * behavior.GetBrakeSensitivity();

            if (curvature <= threshold || speed <= MinBrakeSpeed)
                return 0f;

            return Mathf.Clamp01((curvature - threshold) * BrakeCurvatureScale);
        }
    }
}
