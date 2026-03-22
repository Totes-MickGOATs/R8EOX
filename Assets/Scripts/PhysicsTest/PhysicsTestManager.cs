using UnityEngine;

namespace R8EOX.PhysicsTest
{
    /// <summary>
    /// Top-level coordinator for the physics test track.
    /// Wires WaypointPath, PathFollower, ScriptedInput, PlaybackController,
    /// and DebugOverlay into a single scene-resident MonoBehaviour.
    /// </summary>
    public class PhysicsTestManager : MonoBehaviour
    {
        // ---- Serialized fields -----------------------------------------------

        [Header("Path")]
        [Tooltip("WaypointPath that defines the test loop.")]
        [SerializeField] private Internal.WaypointPath waypointPath;

        [Header("Vehicle")]
        [Tooltip("RC buggy prefab to instantiate. Mutually exclusive with vehicleInScene.")]
        [SerializeField] private GameObject vehiclePrefab;

        [Tooltip("Assign an already-spawned VehicleManager here when not using a prefab.")]
        [SerializeField] private R8EOX.Vehicle.VehicleManager vehicleInScene;

        [Tooltip("Where to spawn the vehicle (used only when vehiclePrefab is set).")]
        [SerializeField] private Transform spawnPoint;

        [Header("Segments")]
        [Tooltip("Display names for each test segment, shown in the debug overlay.")]
        [SerializeField] private string[] segmentNames = { "Straight", "Banked Turn", "Speed Bumps", "Jump", "Washboard", "Sweeper" };

        [Tooltip("Waypoint index that marks the start of each segment.")]
        [SerializeField] private int[] segmentWaypointIndices = { 0, 3, 6, 9, 12, 15 };

        // ---- Private state ---------------------------------------------------

        private GameObject _spawnedVehicle;
        private R8EOX.Vehicle.VehicleManager _vehicleManager;
        private R8EOX.Input.Internal.ScriptedInput _scriptedInput;
        private Internal.PathFollower _pathFollower;
        private Internal.PlaybackController _playback;
        private Internal.DebugOverlay _overlay;

        // ---- Public API ------------------------------------------------------

        public string[] SegmentNames => segmentNames;
        public int CurrentSegment { get; private set; }

        // ---- Unity lifecycle -------------------------------------------------

        private void Start()
        {
            _playback = gameObject.AddComponent<Internal.PlaybackController>();
            _overlay  = gameObject.AddComponent<Internal.DebugOverlay>();

            SetupVehicle();

            if (_vehicleManager != null)
                _overlay.Initialize(_playback, _pathFollower, _vehicleManager, segmentNames, JumpToSegment);
        }

        // ---- Internal setup --------------------------------------------------

        private void SetupVehicle()
        {
            if (vehiclePrefab != null)
            {
                Vector3    pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
                Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
                _spawnedVehicle = Instantiate(vehiclePrefab, pos, rot);
                _vehicleManager = _spawnedVehicle.GetComponent<R8EOX.Vehicle.VehicleManager>();
            }
            else if (vehicleInScene != null)
            {
                _vehicleManager = vehicleInScene;
                _spawnedVehicle = vehicleInScene.gameObject;
            }
            else
            {
                Debug.LogError("[PhysicsTestManager] Assign vehiclePrefab or vehicleInScene in the Inspector.", this);
                return;
            }

            if (_vehicleManager == null)
            {
                Debug.LogError("[PhysicsTestManager] Vehicle has no VehicleManager component.", this);
                return;
            }

            // Remove player input so it does not compete with scripted driving
            var rcInput = _spawnedVehicle.GetComponent<R8EOX.Input.RCInput>();
            if (rcInput != null)
                Destroy(rcInput);

            _scriptedInput = _spawnedVehicle.AddComponent<R8EOX.Input.Internal.ScriptedInput>();

            _pathFollower = gameObject.AddComponent<Internal.PathFollower>();
            _pathFollower.Initialize(waypointPath, _vehicleManager, new Internal.InputAdapter(_scriptedInput));
        }

        // ---- Segment jumping -------------------------------------------------

        /// <summary>Teleports the vehicle to the named test segment and resets momentum.</summary>
        public void JumpToSegment(int index)
        {
            if (index < 0 || index >= segmentWaypointIndices.Length)
                return;

            CurrentSegment = index;

            int        waypointIndex = segmentWaypointIndices[index];
            Vector3    position      = waypointPath.GetWaypointPosition(waypointIndex);
            float      nearestT      = waypointPath.GetNearestT(position);
            Vector3    direction     = waypointPath.GetDirection(nearestT);
            Quaternion rotation      = direction != Vector3.zero
                ? Quaternion.LookRotation(direction)
                : Quaternion.identity;

            _spawnedVehicle.transform.SetPositionAndRotation(position, rotation);

            var rb = _vehicleManager.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity  = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
