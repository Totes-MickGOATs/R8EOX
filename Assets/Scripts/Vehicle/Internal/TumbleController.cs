using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    /// <summary>Computes tilt angle and tumble factor for telemetry and HUD.</summary>
    internal class TumbleController
    {
        bool _wasTumbling;

        public float TiltAngle   { get; private set; }
        public float TumbleFactor { get; private set; }

        public TumbleController() { }

        public void Update(Transform t, bool isAirborne,
            float engageDeg, float fullDeg, float hysteresisDeg)
        {
            TiltAngle = TumbleMath.ComputeTiltAngle(t.up);
            TumbleFactor = TumbleMath.ComputeTumbleFactor(
                TiltAngle, isAirborne, _wasTumbling,
                engageDeg, fullDeg, hysteresisDeg);
            _wasTumbling = TumbleFactor > 0f;
        }
    }
}
