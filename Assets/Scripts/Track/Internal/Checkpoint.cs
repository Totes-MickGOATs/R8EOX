using UnityEngine;

namespace R8EOX.Track.Internal
{
    internal class Checkpoint : MonoBehaviour
    {
        [SerializeField] private int checkpointIndex;

        internal int Index => checkpointIndex;

        internal System.Action<int, GameObject> OnVehiclePassed;

        private void OnTriggerEnter(Collider other)
        {
            // TODO: Notify TrackManager when a vehicle passes through
            OnVehiclePassed?.Invoke(checkpointIndex, other.gameObject);
        }
    }
}
