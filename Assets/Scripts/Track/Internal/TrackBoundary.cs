using UnityEngine;

namespace R8EOX.Track.Internal
{
    internal class TrackBoundary : MonoBehaviour
    {
        [SerializeField] private float bounceForce = 5f;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.contactCount == 0) return;

            var rb = collision.rigidbody;
            if (rb == null) return;

            Vector3 normal = collision.GetContact(0).normal;
            rb.AddForce(normal * bounceForce, ForceMode.VelocityChange);
        }
    }
}
