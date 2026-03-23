namespace R8EOX.Session.Internal
{
    internal struct VehicleReadiness
    {
        internal bool HasRigidbody;
        internal bool HasInput;
        internal bool HasWheels;
        internal bool HasDrivetrain;
        internal bool HasColliders;
        internal bool HasCollisionTracker;
        internal bool HasCameraTarget;
        internal bool HasCameraMountPoint;
        internal bool HasExhaustPoint;
        internal bool HasVFXPoints;

        internal bool IsPlayable => HasRigidbody && HasWheels && HasInput;
        internal bool IsRaceReady => IsPlayable && HasDrivetrain && HasColliders && HasCameraTarget;
        internal bool IsFullyEquipped => IsRaceReady && HasCameraTarget && HasCollisionTracker && HasVFXPoints;

        internal string GetMissingReport()
        {
            var sb = new System.Text.StringBuilder();
            if (!HasRigidbody) sb.Append("Rigidbody, ");
            if (!HasInput) sb.Append("IVehicleInput, ");
            if (!HasWheels) sb.Append("RaycastWheel (min 2), ");
            if (!HasDrivetrain) sb.Append("Drivetrain, ");
            if (!HasColliders) sb.Append("Colliders, ");
            if (!HasCollisionTracker) sb.Append("CollisionTracker, ");
            if (!HasCameraTarget) sb.Append("CameraTarget transform, ");
            if (!HasCameraMountPoint) sb.Append("CameraMountPoint transform, ");
            if (!HasExhaustPoint) sb.Append("ExhaustPoint transform, ");
            if (!HasVFXPoints) sb.Append("Wheel VFXPoints, ");
            if (sb.Length > 2) sb.Length -= 2; // trim trailing ", "
            return sb.Length > 0 ? sb.ToString() : "None";
        }
    }
}
