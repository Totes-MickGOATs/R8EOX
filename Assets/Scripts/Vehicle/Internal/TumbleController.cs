using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    /// <summary>Computes tumble factor and blends physics material during crashes.</summary>
    internal class TumbleController
    {
        const float k_DefaultBounciness = 0.05f;

        readonly PhysicsMaterial _physMat;
        bool _wasTumbling;

        public float TiltAngle   { get; private set; }
        public float TumbleFactor { get; private set; }

        public TumbleController(PhysicsMaterial physMat) { _physMat = physMat; }

        public void Update(Transform t, bool isAirborne,
            float engageDeg, float fullDeg, float hysteresisDeg,
            bool enableDynamic, float bounce, float friction)
        {
            TiltAngle = TumbleMath.ComputeTiltAngle(t.up);
            TumbleFactor = TumbleMath.ComputeTumbleFactor(
                TiltAngle, isAirborne, _wasTumbling,
                engageDeg, fullDeg, hysteresisDeg);
            _wasTumbling = TumbleFactor > 0f;

            if (!enableDynamic || _physMat == null) return;
            _physMat.bounciness      = Mathf.Lerp(k_DefaultBounciness, bounce,  TumbleFactor);
            _physMat.dynamicFriction = Mathf.Lerp(0f,                  friction, TumbleFactor);
            _physMat.staticFriction  = Mathf.Lerp(0f,                  friction, TumbleFactor);
        }
    }
}
