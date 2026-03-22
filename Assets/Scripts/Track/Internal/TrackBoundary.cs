using UnityEngine;

namespace R8EOX.Track.Internal
{
    internal class TrackBoundary : MonoBehaviour
    {
        [SerializeField] private float bounceForce = 5f;

        private void OnCollisionEnter(Collision collision)
        {
            // TODO: Apply bounce/penalty when vehicle hits boundary
        }
    }
}
