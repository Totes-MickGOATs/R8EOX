using UnityEngine;

namespace R8EOX.Session.Internal
{
    internal static class VehicleValidator
    {
        internal static VehicleReadiness Validate(GameObject vehicleInstance)
        {
            var result = new VehicleReadiness();

            if (vehicleInstance == null)
                return result;

            result.HasRigidbody = vehicleInstance.GetComponent<Rigidbody>() != null;
            result.HasInput = vehicleInstance.GetComponent<R8EOX.Input.IVehicleInput>() != null;
            result.HasDrivetrain = vehicleInstance.GetComponentInChildren<R8EOX.Vehicle.Internal.Drivetrain>() != null;
            result.HasColliders = vehicleInstance.GetComponentInChildren<Collider>() != null;
            result.HasCollisionTracker = vehicleInstance.GetComponentInChildren<R8EOX.Vehicle.Internal.CollisionTracker>() != null;

            // Wheel check: need at least 2 RaycastWheel components
            var wheels = vehicleInstance.GetComponentsInChildren<R8EOX.Vehicle.Internal.RaycastWheel>();
            result.HasWheels = wheels != null && wheels.Length >= 2;

            // Attachment points discovered by name convention
            result.HasCameraTarget = vehicleInstance.transform.Find("CameraTarget") != null;
            result.HasCameraMountPoint = vehicleInstance.transform.Find("CameraMountPoint") != null;
            result.HasExhaustPoint = vehicleInstance.transform.Find("ExhaustPoint") != null;

            // VFX points: check if at least one wheel has a VFXPoint child
            bool hasAnyVfx = false;
            foreach (var w in wheels)
            {
                if (w.transform.Find("VFXPoint") != null)
                {
                    hasAnyVfx = true;
                    break;
                }
            }
            result.HasVFXPoints = hasAnyVfx;

            return result;
        }

        internal static void LogReadiness(VehicleReadiness readiness, string vehicleName)
        {
            if (readiness.IsFullyEquipped)
            {
                Debug.Log($"[VehicleValidator] {vehicleName}: fully equipped");
                return;
            }

            if (readiness.IsRaceReady)
            {
                string missing = readiness.GetMissingReport();
                Debug.LogWarning($"[VehicleValidator] {vehicleName}: race-ready but missing optional: {missing}");
                return;
            }

            if (readiness.IsPlayable)
            {
                string missing = readiness.GetMissingReport();
                Debug.LogWarning($"[VehicleValidator] {vehicleName}: playable but missing for race: {missing}");
                return;
            }

            string critical = readiness.GetMissingReport();
            Debug.LogError($"[VehicleValidator] {vehicleName}: NOT PLAYABLE — missing: {critical}");
        }
    }
}
