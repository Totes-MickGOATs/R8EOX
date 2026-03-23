using UnityEngine;
using R8EOX.Vehicle;
using R8EOX.VFX.Internal;

namespace R8EOX.VFX
{
    public class VFXManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TireMarks tireMarks;
        [SerializeField] private ExhaustEffect exhaustEffect;
        [SerializeField] private SparkEffect sparkEffect;
        [SerializeField] private ScreenEffects screenEffects;

        [Header("Thresholds")]
        [SerializeField] private float tireMarkSlipThreshold = 0.3f;
        [SerializeField] private float sparkMinForce = 100f;
        [SerializeField] private float maxSpeed = 100f;

        private VehicleManager vehicleManager;
        private bool hasTarget;
        private bool tireMarksInitialized;

        public void SetTarget(GameObject vehicle)
        {
            if (vehicle == null)
            {
                ClearTarget();
                return;
            }

            vehicleManager = vehicle.GetComponent<VehicleManager>();
            hasTarget = vehicleManager != null;
            tireMarksInitialized = false;
        }

        public void ClearTarget()
        {
            vehicleManager = null;
            hasTarget = false;
            tireMarksInitialized = false;

            if (tireMarks != null) tireMarks.ClearAllMarks();
            if (screenEffects != null) screenEffects.ClearAll();
        }

        private void LateUpdate()
        {
            if (!hasTarget) return;

            var telemetry = vehicleManager.GetTelemetry();

            InitializeTireMarksIfNeeded(telemetry);
            UpdateTireMarks(telemetry);
            UpdateExhaust(telemetry);
            UpdateSparks(telemetry);
            UpdateScreenEffects(telemetry);
        }

        private void InitializeTireMarksIfNeeded(VehicleTelemetry telemetry)
        {
            if (tireMarksInitialized || tireMarks == null) return;
            if (telemetry.Wheels == null || telemetry.Wheels.Length == 0) return;

            tireMarks.Initialize(telemetry.Wheels.Length);
            tireMarksInitialized = true;
        }

        private void UpdateTireMarks(VehicleTelemetry telemetry)
        {
            if (tireMarks == null || telemetry.Wheels == null) return;

            for (int i = 0; i < telemetry.Wheels.Length; i++)
            {
                var wheel = telemetry.Wheels[i];
                bool shouldMark = wheel.IsOnGround
                    && wheel.SlipRatio > tireMarkSlipThreshold;
                float intensity = shouldMark
                    ? Mathf.InverseLerp(tireMarkSlipThreshold, 1f, wheel.SlipRatio)
                    : 0f;

                tireMarks.UpdateWheel(i, wheel.ContactPoint, shouldMark, intensity);
            }
        }

        private void UpdateExhaust(VehicleTelemetry telemetry)
        {
            if (exhaustEffect == null) return;
            exhaustEffect.UpdateExhaust(telemetry.ThrottleAmount);
        }

        private void UpdateSparks(VehicleTelemetry telemetry)
        {
            if (sparkEffect == null) return;
            if (telemetry.LastCollisionImpulse <= sparkMinForce) return;

            float intensity = telemetry.LastCollisionImpulse / (sparkMinForce * 10f);
            Vector3 sparkPosition;
            Vector3 sparkNormal;

            if (TryGetFirstGroundedWheel(telemetry, out var wheel))
            {
                sparkPosition = wheel.ContactPoint;
                sparkNormal = wheel.ContactNormal;
            }
            else
            {
                sparkPosition = vehicleManager.transform.position;
                sparkNormal = Vector3.up;
            }

            sparkEffect.SpawnSparks(sparkPosition, sparkNormal, intensity);
        }

        private void UpdateScreenEffects(VehicleTelemetry telemetry)
        {
            if (screenEffects == null) return;

            float normalizedSpeed = maxSpeed > 0f
                ? Mathf.Clamp01(telemetry.SpeedKmh / maxSpeed)
                : 0f;
            screenEffects.SetSpeedBlur(normalizedSpeed);
        }

        private bool TryGetFirstGroundedWheel(
            VehicleTelemetry telemetry, out WheelTelemetry wheel)
        {
            if (telemetry.Wheels != null)
            {
                for (int i = 0; i < telemetry.Wheels.Length; i++)
                {
                    if (telemetry.Wheels[i].IsOnGround)
                    {
                        wheel = telemetry.Wheels[i];
                        return true;
                    }
                }
            }

            wheel = default;
            return false;
        }

        // Manual override API for external system calls
        public void SpawnTireMarks(
            Vector3 position, Vector3 direction, float intensity)
        {
            // Legacy API kept for external callers
        }

        public void SpawnExhaust(Vector3 position, float throttle)
        {
            if (exhaustEffect != null) exhaustEffect.UpdateExhaust(throttle);
        }

        public void SpawnSparks(Vector3 position, Vector3 normal)
        {
            if (sparkEffect != null) sparkEffect.SpawnSparks(position, normal, 1f);
        }

        public void SetSpeedBlur(float speed, float maxSpeedOverride)
        {
            if (screenEffects == null) return;
            float normalized = maxSpeedOverride > 0f
                ? Mathf.Clamp01(speed / maxSpeedOverride)
                : 0f;
            screenEffects.SetSpeedBlur(normalized);
        }

        public void SetDamageVignette(float damagePercent)
        {
            if (screenEffects != null)
                screenEffects.SetDamageVignette(damagePercent);
        }
    }
}
