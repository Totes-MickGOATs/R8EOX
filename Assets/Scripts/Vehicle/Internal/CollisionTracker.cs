using UnityEngine;

namespace R8EOX.Vehicle.Internal
{
    /// <summary>
    /// Tracks collision events and accumulates damage data.
    /// Discovered by VehicleManager via GetComponent.
    /// </summary>
    internal class CollisionTracker : MonoBehaviour
    {
        const float k_DamageThreshold = 5f;    // Min impulse (N) to register damage
        const float k_DamageScale = 0.01f;     // Impulse-to-damage conversion factor
        const float k_MaxDamage = 1f;          // Damage capped at 1.0 (100%)

        // Readable by VehicleManager each frame
        internal float LastImpulse { get; private set; }
        internal Vector3 LastContactPoint { get; private set; }
        internal int LastContactLayer { get; private set; }
        internal float CumulativeDamage { get; private set; }

        // Reset per-frame data (called by VehicleManager at start of FixedUpdate)
        internal void ResetFrame()
        {
            LastImpulse = 0f;
        }

        // Reset all damage state (called on respawn)
        internal void ResetDamage()
        {
            CumulativeDamage = 0f;
            LastImpulse = 0f;
            LastContactPoint = Vector3.zero;
            LastContactLayer = 0;
        }

        void OnCollisionEnter(Collision collision)
        {
            ProcessCollision(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            // Only process if this frame's collision is stronger than what we already tracked
            float impulse = collision.impulse.magnitude;
            if (impulse > LastImpulse)
                ProcessCollision(collision);
        }

        void ProcessCollision(Collision collision)
        {
            float impulse = collision.impulse.magnitude;
            LastImpulse = impulse;

            if (collision.contactCount > 0)
            {
                ContactPoint contact = collision.GetContact(0);
                LastContactPoint = contact.point;
                LastContactLayer = collision.gameObject.layer;
            }

            if (impulse > k_DamageThreshold)
            {
                float damage = (impulse - k_DamageThreshold) * k_DamageScale;
                CumulativeDamage = Mathf.Min(CumulativeDamage + damage, k_MaxDamage);
            }
        }
    }
}
